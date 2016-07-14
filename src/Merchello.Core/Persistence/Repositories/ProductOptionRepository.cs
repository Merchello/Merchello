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
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// A repository responsible for persisting <see cref="IProductOption"/>.
    /// </summary>
    /// <remarks>
    /// We have to be careful with the runtime cache here since various usages of the product option will be used by different products.
    /// We will need to make sure when we filter the choices, the object is previously cloned into a new option.
    /// </remarks>
    internal class ProductOptionRepository : MerchelloPetaPocoRepositoryBase<IProductOption>, IProductOptionRepository
    {
        /// <summary>
        /// The detached content type repository.
        /// </summary>
        private readonly IDetachedContentTypeRepository _detachedContentTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public ProductOptionRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
            _detachedContentTypeRepository = new DetachedContentTypeRepository(work, cache, logger, sqlSyntax);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        /// <param name="detachedContentTypeRepository">
        /// The detached content type repository.
        /// </param>
        public ProductOptionRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILogger logger, ISqlSyntaxProvider sqlSyntax, IDetachedContentTypeRepository detachedContentTypeRepository)
            : base(work, cache, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(detachedContentTypeRepository, "detachedContentTypeRepository");
            _detachedContentTypeRepository = detachedContentTypeRepository;
        }


        /// <summary>
        /// Saves options associated with a product.
        /// </summary>
        /// <param name="product">
        /// The product with options to be saved.
        /// </param>
        /// <remarks>
        /// Note:  'shared' product options associated with a product may not have the entire collection of ProductAttributes (choices)
        /// so the actual work is done on the shared option which is then filtered again and replaced in the product 
        /// </remarks>
        /// <returns>
        /// A collection of product keys of products that need to be refreshed in the current cache and examine.
        /// </returns>
        public IEnumerable<Guid> SaveForProduct(IProduct product)
        {
            if (!product.ProductOptions.Any()) return Enumerable.Empty<Guid>();

            // Ensures the sort order with respect to this product
            EnsureProductOptionsSortOrder(product.ProductOptions);

            // Reset the Product Options Collection so that updated values are ordered and cached correctly
            product.ProductOptions = SaveForProduct(product.ProductOptions.AsEnumerable(), product.Key);

            return product.ProductOptions.Where(x => x.Shared).Select(x => x.Key);
        }

        /// <summary>
        /// Creates the attribute association between product attribute and product variant.
        /// </summary>
        /// <param name="variant">
        /// The variant.
        /// </param>
        /// <returns>
        /// A collection of product keys of products that need to be refreshed in the current cache and examine.
        /// </returns>
        public IEnumerable<Guid> CreateAttributeAssociationForProductVariant(IProductVariant variant)
        {
            // insert associations for every attribute
            foreach (var association in variant.Attributes.Select(att => new ProductVariant2ProductAttributeDto()
            {
                ProductVariantKey = variant.Key,
                OptionKey = att.OptionKey,
                ProductAttributeKey = att.Key,
                UpdateDate = DateTime.Now,
                CreateDate = DateTime.Now
            }))
            {
                Database.Insert(association);
            }

            Database.Execute("UPDATE merchProductAttribute SET useCount = useCount + 1 WHERE pk IN (@keys)", new { @keys = variant.Attributes.Select(x => x.Key).ToArray() });

            var sharedOptions = GetProductOptions(variant.Attributes.Select(x => x.OptionKey).ToArray(), true);

            return GetProductKeysForCacheRefresh(sharedOptions.Select(x => x.Key).ToArray());
        }

        /// <summary>
        /// Queries for product options by a collection of keys.
        /// </summary>
        /// <param name="optionKeys">
        /// The option Keys.
        /// </param>
        /// <param name="sharedOnly">
        /// The shared Only.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        public IEnumerable<IProductOption> GetProductOptions(Guid[] optionKeys, bool sharedOnly = false)
        {
            if (!optionKeys.Any()) return Enumerable.Empty<IProductOption>();

            var sql = GetBaseQuery(false).Where("merchProductOption.pk IN (@keys)", new { @keys = optionKeys });

            if (sharedOnly) sql.Where("shared = @shared", new { @shared = true });

            var dtos = Database.Fetch<ProductOptionDto>(sql);

            var factory = new ProductOptionFactory();

            return dtos.Select(dto => factory.BuildEntity(dto));
        }

        /// <summary>
        /// Deletes all products options.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <remarks>
        /// Used when deleting a product
        /// </remarks>
        /// <returns>
        /// A collection of product keys of products that need to be refreshed in the current cache and examine.
        /// </returns>
        public IEnumerable<Guid> DeleteAllProductOptions(IProduct product)
        {
            var sharedOptionKeys = product.ProductOptions.Where(x => x.Shared).Select(x => x.Key).ToArray();

            var statements = GetRemoveAllProductOptionsFromProductSql(product, sharedOptionKeys);

            foreach (var sql in statements)
            {
                Database.Execute(sql);
            }


            return sharedOptionKeys;
        }

        /// <summary>
        /// Deletes all product attributes from a product variant.
        /// </summary>
        /// <param name="variant">
        /// The variant.
        /// </param>
        /// <remarks>
        /// Used when deleting a product variant
        /// </remarks>
        /// <returns>
        /// A collection of product keys of products that need to be refreshed in the current cache and examine.
        /// </returns>
        public IEnumerable<Guid> DeleteAllProductVariantAttributes(IProductVariant variant)
        {
            //// This needs to delete all attributes from the merchProductVariant2ProductAttribute table.

            var sharedOptions = GetProductOptions(variant.Attributes.Select(x => x.OptionKey).Distinct().ToArray(), true);

            var attributeKeys = variant.Attributes.Select(x => x.Key);

            Database.Execute("UPDATE merchProductAttribute SET useCount = useCount - 1 WHERE useCount > 0 AND pk IN (@keys)", new { @keys = attributeKeys });
            Database.Execute("DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey = @key", new { @key = variant.Key });

            return GetProductKeysForCacheRefresh(sharedOptions.Select(x => x.Key).ToArray());
        }

        /// <summary>
        /// Gets the shareCount for a <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="optionKey">
        /// The option key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetProductOptionShareCount(Guid optionKey)
        {
            var sql = new Sql();
            sql.Select("COUNT(*)")
                .From<Product2ProductOptionDto>(SqlSyntax)
                .Where<Product2ProductOptionDto>(x => x.OptionKey == optionKey);

            return Database.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Gets the product attribute use count.
        /// </summary>
        /// <param name="attributeKey">
        /// The attribute key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetProductAttributeUseCount(Guid attributeKey)
        {
            var sql = new Sql();
            sql.Select("COUNT(*)")
                .From<ProductVariant2ProductAttributeDto>(SqlSyntax)
                .Where<ProductVariant2ProductAttributeDto>(x => x.ProductAttributeKey == attributeKey);


            return Database.ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Gets the <see cref="ProductOptionCollection"/> for a given product key.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionCollection"/>.
        /// </returns>
        /// <remarks>
        /// The method manages the sort order of the options with respect to the product
        /// This query is never cached and is intended to generate objects that will be cached in 
        /// individual product collections
        /// </remarks>
        public ProductOptionCollection GetProductOptionCollection(Guid productKey)
        {
            var options = GetByProductKey(productKey);

            // Build the collection
            var collection = new ProductOptionCollection();

            foreach (var o in options)
            {
                collection.Add(o);
            }

            return collection;
        }


        /// <summary>
        /// Gets a <see cref="ProductAttributeCollection"/> for a product variant.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeCollection"/>.
        /// </returns>
        public ProductAttributeCollection GetProductAttributeCollectionForVariant(Guid productVariantKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductVariant2ProductAttributeDto>(SqlSyntax)
                .InnerJoin<ProductAttributeDto>(SqlSyntax)
                .On<ProductVariant2ProductAttributeDto, ProductAttributeDto>(SqlSyntax, left => left.ProductAttributeKey, right => right.Key)
                .Where<ProductVariant2ProductAttributeDto>(x => x.ProductVariantKey == productVariantKey);

            var dtos = Database.Fetch<ProductVariant2ProductAttributeDto, ProductAttributeDto>(sql);

            var factory = new ProductAttributeFactory();
            var collection = new ProductAttributeCollection();
            foreach (var dto in dtos)
            {
                collection.Add(factory.BuildEntity(dto.ProductAttributeDto));
            }

            return collection;
        }

        /// <summary>
        /// Gets the product attribute collection.
        /// </summary>
        /// <param name="optionKey">
        /// The option key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeCollection"/>.
        /// </returns>
        public ProductAttributeCollection GetProductAttributeCollection(Guid optionKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductAttributeDto>(SqlSyntax)
                .Where<ProductAttributeDto>(x => x.OptionKey == optionKey)
                .OrderBy<ProductAttributeDto>(x => x.SortOrder, SqlSyntax);

            return GetProductAttributeCollection(sql);
        }


        /// <summary>
        /// Gets a page of <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="term">
        /// A search term to filter by
        /// </param>
        /// <param name="page">
        /// The page requested.
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <param name="sharedOnly">
        /// Indicates whether or not to only include shared option.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProductOption}"/>.
        /// </returns>
        public Page<IProductOption> GetPage(
            string term,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Descending,
            bool sharedOnly = true)
        {
            var sql = this.GetSearchSql(term, sharedOnly);

            if (!string.IsNullOrEmpty(sortBy))
            {
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", sortBy)
                    : string.Format("ORDER BY {0} DESC", sortBy));
            }


            var p = Database.Page<ProductOptionDto>(page, itemsPerPage, sql);

            var keys = p.Items.Select(x => x.Key).ToArray();

            return new Page<IProductOption>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = keys.Any() ? GetAll(keys).ToList() : Enumerable.Empty<IProductOption>().ToList()
            };
        }


        /// <summary>
        /// Saves a collection of product options for a given product.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The saved collection <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        internal ProductOptionCollection SaveForProduct(IEnumerable<IProductOption> options, Guid productKey)
        {
            // Order the options by sort order
            var savers = options.OrderBy(x => x.SortOrder);

            // Find the product options to find any that need to be removed 
            var existing = GetByProductKey(productKey).ToArray();

            if (existing.Any())
            {
                // Remove any options that previously existed in the product option collection that are not present in the new collection
                this.SafeRemoveSharedOptionsFromProduct(savers, existing, productKey);
            }

            foreach (var o in savers)
            {
                this.SafeAddOrUpdateProductWithProductOption(o, existing.Any(x => x.Key == o.Key), productKey);
            }

            return GetProductOptionCollection(productKey);
        }

        #region Base overrides

        /// <summary>
        /// Gets a <see cref="IProductOption"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductOption"/>.
        /// </returns>
        protected override IProductOption PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ProductOptionDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ProductOptionFactory();

            var option = factory.BuildEntity(dto);
            option.Choices = GetProductAttributeCollection(option.Key);

            return option;
        }

        /// <summary>
        /// Performs a get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        protected override IEnumerable<IProductOption> PerformGetAll(params Guid[] keys)
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
                var dtos = Database.Fetch<ProductOptionDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IProductOption"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        protected override IEnumerable<IProductOption> PerformGetByQuery(IQuery<IProductOption> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProductOption>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductOptionDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base SQL query.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<ProductOptionDto>(SqlSyntax);

            return sql;
        }

        /// <summary>
        /// Gets the default SQL Where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchProductOption.pk = @Key";
        }

        /// <summary>
        /// Gets the delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey IN (SELECT productVariantKey FROM merchProductVariant2ProductAttribute WHERE optionKey = @Key)",
                    "DELETE FROM merchProduct2ProductOption WHERE optionKey = @Key",
                    "DELETE FROM merchProductAttribute WHERE optionKey = @Key",
                    "DELETE FROM merchProductOption WHERE pk = @Key"
                };

            return list;
        }

        /// <summary>
        /// Saves a new <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <remarks>
        /// This should never be a partial variation of an option 
        /// (e.g. NEVER save a non shared option that has been queried with a reduced collection of choices) 
        /// </remarks>
        protected override void PersistNewItem(IProductOption entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new ProductOptionFactory();
            var dto = factory.BuildDto(entity);
            Database.Insert(dto);
            entity.Key = dto.Key;
            
            SaveProductAttributes(entity);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <remarks>
        /// This should never be a partial variation of an option 
        /// (e.g. NEVER save a non shared option that has been queried with a reduced collection of choices) 
        /// </remarks>
        protected override void PersistUpdatedItem(IProductOption entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ProductOptionFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            SaveProductAttributes(entity);

            entity.ResetDirtyProperties();
        }


        #endregion

        /// <summary>
        /// Ensures duplicate SKUs do not exist.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="att">
        /// The attribute.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        private static void EnsureAttributeSku(IProductOption option, IProductAttribute att, int index = 1)
        {
            if (option.Choices.Count(x => x.Sku == att.Sku) == 1) return;

            if (option.Choices.All(x => x.Sku != string.Concat(att.Sku, index)))
            {
                att.Sku = string.Concat(att.Sku, index);
                return;
            }

            index++;

            EnsureAttributeSku(option, att, index);
        }



        /// <summary>
        /// Gets the <see cref="ProductAttributeCollection"/> for a specific product.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeCollection"/>.
        /// </returns>
        private ProductAttributeCollection GetProductAttributeCollection(IProductOption option, Guid productKey)
        {
            var sql = new Sql();

            if (option.Shared)
            {
                sql.Select("*")
                    .From<ProductAttributeDto>(SqlSyntax)
                    .InnerJoin<ProductOptionAttributeShareDto>(SqlSyntax)
                    .On<ProductAttributeDto, ProductOptionAttributeShareDto>(
                        SqlSyntax,
                        left => left.Key,
                        right => right.AttributeKey)
                    .Where<ProductOptionAttributeShareDto>(x => x.ProductKey == productKey && x.OptionKey == option.Key)
                    .OrderBy<ProductAttributeDto>(x => x.SortOrder, SqlSyntax);
            }
            else
            {
                sql.Select("*")
                    .From<ProductAttributeDto>(SqlSyntax)
                    .Where<ProductAttributeDto>(x => x.OptionKey == option.Key)
                    .OrderBy<ProductAttributeDto>(x => x.SortOrder, SqlSyntax);
            }

            return GetProductAttributeCollection(sql);
        }

        /// <summary>
        /// Gets the the <see cref="ProductAttributeCollection"/> by SQL.
        /// </summary>
        /// <param name="sql">
        /// The SQL.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeCollection"/>.
        /// </returns>
        private ProductAttributeCollection GetProductAttributeCollection(Sql sql)
        {
            var dtos = Database.Fetch<ProductAttributeDto>(sql);

            var attributes = new ProductAttributeCollection();
            var factory = new ProductAttributeFactory();

            foreach (var dto in dtos.OrderBy(x => x.SortOrder))
            {
                var attribute = factory.BuildEntity(dto);
                attributes.Add(attribute);
            }

            return attributes;
        }

        /// <summary>
        /// Gets a collection of .
        /// </summary>
        /// <param name="sharedOptionKeys">
        /// The shared option keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Guid}"/>.
        /// </returns>
        private IEnumerable<Guid> GetProductKeysForCacheRefresh(Guid[] sharedOptionKeys)
        {
            var sql = new Sql().Select("*")
                .From<Product2ProductOptionDto>(SqlSyntax)
                .Where("optionKey IN (@keys)", new { @keys = sharedOptionKeys });

            var dtos = Database.Fetch<Product2ProductOptionDto>(sql);

            return dtos.Select(x => x.ProductKey);
        }

        /// <summary>
        /// Deletes a product attribute.
        /// </summary>
        /// <param name="productAttribute">
        /// The product attribute.
        /// </param>
        private void DeleteProductAttribute(IProductAttribute productAttribute)
        {
            //// We want ProductVariant events to trigger on a ProductVariant Delete
            //// and we need to delete all variants that had the attribute that is to be deleted so the current solution
            //// is to delete all associations from the merchProductVariant2ProductAttribute table so that the follow up
            //// EnsureProductVariantsHaveAttributes called in the ProductVariantService cleans up the orphaned variants and fires off
            //// the events

            Database.Execute(
                "DELETE FROM merchProductVariant2ProductAttribute WHERE productVariantKey IN (SELECT productVariantKey FROM merchProductVariant2ProductAttribute WHERE productAttributeKey = @Key)",
                new { Key = productAttribute.Key });

            Database.Execute("DELETE FROM merchProductAttribute WHERE pk = @Key", new { Key = productAttribute.Key });
        }

        private void SafeAddOrUpdateProductWithProductOption(IProductOption option, bool exists, Guid productKey)
        {
            var makeAssociation = false;

            if (!option.HasIdentity)
            {
                // this option is being added through the product UI
                option.Shared = false;
                option.SharedCount = 1;
                PersistNewItem(option);
                makeAssociation = true;
            }
            else
            {
                if (!exists)
                {
                    option.SharedCount++;
                    makeAssociation = true;
                }

                PersistUpdatedItem(option);
            }

            if (makeAssociation)
            {
                var dto = new Product2ProductOptionDto
                {
                    OptionKey = option.Key,
                    ProductKey = productKey,
                    SortOrder = option.SortOrder,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                Database.Insert(dto);

                if (option.Shared)
                {
                    foreach (var choice in option.Choices)
                    {
                        var mapDto = new ProductOptionAttributeShareDto
                                         {
                                             ProductKey = productKey,
                                             OptionKey = choice.OptionKey,
                                             AttributeKey = choice.Key,
                                             CreateDate = DateTime.Now,
                                             UpdateDate = DateTime.Now
                                         };
                        Database.Insert(dto);
                    }
                }
            }
        }

        /// <summary>
        /// Safely removes old shared options from a product.
        /// </summary>
        /// <param name="savers">
        /// The savers.
        /// </param>
        /// <param name="existing">
        /// The existing.
        /// </param>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        private void SafeRemoveSharedOptionsFromProduct(IEnumerable<IProductOption> savers, IEnumerable<IProductOption> existing, Guid productKey)
        {
            var removers = existing.Where(ex => savers.All(sv => sv.Key != ex.Key)).ToArray();
            if (removers.Any())
            {
                foreach (var rm in removers)
                {
                    if (rm.Shared)
                    {
                        //// shared options cannot be deleted from a product.  Instead useCount will be decremented
                        this.DeleteSharedProductOptionFromProduct(rm, productKey);
                    }
                    else
                    {
                        Delete(rm);
                    }
                }
            }
        }


        /// <summary>
        /// Removes attribute association from IProduct for a shared option.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <remarks>
        /// This affectively deletes the shared option from the product
        /// </remarks>
        private void DeleteSharedProductOptionFromProduct(IProductOption option, Guid productKey)
        {
            foreach (var clause in this.GetRemoveShareProductOptionFromProductSql(option, productKey))
            {
                Database.Execute(clause);
            }
        }

        /// <summary>
        /// Saves the attribute collection.
        /// </summary>
        /// <param name="option">
        /// The product option.
        /// </param>
        private void SaveProductAttributes(IProductOption option)
        {
            if (!option.Choices.Any()) return;

            // This asserts that all of the option choices are there when the option is shared
            var existing = GetProductAttributeCollection(option.Key);

            //// Ensure all ids are in the new list
            var resetSorts = false;
            foreach (var ex in existing)
            { 
                if (option.Choices.Contains(ex.Key)) continue;

                if (!option.Shared)
                {
                    DeleteProductAttribute(ex);
                    resetSorts = true;
                }
                else
                {
                    ex.UseCount--;
                }

            }

            if (resetSorts)
            {
                var count = 1;
                foreach (var att in option.Choices.OrderBy(x => x.SortOrder))
                {
                    att.SortOrder = count;
                    att.OptionKey = option.Key;
                    option.Choices.Add(att);
                    count = count + 1;
                }
            }

            foreach (var att in option.Choices)
            {
                SaveProductAttribute(option, att);
            }
        }

        /// <summary>
        /// Saves a product attribute.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="att">
        /// The product attribute.
        /// </param>
        private void SaveProductAttribute(IProductOption option, IProductAttribute att)
        {
            var factory = new ProductAttributeFactory();

            if (!att.HasIdentity)
            {
                att.CreateDate = DateTime.Now;
                att.UpdateDate = DateTime.Now;

                EnsureAttributeSku(option, att);

                // Ensure the option key
                att.OptionKey = option.Key;

                var dto = factory.BuildDto(att);
                Database.Insert(dto);
                att.Key = dto.Key;
            }
            else
            {
                ((Entity)att).UpdatingEntity();
                EnsureAttributeSku(option, att);
                var dto = factory.BuildDto(att);
                Database.Update(dto);
            }
        }

        /// <summary>
        /// Ensures the sort order of product options.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        private void EnsureProductOptionsSortOrder(ProductOptionCollection options)
        {
            if (!options.Any()) return;

            var sort = 1;
            var sorted = options.OrderBy(x => x.SortOrder);
            foreach (var option in sorted)
            {
                if (option.SortOrder != sort) option.SortOrder = sort;
                sort++;
            }
        }

        /// <summary>
        /// Gets a collection of options for a specific <see cref="IProduct"/>.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProduct}"/>.
        /// </returns>
        private IEnumerable<IProductOption> GetByProductKey(Guid productKey)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductOptionDto>(SqlSyntax)
               .InnerJoin<Product2ProductOptionDto>(SqlSyntax)
               .On<ProductOptionDto, Product2ProductOptionDto>(SqlSyntax, left => left.Key, right => right.OptionKey)
               .Where<Product2ProductOptionDto>(x => x.ProductKey == productKey)
               .OrderBy<Product2ProductOptionDto>(x => x.SortOrder, SqlSyntax);

            var dtos = Database.Fetch<ProductOptionDto, Product2ProductOptionDto>(sql);

            var factory = new ProductOptionFactory();

            if (!dtos.Any()) return Enumerable.Empty<IProductOption>();

            var options = new List<IProductOption>();

            foreach (var option in dtos.Select(dto => factory.BuildEntity(dto)))
            {
                option.Choices = this.GetProductAttributeCollection(option, productKey);
                options.Add(option);
            }

            return options;
        }

        #region Sql

        /// <summary>
        /// Builds the product search SQL.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="sharedOnly">
        /// The shared Only.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        private Sql GetSearchSql(string searchTerm, bool sharedOnly)
        {
            searchTerm = searchTerm.Replace(",", " ");
            var invidualTerms = searchTerm.Split(' ');

            var terms = invidualTerms.Where(x => !string.IsNullOrEmpty(x)).ToList();


            var sql = new Sql();
            sql.Select("*").From<ProductOptionDto>(SqlSyntax);

            if (terms.Any())
            {
                var preparedTerms = string.Format("%{0}%", string.Join("% ", terms)).Trim();

                sql.Where("name LIKE @name", new { @name = preparedTerms });
            }

            if (sharedOnly) sql.Where("shared = @shared", new { @shared = true });

            return sql;
        }

        /// <summary>
        /// Gets the SQL statements to execute when deleting options from a product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="sharedOptionKeys">
        /// The shared option keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Sql}"/>.
        /// </returns>
        private IEnumerable<Sql> GetRemoveAllProductOptionsFromProductSql(IProduct product, Guid[] sharedOptionKeys)
        {
            var list = new List<Sql>
                {
                    new Sql().Append("DELETE FROM merchProduct2ProductOption WHERE productKey = @key", new { @key = product.Key })  
                };

            if (sharedOptionKeys.Any()) 
                 list.Add(new Sql().Append("UPDATE merchProductOption SET shareCount = shareCount - 1 WHERE pk IN (@keys)", new { @keys = sharedOptionKeys }));

            return list;
        }

        /// <summary>
        /// Gets a list of SQL clauses to be executed when removing shared options from a product.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Sql}"/>.
        /// </returns>
        private IEnumerable<Sql> GetRemoveShareProductOptionFromProductSql(IProductOption option, Guid productKey)
        {
            var keys = option.Choices.Select(x => x.Key).ToArray();

            var sortOrder = option.SortOrder;

            var list = new List<Sql>
            {
                //// Product Attribute Association
                new Sql().Append("DELETE FROM merchProductVariant2ProductAttribute")
                    .Append("WHERE productVariantKey IN (")
                    .Append("SELECT productVariantKey FROM  merchProductVariant2ProductAttribute T1")
                    .Append("JOIN  merchProductVariant T2 ON T1.productVariantKey = T2.pk")
                    .Append("WHERE T2.productKey = @pk", new { @pk = productKey })
                    .Append(")")
                    .Append("AND optionKey IN (@oks)", new { @oks = keys }),

                // Delete the shared attribute association
                new Sql().Append("DELETE FROM merchProductOption2ProductAttributeShare WHERE optionKey = @ok A"),

                //// Product Option Association
                new Sql().Append("DELETE FROM merchProduct2ProductOption WHERE optionKey = @ok AND productKey = @pk", new { @ok = option.Key, @pk = productKey }),

                //// Update SortOrder
                new Sql().Append("UPDATE merchProduct2ProductOption SET sortOrder = sortOrder -1 WHERE sortOrder > @so", new { @so = sortOrder }),

                //// Update the ProductOption shareCount
                new Sql().Append("UPDATE merchProductOption SET shareCount = shareCount - 1 WHERE shareCount > 0 AND pk = @key", new { @key = option.Key })
            };

            // Update the product attribute use counts
            list.AddRange(option.Choices.Select(choice => new Sql().Append("UPDATE merchProductAttribute SET useCount = useCount - 1 WHERE useCount > 0 AND pk = @key", new { @key = choice.Key })));

            return list;
        }

        #endregion
    }
}