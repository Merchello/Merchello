namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

    internal class ProductRepository : PagedRepositoryBase<IProduct, ProductDto>, IProductRepository
    {
        private readonly IProductVariantRepository _productVariantRepository;
        

        public ProductRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, IProductVariantRepository productVariantRepository)
            : base(work, cache)
        {
           Mandate.ParameterNotNull(productVariantRepository, "productVariantRepository");
           _productVariantRepository = productVariantRepository;        
        }

        //// TODO this is a total hack and needs to be thought through a bit better.  IQuery is a worthless parameter here
        public override Page<IProduct> GetPage(long page, long itemsPerPage, IQuery<IProduct> query, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            var p = SearchKeys(string.Empty, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<IProduct>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(Get).ToList()
            };
        }

        public override Page<Guid> GetPagedKeys(long page, long itemsPerPage, IQuery<IProduct> query, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            return SearchKeys(string.Empty, page, itemsPerPage, orderExpression, sortDirection);
        }

        /// <summary>
        /// Searches the 
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public override Page<Guid> SearchKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            searchTerm = searchTerm.Replace(",", " ");
            var invidualTerms = searchTerm.Split(' ');

            var terms = invidualTerms.Where(x => !string.IsNullOrEmpty(x)).ToList();


            var sql = new Sql();
            sql.Select("*").From<ProductVariantDto>();

            if (terms.Any())
            {
                var preparedTerms = string.Format("%{0}%", string.Join("%", terms));

                sql.Where("sku LIKE @sku OR name LIKE @name", new { @sku = preparedTerms, @name = preparedTerms });
            }

            sql.Where("master = @master", new { @master = true });

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Get the paged keys.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sql">
        /// The <see cref="Sql"/>.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        protected override Page<Guid> GetPagedKeys(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            var p = GetDtoPage(page, itemsPerPage, sql, orderExpression, sortDirection);

            return new Page<Guid>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(x => x.ProductKey).ToList()
            };
        }

        private Page<ProductVariantDto> GetDtoPage(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            if (!string.IsNullOrEmpty(orderExpression))
            {
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            return Database.Page<ProductVariantDto>(page, itemsPerPage, sql);
        } 

        #region Overrides of RepositoryBase<IProduct>


        protected override IProduct PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var inventoryCollection =((ProductVariantRepository) _productVariantRepository).GetCategoryInventoryCollection(dto.ProductVariantDto.Key);
            var productAttributeCollection = ((ProductVariantRepository) _productVariantRepository).GetProductAttributeCollection(dto.ProductVariantDto.Key);

            var factory = new ProductFactory(productAttributeCollection, inventoryCollection, GetProductOptionCollection(dto.Key), GetProductVariantCollection(dto.Key));
            var product = factory.BuildEntity(dto);


            product.ResetDirtyProperties();

            return product;
        }

        protected override IEnumerable<IProduct> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var id in keys)
                {
                    yield return Get(id);
                }
            }
            else
            {
                //var factory = new ProductFactory();
                var dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false));
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
               .InnerJoin<ProductVariantIndexDto>()
               .On<ProductVariantDto, ProductVariantIndexDto>(left => left.Key, right => right.ProductVariantKey)
               .Where<ProductVariantDto>(x => x.Master);
            
            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchProduct.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {                    
                    @"DELETE FROM merchProductVariant2ProductAttribute WHERE optionKey IN 
                        (SELECT optionKey FROM merchProductOption WHERE pk IN 
                        (SELECT optionKey FROM merchProduct2ProductOption WHERE productKey = @Key))",                    
                    @"DELETE FROM merchProductAttribute WHERE optionKey IN 
                        (SELECT optionKey FROM merchProductOption WHERE pk IN 
                        (SELECT optionKey FROM merchProduct2ProductOption WHERE productKey = @Key))",
                    "DELETE FROM merchProduct2ProductOption WHERE productKey = @Key",
                    "DELETE FROM merchCatalogInventory WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Key)",
                    "DELETE FROM merchProductVariantIndex WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Key)",
                    "DELETE FROM merchProductVariant WHERE productKey = @Key",
                    "DELETE FROM merchProduct WHERE pk = @Key",
                    "DELETE FROM merchProductOption WHERE pk NOT IN (SELECT optionKey FROM merchProduct2ProductOption)"
                };

            return list;
        }

        protected override void PersistNewItem(IProduct entity)
        {
            Mandate.ParameterCondition(SkuExists(entity.Sku) == false, "Skus must be unique.");

            ((Product)entity).AddingEntity();
            ((ProductVariant)((Product)entity).MasterVariant).VersionKey = Guid.NewGuid();

            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            // save the product
            Database.Insert(dto);
            entity.Key = dto.Key;

            // setup and save the master (singular) variant
            dto.ProductVariantDto.ProductKey = dto.Key;
            Database.Insert(dto.ProductVariantDto);
            Database.Insert(dto.ProductVariantDto.ProductVariantIndexDto);

            ((Product) entity).MasterVariant.ProductKey = dto.ProductVariantDto.ProductKey;
            ((Product) entity).MasterVariant.Key = dto.ProductVariantDto.Key;
            ((ProductVariant) ((Product)entity).MasterVariant).ExamineId = dto.ProductVariantDto.ProductVariantIndexDto.Id;

            // save the product options
            SaveProductOptions(entity);

            // synchronize the inventory
            ((ProductVariantRepository)_productVariantRepository).SaveCatalogInventory(((Product)entity).MasterVariant);
            
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IProduct entity)
        {
            ((Product)entity).UpdatingEntity();
            ((ProductVariant) ((Product) entity).MasterVariant).VersionKey = Guid.NewGuid();

            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            Database.Update(dto.ProductVariantDto);
            
            SaveProductOptions(entity);

            // synchronize the inventory
            ((ProductVariantRepository) _productVariantRepository).SaveCatalogInventory(((Product)entity).MasterVariant);

            entity.ResetDirtyProperties();         
        }

        protected override void PersistDeletedItem(IProduct entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        protected override IEnumerable<IProduct> PerformGetByQuery(IQuery<IProduct> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProduct>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(sql);

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
               .On<ProductOptionDto, Product2ProductOptionDto>(left => left.Key, right => right.OptionKey)
               .Where<Product2ProductOptionDto>(x => x.ProductKey == productKey)
               .OrderBy<Product2ProductOptionDto>(x => x.SortOrder);

            var dtos = Database.Fetch<ProductOptionDto, Product2ProductOptionDto>(sql);

            var productOptions = new ProductOptionCollection();
            var factory = new ProductOptionFactory();
            foreach (var option in dtos.Select(factory.BuildEntity))
            {
                var attributes = GetProductAttributeCollection(option.Key);
                option.Choices = attributes;
                productOptions.Insert(0, option);
            }

            return productOptions;
        }

        private ProductVariantCollection GetProductVariantCollection(Guid productKey)
        {
            var collection = new ProductVariantCollection();
            var query = Querying.Query<IProductVariant>.Builder.Where(x => x.ProductKey == productKey && ((ProductVariant)x).Master == false);
            var variants = _productVariantRepository.GetByQuery(query);
            foreach (var variant in variants)
            {
                if(variant != null) // todo why is this need?
                collection.Add(variant);
            }
            return collection;
        }

        private void DeleteProductOption(IProductOption option)
        {
            var executeClauses = new[]
                {
                    "DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey IN (SELECT productVariantKey FROM merchProductVariant2ProductAttribute WHERE optionKey = @Key)",
                    "DELETE FROM merchProduct2ProductOption WHERE optionKey = @Key",
                    "DELETE FROM merchProductAttribute WHERE optionKey = @Key",
                    "DELETE FROM merchProductOption WHERE pk = @Key"
                };

            foreach (var clause in executeClauses)
            {
                Database.Execute(clause, new { Key = option.Key });
            }
        }

        private void SaveProductOptions(IProduct product)
        {            
            var existing = GetProductOptionCollection(product.Key);
            if (!product.DefinesOptions && !existing.Any()) return;

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
                ((Entity)productOption).AddingEntity();
                var dto = factory.BuildDto(productOption);

                Database.Insert(dto);
                productOption.Key = dto.Key;

                // associate the product with the product option
                var association = new Product2ProductOptionDto()
                {
                    ProductKey = product.Key,
                    OptionKey = productOption.Key,
                    SortOrder = productOption.SortOrder,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                Database.Insert(association);

            }
            else
            {
                ((Entity)productOption).UpdatingEntity();
                var dto = factory.BuildDto(productOption);
                Database.Update(dto);

                // TODO : this should be refactored
                const string update = "UPDATE merchProduct2ProductOption SET SortOrder = @So, updateDate = @Ud WHERE productKey = @pk AND optionKey = @OKey";

                Database.Execute(update,
                                 new
                                 {
                                     So = productOption.SortOrder,
                                     Ud = productOption.UpdateDate,
                                     pk = product.Key,
                                     OKey = productOption.Key
                                 });


            }

            // now save the product attributes
            SaveProductAttributes(product, productOption);            
        }

        private ProductAttributeCollection GetProductAttributeCollection(Guid optionKey)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductAttributeDto>()
               .Where<ProductAttributeDto>(x => x.OptionKey == optionKey);

            var dtos = Database.Fetch<ProductAttributeDto>(sql);

            var attributes = new ProductAttributeCollection();
            var factory = new ProductAttributeFactory();

            foreach (var dto in dtos)
            {
                var attribute = factory.BuildEntity(dto);
                attributes.Add(attribute);
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

            Database.Execute("DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey IN (SELECT productVariantKey FROM merchProductVariant2ProductAttribute WHERE productAttributeKey = @Key)", 
                new { Key = productAttribute.Key});
            Database.Execute("DELETE FROM merchProductAttribute WHERE pk = @Key", new { Key = productAttribute.Key });
            
        }

        private void SaveProductAttributes(IProduct product, IProductOption productOption)
        {
            if (!productOption.Choices.Any()) return;

            var existing = GetProductAttributeCollection(productOption.Key);

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
                att.OptionKey = productOption.Key;
                SaveProductAttribute(att);
            }
            // this is required due to the special case relation between a product and product variants
            foreach (var variant in product.ProductVariants)
            {
                RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProductVariant>(variant.Key));
            }
        }

        private void SaveProductAttribute(IProductAttribute productAttribute)
        {
            var factory = new ProductAttributeFactory();

            if (!productAttribute.HasIdentity)
            {
                //((Entity)productAttribute).AddingEntity();
                productAttribute.CreateDate = DateTime.Now;
                productAttribute.UpdateDate = DateTime.Now;

                var dto = factory.BuildDto(productAttribute);
                Database.Insert(dto);
                productAttribute.Key = dto.Key;
            }
            else
            {
                ((Entity)productAttribute).UpdatingEntity();
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
