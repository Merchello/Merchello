using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class ProductRepository : MerchelloPetaPocoRepositoryBase<Guid, IProduct>, IProductRepository
    {
        private IProductVariantRepository _productVariantRepository;

        public ProductRepository(IDatabaseUnitOfWork work, IProductVariantRepository productVariantRepository)
            : base(work)
        {
            Mandate.ParameterNotNull(productVariantRepository, "productVariantRepository");
            _productVariantRepository = productVariantRepository;
        }
        
        public ProductRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache, IProductVariantRepository productVariantRepository)
            : base(work, cache)
        {
           Mandate.ParameterNotNull(productVariantRepository, "productVariantRepository");
           _productVariantRepository = productVariantRepository;
        }

        #region Overrides of RepositoryBase<IProduct>


        protected override IProduct PerformGet(Guid id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<ProductDto, ProductVariantDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ProductFactory();
            var product = factory.BuildEntity(dto);

            // Build the list of options
            product.ProductOptions = GetProductOptionCollection(product.Key);

            product.ResetDirtyProperties();

            return product;
        }

        protected override IEnumerable<IProduct> PerformGetAll(params Guid[] ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    yield return Get(id);
                }
            }
            else
            {
                //var factory = new ProductFactory();
                var dtos = Database.Fetch<ProductDto, ProductVariantDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IProduct>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<ProductDto>()
               .InnerJoin<ProductVariantDto>()
               .On<ProductDto, ProductVariantDto>(left => left.Key, right => right.ProductKey)
               .Where<ProductVariantDto>(x => x.Master);
            
            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchProduct.pk = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Id)",
                    @"DELETE FROM merchProductAttribute WHERE optionId IN 
                        (SELECT optionId FROM merchProductOption WHERE id IN 
                        (SELECT optionId FROM merchProduct2ProductOption WHERE productKey = @Id))",
                    "DELETE FROM merchProduct2ProductOption WHERE productKey = @Id",
                    "DELETE FROM merchInventory WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Id)",
                    "DELETE FROM merchProductVariant WHERE productKey = @Id",
                    "DELETE FROM merchProduct WHERE pk = @Id",
                    "DELETE FROM merchProductOption WHERE id NOT IN (SELECT optionId FROM merchProduct2ProductOption)"
                };

            return list;
        }

        protected override void PersistNewItem(IProduct entity)
        {
            Mandate.ParameterCondition(SkuExists(entity.Sku) == false, "Skus must be unique.");

            ((Product)entity).AddingEntity();
            
            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            // save the product
            Database.Insert(dto);
            entity.Key = dto.Key;
            
            // setup and save the master (singular) variant
            dto.ProductVariantDto.ProductKey = dto.Key;
            Database.Insert(dto.ProductVariantDto);

            ((Product) entity).ProductVariantMaster.ProductKey = dto.ProductVariantDto.ProductKey;
            
            // save the product options
            SaveProductOptions(entity);

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IProduct entity)
        {
            ((Product)entity).UpdatingEntity();

            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            Database.Update(dto.ProductVariantDto);
            
            SaveProductOptions(entity);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IProduct entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Key });
            }
        }

        protected override IEnumerable<IProduct> PerformGetByQuery(IQuery<IProduct> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProduct>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductDto, ProductVariantDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }

        #endregion

        #region Product Options and Attributes

        private ProductOptionCollection GetProductOptionCollection(Guid productKey)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductOptionDto>()
               .InnerJoin<Product2ProductOptionDto>()
               .On<ProductOptionDto, Product2ProductOptionDto>(left => left.Id, right => right.OptionId)
               .Where<Product2ProductOptionDto>(x => x.ProductKey == productKey)
               .OrderBy<Product2ProductOptionDto>(x => x.SortOrder);

            var dtos = Database.Fetch<ProductOptionDto, Product2ProductOptionDto>(sql);

            var productOptions = new ProductOptionCollection();
            var factory = new ProductOptionFactory();
            foreach (var option in dtos.Select(factory.BuildEntity))
            {
                var attributes = GetProductAttributeCollection(option.Id);
                option.Choices = attributes;
                productOptions.Insert(0, option);
            }

            return productOptions;
        }

        private void DeleteProductOption(IProductOption option)
        {
            var executeClauses = new[]
                {
                    "DELETE FROM merchProductVariant2ProductAttribute WHERE optionId = @Id",
                    "DELETE FROM merchProduct2ProductOption WHERE optionId = @Id",
                    "DELETE FROM merchProductAttribute WHERE optionId = @Id",
                    "DELETE FROM merchProductOption WHERE id = @Id"
                };

            foreach (var clause in executeClauses)
            {
                Database.Execute(clause, new { Id = option.Id });
            }
        }

        private void SaveProductOptions(IProduct product)
        {
            if (!product.DefinesOptions) return;

            var existing = GetProductOptionCollection(product.Key);
            //ensure all ids are in the new list
            var resetSorts = false;
            foreach (var ex in existing)
            {
                if (!product.ProductOptions.Contains(ex.Name))
                {
                    DeleteProductOption(ex);
                    resetSorts = true;
                }
            }
            if (resetSorts)
            {
                var count = 1;
                foreach (var o in product.ProductOptions.OrderBy(x => x.SortOrder))
                {
                    o.SortOrder = count;
                    count = count + 1;
                    product.ProductOptions.Add(o);
                }
            }

            foreach (var option in product.ProductOptions)
            {
                SaveProductOption(product, option);
            }
        }

        private void SaveProductOption(IProduct product, IProductOption productOption)
        {
            var factory = new ProductOptionFactory();

            if (!productOption.HasIdentity)
            {
                ((IdEntity)productOption).AddingEntity();
                var dto = factory.BuildDto(productOption);

                Database.Insert(dto);
                productOption.Id = dto.Id;

                // associate the product with the product option
                var association = new Product2ProductOptionDto()
                {
                    ProductKey = product.Key,
                    OptionId = productOption.Id,
                    SortOrder = productOption.SortOrder,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                Database.Insert(association);

            }
            else
            {
                ((IdEntity)productOption).UpdatingEntity();
                var dto = factory.BuildDto(productOption);
                Database.Update(dto);

                // TODO : this should be refactored
                const string update = "UPDATE merchProduct2ProductOption SET SortOrder = @So, updateDate = @Ud WHERE productKey = @pk AND optionId = @Oid";

                Database.Execute(update,
                                 new
                                 {
                                     So = productOption.SortOrder,
                                     Ud = productOption.UpdateDate,
                                     pk = product.Key,
                                     Oid = productOption.Id
                                 });


            }

            // now save the product attributes
            SaveProductAttributes(productOption);            
        }

        private ProductAttributeCollection GetProductAttributeCollection(int optionId)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductAttributeDto>()
               .Where<ProductAttributeDto>(x => x.OptionId == optionId);

            var dtos = Database.Fetch<ProductAttributeDto>(sql);

            var attributes = new ProductAttributeCollection();
            var factory = new ProductAttributeFactory();

            foreach (var dto in dtos)
            {
                var attribute = factory.BuildEntity(dto);
                attributes.Insert(attribute.SortOrder - 1, attribute);
            }
            return attributes;
        }

        private void DeleteProductAttribute(IProductAttribute productAttribute)
        {
            // TODO : this is sort of hacky but we want ProductVariant events to trigger on a ProductVariant Delete
            // and we need to delete all variants that had the attribute that is to be deleted so the current solution
            // is to delete all associations from the merchProductVariant2ProductAttribute table so that the follow up
            // EnsureProductVariantsHaveAttributes called in the ProductVariantService cleans up the orphaned variants and fires off
            // the events
            var sql = new Sql();
            sql.Select("*")
               .From<ProductVariant2ProductAttributeDto>()
               .Where<ProductVariant2ProductAttributeDto>(x => x.ProductAttributeId == productAttribute.Id);

            var dtos = Database.Fetch<ProductVariant2ProductAttributeDto>(sql);
            foreach (var dto in dtos)
            {
                Database.Delete(dto);
            }
            
            Database.Execute("DELETE FROM merchProductAttribute WHERE Id = @Id", new { Id = productAttribute.Id });
                      
        }

        private void SaveProductAttributes(IProductOption productOption)
        {
            if (!productOption.Choices.Any()) return;

            var existing = GetProductAttributeCollection(productOption.Id);

            //ensure all ids are in the new list
            var resetSorts = false;
            foreach (var ex in existing)
            {
                if (productOption.Choices.Contains(ex.Sku)) continue;
                DeleteProductAttribute(ex);
                resetSorts = true;
            }
            if (resetSorts)
            {
                var count = 1;
                foreach (var o in productOption.Choices.OrderBy(x => x.SortOrder))
                {
                    o.SortOrder = count;
                    count = count + 1;
                    productOption.Choices.Add(o);
                }
            }

            foreach (var att in productOption.Choices.OrderBy(x => x.SortOrder))
            {
                // ensure the id is set
                att.OptionId = productOption.Id;
                SaveProductAttribute(att);
            }
        }

        private void SaveProductAttribute(IProductAttribute productAttribute)
        {
            var factory = new ProductAttributeFactory();

            if (!productAttribute.HasIdentity)
            {
                ((IdEntity)productAttribute).AddingEntity();
                var dto = factory.BuildDto(productAttribute);
                Database.Insert(dto);
                productAttribute.Id = dto.Id;
            }
            else
            {
                ((IdEntity)productAttribute).UpdatingEntity();
                var dto = factory.BuildDto(productAttribute);
                Database.Update(dto);
            }
        }

        #endregion

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        public bool SkuExists(string sku)
        {
            return _productVariantRepository.SkuExists(sku);
        }



    }
}
