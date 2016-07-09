namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;
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
    /// The product repository.
    /// </summary>
    internal class ProductRepository : PagedRepositoryBase<IProduct, ProductDto>, IProductRepository
    {
        /// <summary>
        /// The product variant repository.
        /// </summary>
        private readonly IProductVariantRepository _productVariantRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="productVariantRepository">
        /// The product variant repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        public ProductRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, IProductVariantRepository productVariantRepository, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {
           Mandate.ParameterNotNull(productVariantRepository, "productVariantRepository");
           _productVariantRepository = productVariantRepository;        
        }

        /// <summary>
        /// The get page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        /// <remarks>
        //// TODO this is a total hack and needs to be thought through a bit better.  IQuery is a worthless parameter here
        /// </remarks>
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

        /// <summary>
        /// The get paged keys.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="query">
        /// The query.
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
            var sql = this.BuildProductSearchSql(searchTerm);

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get by detached content type.
        /// </summary>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProduct}"/>.
        /// </returns>
        public IEnumerable<IProduct> GetByDetachedContentType(Guid detachedContentTypeKey)
        {
            var sql = new Sql();
            sql.Append("SELECT DISTINCT([merchProductVariant].[productKey])")
                .Append("FROM [merchProductVariant]")
                .Append("WHERE [merchProductVariant].[pk] IN (")
                .Append("SELECT DISTINCT([merchProductVariantDetachedContent].[productVariantKey])")
                .Append("FROM [merchProductVariantDetachedContent]")
                .Append(
                    "WHERE [merchProductVariantDetachedContent].[detachedContentTypeKey] = @Key",
                    new { @Key = detachedContentTypeKey })
                .Append(")");

            var productKeys = Database.Fetch<Guid>(sql);

            return productKeys.Select(Get);
        }

        /// <summary>
        /// The get key for slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetKeyForSlug(string slug)
        {
            var sql = new Sql();
            sql.Append("SELECT [merchProductVariant].[productKey]")
                .Append("FROM [merchProductVariant]")
                .Append(
                    "JOIN [merchProductVariantDetachedContent] ON [merchProductVariant].[pk] = [merchProductVariantDetachedContent].[productVariantKey]")
                .Append("WHERE [merchProductVariantDetachedContent].[slug] = @Sl", new { @Sl = slug });

            return Database.Fetch<Guid>(sql).FirstOrDefault();
        }

        /// <summary>
        /// True/false indicating whether or not a SKU is already exists in the database
        /// </summary>
        /// <param name="sku">
        /// The SKU to be tested
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> indicating whether or not the SKU exists.
        /// </returns>
        public bool SkuExists(string sku)
        {
            return _productVariantRepository.SkuExists(sku);
        }

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
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
        public Page<Guid> GetProductsKeysWithOption(
            string optionName,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetProductsKeysWithOption(new[] { optionName }, page, itemsPerPage, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceName">
        /// The choice name.
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
        public Page<Guid> GetProductsKeysWithOption(
            string optionName,
            string choiceName,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchProductVariant]")
                .Append("WHERE [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM (")
                .Append("SELECT	[merchProductVariant].[productKey]")
                .Append("FROM [merchProductVariant]")
                .Append("INNER JOIN [merchProductVariant2ProductAttribute]")
                .Append("ON	[merchProductVariant].[pk] = [merchProductVariant2ProductAttribute].[productVariantKey]")
                .Append("INNER JOIN [merchProductOption]")
                .Append("ON [merchProductVariant2ProductAttribute].[optionKey] = [merchProductOption].[pk]")
                .Append("INNER JOIN [merchProductAttribute]")
                .Append("ON [merchProductVariant2ProductAttribute].[productAttributeKey] = [merchProductAttribute].[pk]")
                .Append("WHERE [merchProductOption].[name] = @name", new { @name = optionName })
                .Append("AND")
                .Append("[merchProductAttribute].[name] = @name", new { @name = choiceName })
                .Append(") [merchProductVariant]")
                .Append(")")
                .Append("AND [merchProductVariant].[master] = 1");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionNames">
        /// The option names.
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
        /// The <see cref="Page{GUID}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysWithOption(
            IEnumerable<string> optionNames,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchProductVariant]")
                .Append("WHERE [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM (")
                .Append("SELECT	[merchProductVariant].[productKey]")
                .Append("FROM [merchProductVariant]")
                .Append("INNER JOIN [merchProductVariant2ProductAttribute]")
                .Append("ON	[merchProductVariant].[pk] = [merchProductVariant2ProductAttribute].[productVariantKey]")
                .Append("INNER JOIN [merchProductOption]")
                .Append("ON [merchProductVariant2ProductAttribute].[optionKey] = [merchProductOption].[pk]")
                .Append("WHERE [merchProductOption].[name] IN (@names)", new { @names = optionNames })
                .Append(") [merchProductVariant]")
                .Append(")")
                .Append("AND [merchProductVariant].[master] = 1");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys with option an option with specific choices
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceNames">
        /// The choice names.
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
        /// The <see cref="Page{GUID}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysWithOption(
            string optionName,
            IEnumerable<string> choiceNames,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
                .Append("FROM [merchProductVariant]")
                .Append("WHERE [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM (")
                .Append("SELECT	[merchProductVariant].[productKey]")
                .Append("FROM [merchProductVariant]")
                .Append("INNER JOIN [merchProductVariant2ProductAttribute]")
                .Append("ON	[merchProductVariant].[pk] = [merchProductVariant2ProductAttribute].[productVariantKey]")
                .Append("INNER JOIN [merchProductOption]")
                .Append("ON [merchProductVariant2ProductAttribute].[optionKey] = [merchProductOption].[pk]")
                .Append("INNER JOIN [merchProductAttribute]")
                .Append("ON [merchProductVariant2ProductAttribute].[productAttributeKey] = [merchProductAttribute].[pk]")
                .Append("WHERE [merchProductOption].[name] = @name", new { @name = optionName })
                .Append("AND")
                .Append("[merchProductAttribute].[name] IN (@names)", new { @names = choiceNames })
                .Append(") [merchProductVariant]")
                .Append(")")
                .Append("AND [merchProductVariant].[master] = 1");
                        
            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys in price range.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
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
        public Page<Guid> GetProductsKeysInPriceRange(
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetProductsKeysInPriceRange(min, max, 0, page, itemsPerPage, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys in price range.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="taxModifier">
        /// The tax modifier.
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
        public Page<Guid> GetProductsKeysInPriceRange(
            decimal min,
            decimal max,
            decimal taxModifier,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var modifier = taxModifier;
            if (modifier > 0) modifier = taxModifier / 100;

            modifier += 1;

            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
              .Append("WHERE [merchProductVariant].[productKey] IN (")
              .Append("SELECT DISTINCT([productKey])")
              .Append("FROM [merchProductVariant]")
              .Append("WHERE ([merchProductVariant].[onSale] = 0 AND [merchProductVariant].[price] BETWEEN @low AND @high)", new { @low = min * modifier, @high = max * modifier })
              .Append("OR")
              .Append("([merchProductVariant].[onSale] = 1 AND [merchProductVariant].[salePrice] BETWEEN @low AND @high)", new { @low = min * modifier, @high = max * modifier })
              .Append(")")
              .Append("AND [merchProductVariant].[master] = 1");
            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys by manufacturer.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
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
        public Page<Guid> GetProductsKeysByManufacturer(
            string manufacturer,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetProductsKeysByManufacturer(
                new[] { manufacturer },
                page,
                itemsPerPage,
                orderExpression,
                sortDirection);
        }

        /// <summary>
        /// The get products keys by manufacturer.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
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
        public Page<Guid> GetProductsKeysByManufacturer(
            IEnumerable<string> manufacturer,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
              .Append("WHERE [merchProductVariant].[manufacturer] IN (@manufacturers)", new { @manufacturers = manufacturer})
              .Append("AND [merchProductVariant].[master] = 1");
            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys by barcode.
        /// </summary>
        /// <param name="barcode">
        /// The barcode.
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
        public Page<Guid> GetProductsKeysByBarcode(
            string barcode,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetProductsKeysByBarcode(new[] { barcode }, page, itemsPerPage, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys by barcode.
        /// </summary>
        /// <param name="barcodes">
        /// The barcodes.
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
        public Page<Guid> GetProductsKeysByBarcode(
            IEnumerable<string> barcodes,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
              .Append("WHERE [merchProductVariant].[barcode] IN (@codes)", new { @codes = barcodes })
              .Append("AND [merchProductVariant].[master] = 1");
            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys in stock.
        /// </summary>
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
        /// <param name="includeAllowOutOfStockPurchase">
        /// The include allow out of stock purchase.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysInStock(
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeAllowOutOfStockPurchase = false)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
               .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] IN (")
               .Append("SELECT DISTINCT([productKey])")
               .Append("FROM (")
               .Append("SELECT	[merchProductVariant].[productKey]")
               .Append("FROM [merchProductVariant]")
               .Append("INNER JOIN [merchCatalogInventory]")
               .Append("ON	[merchProductVariant].[pk] = [merchCatalogInventory].[productVariantKey]")
               .Append("WHERE ([merchCatalogInventory].[count] > 0 AND [merchProductVariant].[trackInventory] = 1)")
               .Append("OR [merchProductVariant].[trackInventory] = 0")
               .Append(") [merchProductVariant]")
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products keys on sale.
        /// </summary>
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
        public Page<Guid> GetProductsKeysOnSale(
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
              .Append("WHERE [merchProductVariant].[onSale] = 1")
              .Append("AND [merchProductVariant].[master] = 1");
            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// Returns a value indicating whether or not the product exists in a collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid entityKey, Guid collectionKey)
        {
            var sql = new Sql();
            sql.Append("SELECT COUNT(*)")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[productKey] = @pkey AND [merchProduct2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @pkey = entityKey, @eckey = collectionKey });

            return Database.ExecuteScalar<int>(sql) > 0;
        }

        /// <summary>
        /// Adds a product to a static product collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void AddToCollection(Guid entityKey, Guid collectionKey)
        {
            if (this.ExistsInCollection(entityKey, collectionKey)) return;
            
            var dto = new Product2EntityCollectionDto()
                          {
                              ProductKey = entityKey,
                              EntityCollectionKey = collectionKey,
                              CreateDate = DateTime.Now,
                              UpdateDate = DateTime.Now
                          };

            Database.Insert(dto);
        }

        /// <summary>
        /// The remove product from collection.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public void RemoveFromCollection(Guid entityKey, Guid collectionKey)
        {
            Database.Execute(
                "DELETE [merchProduct2EntityCollection] WHERE [merchProduct2EntityCollection].[productKey] = @pkey AND [merchProduct2EntityCollection].[entityCollectionKey] = @eckey",
                new { @pkey = entityKey, @eckey = collectionKey });
        }

        /// <summary>
        /// The get product keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        public Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] IN (")
               .Append("SELECT DISTINCT([productKey])")
               .Append("FROM [merchProduct2EntityCollection]")
               .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] = @eckey", new { @eckey = collectionKey })
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        public Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        public Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] NOT IN (")
               .Append("SELECT DISTINCT([productKey])")
               .Append("FROM [merchProduct2EntityCollection]")
               .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] = @eckey", new { @eckey = collectionKey })
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get keys not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        public Page<Guid> GetKeysNotInCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] NOT IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
        }

        /// <summary>
        /// The get products from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="Page{IProduct}"/>.
        /// </returns>
        public Page<IProduct> GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = this.GetKeysFromCollection(collectionKey, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<IProduct>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(Get).ToList()
            };
        }

        /// <summary>
        /// The get from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="term">
        /// The term.
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
        /// The <see cref="Page{IProduct}"/>.
        /// </returns>
        public Page<IProduct> GetFromCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var p = GetKeysFromCollection(collectionKey, term, page, itemsPerPage, orderExpression, sortDirection);

            return new Page<IProduct>()
            {
                CurrentPage = p.CurrentPage,
                ItemsPerPage = p.ItemsPerPage,
                TotalItems = p.TotalItems,
                TotalPages = p.TotalPages,
                Items = p.Items.Select(Get).ToList()
            };
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


        /// <summary>
        /// The perform get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        protected override IProduct PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            //var inventoryCollection = ((ProductVariantRepository)_productVariantRepository).GetCategoryInventoryCollection(dto.ProductVariantDto.Key);
            //var productAttributeCollection = ((ProductVariantRepository)_productVariantRepository).GetProductAttributeCollection(dto.ProductVariantDto.Key);

            var factory = new ProductFactory(
                ((ProductVariantRepository)_productVariantRepository).GetProductAttributeCollection,
                ((ProductVariantRepository)_productVariantRepository).GetCategoryInventoryCollection, 
                GetProductOptionCollection, 
                GetProductVariantCollection,
                ((ProductVariantRepository)_productVariantRepository).GetDetachedContentCollection);

            var product = factory.BuildEntity(dto);


            product.ResetDirtyProperties();

            return product;
        }

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProduct}"/>.
        /// </returns>
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
                var dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.Key);
                }
            }
        }


        /// <summary>
        /// The get base query.
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
               .From<ProductDto>(SqlSyntax)
               .InnerJoin<ProductVariantDto>(SqlSyntax)
               .On<ProductDto, ProductVariantDto>(SqlSyntax, left => left.Key, right => right.ProductKey)
               .InnerJoin<ProductVariantIndexDto>(SqlSyntax)
               .On<ProductVariantDto, ProductVariantIndexDto>(SqlSyntax, left => left.Key, right => right.ProductVariantKey)
               .Where<ProductVariantDto>(x => x.Master);

            return sql;
        }

        /// <summary>
        /// Gets the base SQL where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchProduct.pk = @Key";
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
                    @"DELETE FROM merchProductVariant2ProductAttribute WHERE optionKey IN 
                        (SELECT pk FROM merchProductOption WHERE pk IN 
                        (SELECT optionKey FROM merchProduct2ProductOption WHERE productKey = @Key))",                    
                    @"DELETE FROM merchProductAttribute WHERE optionKey IN 
                        (SELECT pk FROM merchProductOption WHERE pk IN 
                        (SELECT optionKey FROM merchProduct2ProductOption WHERE productKey = @Key))",
                    "DELETE FROM merchProduct2ProductOption WHERE productKey = @Key",
                    "DELETE FROM merchCatalogInventory WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Key)",
                    "DELETE FROM merchProductVariantDetachedContent WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Key)",
                    "DELETE FROM merchProductVariantIndex WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Key)",                    
                    "DELETE FROM merchProductVariant WHERE productKey = @Key",
                    "DELETE FROM merchProduct2EntityCollection WHERE productKey = @Key",
                    "DELETE FROM merchProduct WHERE pk = @Key",
                    "DELETE FROM merchProductOption WHERE pk NOT IN (SELECT optionKey FROM merchProduct2ProductOption)"
                };

            return list;
        }

        /// <summary>
        /// Saves a new product.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
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

            ((Product)entity).MasterVariant.ProductKey = dto.ProductVariantDto.ProductKey;
            ((Product)entity).MasterVariant.Key = dto.ProductVariantDto.Key;
            ((ProductVariant)((Product)entity).MasterVariant).ExamineId = dto.ProductVariantDto.ProductVariantIndexDto.Id;

            // save the product options
            SaveProductOptions(entity);

            // synchronize the inventory
            ((ProductVariantRepository)_productVariantRepository).SaveCatalogInventory(((Product)entity).MasterVariant);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(IProduct entity)
        {
            var cachedKeys = entity.ProductVariants.Select(x => x.Key).ToList();
            cachedKeys.Add(((ProductVariant)((Product)entity).MasterVariant).Key);

            ((Product)entity).UpdatingEntity();
            ((ProductVariant)((Product)entity).MasterVariant).VersionKey = Guid.NewGuid();

            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            Database.Update(dto.ProductVariantDto);

            SaveProductOptions(entity);

            // synchronize the inventory
            ((ProductVariantRepository)_productVariantRepository).SaveCatalogInventory(((Product)entity).MasterVariant);

            ((ProductVariantRepository)_productVariantRepository).SaveDetachedContents(((Product)entity).MasterVariant);

            entity.ResetDirtyProperties();
        }

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistDeletedItem(IProduct entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IProduct"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProduct}"/>.
        /// </returns>
        protected override IEnumerable<IProduct> PerformGetByQuery(IQuery<IProduct> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProduct>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// The get dto page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sql">
        /// The SQL string.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{ProductVariantDto}"/>.
        /// </returns>
        private new Page<ProductVariantDto> GetDtoPage(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending)
        {
            if (!string.IsNullOrEmpty(orderExpression))
            {
                // TODO this may contribute to the PetaPoco memory leak issue
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            return Database.Page<ProductVariantDto>(page, itemsPerPage, sql);
        }

        /// <summary>
        /// The get product option collection.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionCollection"/>.
        /// </returns>
        private ProductOptionCollection GetProductOptionCollection(Guid productKey)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductOptionDto>(SqlSyntax)
               .InnerJoin<Product2ProductOptionDto>(SqlSyntax)
               .On<ProductOptionDto, Product2ProductOptionDto>(SqlSyntax, left => left.Key, right => right.OptionKey)
               .Where<Product2ProductOptionDto>(x => x.ProductKey == productKey)
               .OrderBy<Product2ProductOptionDto>(x => x.SortOrder, SqlSyntax);

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

        /// <summary>
        /// Gets the product variant collection.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantCollection"/>.
        /// </returns>
        private ProductVariantCollection GetProductVariantCollection(Guid productKey)
        {
            var collection = new ProductVariantCollection();
            var query = Querying.Query<IProductVariant>.Builder.Where(x => x.ProductKey == productKey && ((ProductVariant)x).Master == false);
            var variants = _productVariantRepository.GetByQuery(query);
            foreach (var variant in variants)
            {
                if (variant != null) 
                    collection.Add(variant);
            }
            return collection;
        }

        /// <summary>
        /// Deletes a product option.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
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

        /// <summary>
        /// Saves a collection of product options.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        private void SaveProductOptions(IProduct product)
        {
            var existing = GetProductOptionCollection(product.Key);
            if (!product.DefinesOptions && !existing.Any()) return;

            //// ensure all ids are in the new list
            var resetSorts = false;
            foreach (var ex in existing)
            {
                // Von: Options with duplicate names are allowed and because of that this check will not work.
                //      If there are more than one options with the same name and one of those is removed on the back-end,
                //      it will not be deleted from the database because there are still existing options with the same name.
                //      So let's test by PK since the PKs are (and "should" be) passed from the DB (or cache) to the back-end UI.
                //if (!product.ProductOptions.Contains(ex.Name))
                if (!product.ProductOptions.Contains(ex.Key))
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
                    ((ProductOption)o).SortOrder = count;
                    count = count + 1;
                    product.ProductOptions.Add(o);
                }
            }

            foreach (var option in product.ProductOptions)
            {
                SaveProductOption(product, option);
            }
        }

        /// <summary>
        /// Saves a product option.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="productOption">
        /// The product option.
        /// </param>
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

                const string Update = "UPDATE merchProduct2ProductOption SET SortOrder = @So, updateDate = @Ud WHERE productKey = @pk AND optionKey = @OKey";

                Database.Execute(
                    Update,
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

        /// <summary>
        /// Saves the product attribute collection.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="productOption">
        /// The product option.
        /// </param>
        private void SaveProductAttributes(IProduct product, IProductOption productOption)
        {
            if (!productOption.Choices.Any()) return;

            var existing = GetProductAttributeCollection(productOption.Key);

            ////ensure all ids are in the new list
            var resetSorts = false;
            foreach (var ex in existing)
            {
                //// Von: Duplicate SKU is not allowed in the system. However, 
                ////      there could be custom implementation where SKUs are generated based on some logic.
                ////      That could lead into a timing issue where the name of the attribute is not the same as with its SKU.
                ////      "Ideally" the logic should be put in correct event but it's easy to miss that.
                ////      So this change will ensure that even if that timing is off,
                ////      we will still not remove attributes unnecessarily.
                ////if (productOption.Choices.Contains(ex.Sku)) continue;

                //// TODO RSS:  We still need to validate SKU integrity so that we do not create an issue when generating the product variants.
                ////            We need to add the ability to change the attribute SKU via the back office with a UI change anyway so we'll look at this
                ////            again at that time.

                if (productOption.Choices.Contains(ex.Key)) continue;
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

            // We need to save now the correct ordering so the display on the UI will be correct.
            // The attributes in "all" options are assigned sort orders that are off.
            // For example the Color Red is given an order of 10 and the Size 2 is given an order of 5.
            // If Color is ordered first than the Size then we should have "Red" first then "2".
            // But since the ordering is taken from the attribute order then we have a wrong display.
            // Let's use the options sortOrder as an offset to have a correct attributes ordering.      
            var attributeSortOrderOffset = productOption.SortOrder * 100;
            foreach (var att in productOption.Choices.OrderBy(x => x.SortOrder))
            {
                // ensure the id is set
                att.OptionKey = productOption.Key;
                // adjust the ordering of attributes
                att.SortOrder = attributeSortOrderOffset + att.SortOrder;
                SaveProductAttribute(att);
            }

            //// this is required due to the special case relation between a product and product variants
            foreach (var variant in product.ProductVariants)
            {
                RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<IProductVariant>(variant.Key));
            }
        }

        /// <summary>
        /// Saves a product attribute.
        /// </summary>
        /// <param name="productAttribute">
        /// The product attribute.
        /// </param>
        private void SaveProductAttribute(IProductAttribute productAttribute)
        {
            var factory = new ProductAttributeFactory();

            if (!productAttribute.HasIdentity)
            {
                productAttribute.UseCount = 1;
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

        /// <summary>
        /// Builds the product search SQL.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        private Sql BuildProductSearchSql(string searchTerm)
        {
            searchTerm = searchTerm.Replace(",", " ");
            var invidualTerms = searchTerm.Split(' ');

            var terms = invidualTerms.Where(x => !string.IsNullOrEmpty(x)).ToList();


            var sql = new Sql();
            sql.Select("*").From<ProductVariantDto>(SqlSyntax);

            if (terms.Any())
            {
                var preparedTerms = string.Format("%{0}%", string.Join("% ", terms)).Trim();

                sql.Where("sku LIKE @sku OR name LIKE @name", new { @sku = preparedTerms, @name = preparedTerms });
            }

            sql.Where("master = @master", new { @master = true });

            return sql;
        }
    }
}
