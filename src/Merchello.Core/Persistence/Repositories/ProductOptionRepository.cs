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

        public IEnumerable<IProductOption> GetByProductKey(Guid productKey)
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

            return dtos.Any()
                       ? dtos.Select(
                           x =>
                               {
                                   var option = factory.BuildEntity(x);
                                   option.Choices = GetProductAttributeCollection(option.Key, productKey);
                                   return option;
                               })
                       : Enumerable.Empty<IProductOption>();
        }

        public IEnumerable<IProductOption> SaveForProduct(IEnumerable<IProductOption> options, Guid productKey)
        {
            // Order the options by sort order
            var savers = options.OrderBy(x => x.SortOrder);
            
            // Find the product options to find any that need to be removed 
            var existing = GetByProductKey(productKey).ToArray();

            if (existing.Any())
            {
                // Remove any options that previously existed in the product option collection that are not present in the new collection
                SafeRemoveOldOptions(savers, existing, productKey);
            }

            return savers;
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
            var sql = BuildSearchSql(term, sharedOnly);

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
        protected override void PersistUpdatedItem(IProductOption entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new ProductOptionFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            SaveProductAttributes(entity);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Deletes a ProductOption.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IProductOption entity)
        {
            if (EnsureSharedDelete(entity))
            base.PersistDeletedItem(entity);
        }


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
        /// Gets the product attribute collection.
        /// </summary>
        /// <param name="optionKey">
        /// The option key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeCollection"/>.
        /// </returns>
        private ProductAttributeCollection GetProductAttributeCollection(Guid optionKey)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductAttributeDto>(SqlSyntax)
               .Where<ProductAttributeDto>(x => x.OptionKey == optionKey);

            return GetProductAttributeCollection(sql);
        }

        private ProductAttributeCollection GetProductAttributeCollection(Guid optionKey, Guid productKey)
        {
            var sql = new Sql();
            sql.Select("*")
                .From<ProductAttributeDto>(SqlSyntax)
                .Append("WHERE [merchProductAttribute].[pk] IN (")
                .Append("SELECT DISTINCT(productAttributeKey)")
                .Append("FROM [merchProductVariant2ProductAttribute]")
                .Append("WHERE optionKey = @okey", new { @okey = optionKey })
                .Append("AND productVariantKey IN (")
                .Append("SELECT [merchProductVariant].[pk] IN (")
                .Append("FROM [merchProductVariant]")
                .Append("WHERE productKey = @pvk", new { @pvk = productKey })
                .Append("AND [master] = @m", new { @m = false })
                .Append(")) ORDER BY sortOrder");

            return GetProductAttributeCollection(sql);

        }

        private ProductAttributeCollection GetProductAttributeCollection(Sql sql)
        {
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


        private void DeleteSharedProductAttributeVariantAssociation(IProductOption option, Guid productKey)
        {
            var keys = option.Choices.Select(x => x.Key).ToArray();

            var sql = new Sql();
            sql.Append("DELETE FROM merchProductVariant2ProductAttribute")
                .Append("WHERE productVariantKey IN (")
                .Append("SELECT productVariantKey FROM  merchProductVariant2ProductAttribute T1")
                .Append("JOIN  merchProductVariant T2 ON T1.productVariantKey = T2.pk")
                .Append("WHERE T2.productKey = @pk", new { @pk = productKey })
                .Append(")")
                .Append("AND optionKey IN (@oks)", new { @oks = keys });

            Database.Execute(sql);

            foreach (var choice in option.Choices.Where(choice => choice.UseCount >= 0))
            {
                choice.UseCount--;
            }
        }

        private void SafeRemoveOldOptions(IEnumerable<IProductOption> savers, IEnumerable<IProductOption> existing, Guid productKey)
        {

            var removers = existing.Where(ex => savers.All(sv => sv.Key != ex.Key)).ToArray();
            if (removers.Any())
            {
                foreach (var rm in removers)
                {
                    if (rm.Shared)
                    {
                        //// shared options cannot be deleted from a product.  Instead useCount will be decremented
                        rm.SharedCount--;
                        DeleteSharedProductAttributeVariantAssociation(rm, productKey);
                        AddOrUpdate(rm);
                    }
                    else
                    {
                        Delete(rm);
                    }
                }
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

            var existing = GetProductAttributeCollection(option.Key);

            //// Ensure all ids are in the new list
            var resetSorts = false;
            foreach (var ex in existing)
            { 
                if (option.Choices.Contains(ex.Key)) continue;
                DeleteProductAttribute(ex);
                resetSorts = true;
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


            //// TODO RSS need to raise an event here to clear caches and reindex products
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
                att.UseCount = 0;
                att.CreateDate = DateTime.Now;
                att.UpdateDate = DateTime.Now;

                att.SortOrder = option.Choices.Max(x => x.SortOrder) + 1;
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
        /// The ensure shared delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool EnsureSharedDelete(IProductOption entity)
        {
            return !entity.Shared || 
                (entity.Shared && entity.Choices.Sum(x => x.UseCount) == 0);
        }


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
        private Sql BuildSearchSql(string searchTerm, bool sharedOnly)
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
    }
}