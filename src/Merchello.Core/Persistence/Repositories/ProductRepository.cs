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

        public ProductRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public ProductRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
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
                var factory = new ProductFactory();
                var dtos = Database.Fetch<ProductDto, ProductVariantDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
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
               .Where<ProductVariantDto>(x => x.Template);
            
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
         //           "DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Id)",
           //         "DELETE FROM merchProductAttribute WHERE optionId IN (SELECT optionId FROM merchOption WHERE id IN (SELECT optionId FROM merchProduct2Option WHERE productKey = @Id))",
                    "DELETE FROM merchProduct2ProductOption WHERE productKey = @Id",
               //     "DELETE FROM merchInventory WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Id)",
                    "DELETE FROM merchProductVariant WHERE productKey = @Id",
                    "DELETE FROM merchProduct WHERE pk = @Id",
                    "DELETE FROM merchProductOption WHERE id NOT IN (SELECT optionId FROM merchProduct2ProductOption)"
                };

            return list;
        }


        
        //public void SaveProductOption(IProduct product, IProductOption productOption)
        //{
        //    var factory = new ProductOptionFactory();

        //    if (!productOption.HasIdentity)
        //    {
        //        ((IdEntity)productOption).AddingEntity();
        //        var dto = factory.BuildDto(productOption);

        //        Database.Insert(dto);
        //        productOption.Id = dto.Id;

        //        // associate the product with the product option
        //        var association = new Product2ProductOptionDto()
        //            {
        //                ProductKey = product.Key,
        //                OptionId = productOption.Id,
        //                CreateDate = DateTime.Now,
        //                UpdateDate = DateTime.Now
        //            };

        //        Database.Insert(association);
        //        ((Product)product).AddProductOption(productOption);
        //    }
        //    else
        //    {
        //        ((IdEntity)productOption).UpdatingEntity();
        //        var dto = factory.BuildDto(productOption);

        //        Database.Update(dto);
        //    }
            
        //}

        protected override void PersistNewItem(IProduct entity)
        {
            Mandate.ParameterCondition(SkuExists(entity.Sku) == false, "Skus must be unique.");

            ((Product)entity).AddingEntity();
            
            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            dto.ProductVariantDto.ProductKey = dto.Key;
            entity.Key = dto.Key;

            Database.Insert(dto.ProductVariantDto);

            ((Product) entity).ProductVariantTemplate.ProductKey = dto.ProductVariantDto.ProductKey;
            
            // if product has options
            if(entity.DefinesOptions)


            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IProduct entity)
        {
            ((Product)entity).UpdatingEntity();

            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            Database.Update(dto.ProductVariantDto);

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


        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        public bool SkuExists(string sku)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductVariantDto>()
               .Where<ProductVariantDto>(x => x.Sku == sku);

            return Database.Fetch<ProductVariantDto>(sql).Any();

        }

    }
}
