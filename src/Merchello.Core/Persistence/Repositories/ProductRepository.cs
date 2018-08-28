namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    // REFACTOR request based caching should be either all done in .Web services or all done here in a base class

    /// <summary>
    /// The product repository.
    /// </summary>
    internal partial class ProductRepository : PagedRepositoryBase<IProduct, ProductDto>, IProductRepository
    {
        /// <summary>
        /// The product variant repository.
        /// </summary>
        private readonly IProductVariantRepository _productVariantRepository;

        /// <summary>
        /// The product option repository.
        /// </summary>
        private readonly IProductOptionRepository _productOptionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        /// <param name="productVariantRepository">
        /// The product variant repository.
        /// </param>
        /// <param name="productOptionRepository">
        /// The product option Repository.
        /// </param>
        public ProductRepository(IDatabaseUnitOfWork work, ILogger logger, ISqlSyntaxProvider sqlSyntax, IProductVariantRepository productVariantRepository, IProductOptionRepository productOptionRepository)
            : base(work, logger, sqlSyntax)
        {
            Mandate.ParameterNotNull(productVariantRepository, "productVariantRepository");
            Mandate.ParameterNotNull(productOptionRepository, "productOptionRepository");

            _productVariantRepository = productVariantRepository;
            _productOptionRepository = productOptionRepository;
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
        //// REFACTOR IQuery is a worthless parameter here
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
            return SearchKeys(searchTerm, page, itemsPerPage, orderExpression, sortDirection, false);
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
        public Page<Guid> SearchKeys(
            string searchTerm,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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
        /// <param name="optionKey">
        /// The option key.
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
            Guid optionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = new Sql("SELECT *")
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
                .Append("WHERE [merchProductOption].[pk] = @key", new { @key = optionKey })
                .Append(") [merchProductVariant]")
                .Append(")")
                .Append("AND [merchProductVariant].[master] = 1");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            return GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            return GetProductsKeysWithOption(new[] { optionName }, page, itemsPerPage, orderExpression, sortDirection, includeUnavailable);
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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{GUID}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysWithOption(
            IEnumerable<string> optionNames,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            return GetProductsKeysInPriceRange(min, max, 0, page, itemsPerPage, orderExpression, sortDirection, includeUnavailable);
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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysByManufacturer(
            string manufacturer,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysByManufacturer(
            IEnumerable<string> manufacturer,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
              .Append("WHERE [merchProductVariant].[manufacturer] IN (@manufacturers)", new { @manufacturers = manufacturer})
              .Append("AND [merchProductVariant].[master] = 1");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysByBarcode(
            string barcode,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysByBarcode(
            IEnumerable<string> barcodes,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
              .Append("WHERE [merchProductVariant].[barcode] IN (@codes)", new { @codes = barcodes })
              .Append("AND [merchProductVariant].[master] = 1");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysInStock(
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeAllowOutOfStockPurchase = false,
            bool includeUnavailable = false)
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

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetProductsKeysOnSale(
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
              .Append("WHERE [merchProductVariant].[onSale] = 1")
              .Append("AND [merchProductVariant].[master] = 1");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

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
        /// Returns a value indicating whether or not the entity exists in at least one of the collections.
        /// </summary>
        /// <param name="entityKey">
        /// The entity key.
        /// </param>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ExistsInCollection(Guid entityKey, Guid[] collectionKeys)
        {
            var sql = new Sql();
            sql.Append("SELECT COUNT(*)")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[productKey] = @pkey AND [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @pkey = entityKey, @eckeys = collectionKeys });

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
        /// Bulk inserts products to a collection
        /// </summary>
        /// <param name="entityAndCollectionKeys"></param>
        public void AddToCollections(Dictionary<Guid, Guid> entityAndCollectionKeys)
        {
            var dtos = new List<Product2EntityCollectionDto>();

            var allMerchProduct2EntityCollections = GetAllMerchProduct2EntityCollections();

            foreach (var entityAndCollectionKey in entityAndCollectionKeys)
            {
                var key = string.Concat(entityAndCollectionKey.Key, "|", entityAndCollectionKey.Value);

                if (!allMerchProduct2EntityCollections.ContainsKey(key))
                {
                    //Guid entityKey, Guid collectionKey
                    dtos.Add(new Product2EntityCollectionDto
                    {
                        ProductKey = entityAndCollectionKey.Key,
                        EntityCollectionKey = entityAndCollectionKey.Value,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }
            }

            Database.BulkInsertRecords(dtos);
        }

        /// <summary>
        /// Creates a dictionary we can look up to check existing MerchProduct2EntityCollection
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Product2EntityCollectionDto> GetAllMerchProduct2EntityCollections()
        {
            var sql = new Sql()
                .Select("*")
                .From("merchProduct2EntityCollection");

            var competitions = Database.Query<Product2EntityCollectionDto>(sql);

            var product2EntityCollectionDict = new Dictionary<string, Product2EntityCollectionDto>();

            foreach (var mpec in competitions)
            {
                var key = string.Concat(mpec.ProductKey, "|", mpec.EntityCollectionKey);
                product2EntityCollectionDict.Add(key, mpec);
            }

            return product2EntityCollectionDict;
        }

        /// <summary>
        /// Batch removes from collections
        /// </summary>
        /// <param name="entityKeycollectionKey">
        /// Key=ProductKey
        /// Value=collectionKey
        /// </param>
        public void RemoveFromCollections(Dictionary<Guid , Guid> entityKeycollectionKey)
        {
            var sql = new Sql();

            foreach (var dict in entityKeycollectionKey)
            {
                sql.Append(
                    "DELETE [merchProduct2EntityCollection] WHERE [merchProduct2EntityCollection].[productKey] = @pkey AND [merchProduct2EntityCollection].[entityCollectionKey] = @eckey",
                    new {@pkey = dict.Key, @eckey = dict.Value});
            }

            Database.Execute(sql);
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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysFromCollection(collectionKey, page, itemsPerPage, orderExpression, sortDirection, false);
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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetKeysFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);

            return pagedKeys;
        }


        /// <summary>
        /// The get product keys from collection.
        /// </summary>
        /// <param name="collectionKeys">
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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysThatExistInAllCollections(collectionKeys, page, itemsPerPage, orderExpression, sortDirection, false);
        }

        /// <summary>
        /// The get product keys from collection.
        /// </summary>
        /// <param name="collectionKeys">
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
        /// <param name="includeUnavailable">
        /// Include products that are marked as not available
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        public Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] IN (")
               .Append("SELECT [productKey]")
               .Append("FROM [merchProduct2EntityCollection]")
               .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append("GROUP BY productKey")
               .Append("HAVING COUNT(*) = @keyCount", new { @keyCount = collectionKeys.Count() })
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }
            var pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);

            return pagedKeys;
        }

        public int CountKeysThatExistInAllCollections(Guid[] collectionKeys, bool includeUnavailable = false)
        {
            return Database.ExecuteScalar<int>(SqlForKeysThatExistInAllCollections(collectionKeys, true, includeUnavailable));
        }

        public IEnumerable<Tuple<IEnumerable<Guid>, int>> CountKeysThatExistInAllCollections(IEnumerable<Guid[]> collectionKeysGroups, bool includeUnavailable = false)
        {
            var sql = new Sql();
            var keysGroups = collectionKeysGroups as Guid[][] ?? collectionKeysGroups.ToArray();
            foreach (var group in keysGroups)
            {
                if (sql.SQL.Length > 0) sql.Append("UNION");
                sql.Append(string.Format("SELECT {0} as Hash", group.GetHashCode())) // can't paramertize this SqlCE chokes but it should not matter since it's just a value.
                    .Append(", T1.Count")
                    .Append("FROM (")
                    .Append("SELECT COUNT(*) AS Count")
                       .Append("FROM [merchProductVariant]")
                       .Append("WHERE [merchProductVariant].[productKey] IN (")
                       .Append("SELECT [productKey]")
                       .Append("FROM [merchProduct2EntityCollection]")
                       .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = group })
                       .Append("GROUP BY productKey")
                       .Append("HAVING COUNT(*) = @keyCount", new { @keyCount = group.Count() })
                       .Append(")")
                       .Append("AND [merchProductVariant].[master] = 1");

                if (!includeUnavailable)
                {
                    sql.Append("AND [merchProductVariant].[available] = 1");
                }

                sql.Append(") AS T1");

            }

            var dtos = Database.Fetch<CountDto>(sql);

            var results = new List<Tuple<IEnumerable<Guid>, int>>();
            foreach (var group in keysGroups)
            {
                var hash = group.GetHashCode();
                var dto = dtos.FirstOrDefault(x => x.Hash == hash);
                if (dto != null) results.Add(new Tuple<IEnumerable<Guid>, int>(group, dto.Count));
            }

            return results;
        }

        private Sql SqlForKeysThatExistInAllCollections(Guid[] collectionKeys, bool isCount = false, bool includeUnavailable = false)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*) AS Count" : "*")
                .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] IN (")
               .Append("SELECT [productKey]")
               .Append("FROM [merchProduct2EntityCollection]")
               .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append("GROUP BY productKey")
               .Append("HAVING COUNT(*) = @keyCount", new { @keyCount = collectionKeys.Count() })
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            return sql;
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
        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysFromCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysFromCollection(collectionKey, term, page, itemsPerPage, orderExpression, sortDirection, false);
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {

            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);

            return pagedKeys;
        }

        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysThatExistInAllCollections(collectionKeys, term, page, itemsPerPage, orderExpression, sortDirection, false);
        }

        public Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
                .Append("GROUP BY productKey")
                .Append("HAVING COUNT(*) = @keyCount", new { @keyCount = collectionKeys.Count() })
                .Append(")");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }


            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);

            return pagedKeys;
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
        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysNotInCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysNotInCollection(collectionKey, page, itemsPerPage, orderExpression, sortDirection, false);
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
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

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return pagedKeys;
        }

        /// <summary>
        /// Gets the page of product keys that do not exist in any of the collections with keys passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysNotInAnyCollections(collectionKeys, page, itemsPerPage, orderExpression, sortDirection, false);
        }

        /// <summary>
        /// Gets the page of product keys that do not exist in any of the collections with keys passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        public Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] NOT IN (")
               .Append("SELECT DISTINCT([productKey])")
               .Append("FROM [merchProduct2EntityCollection]")
               .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return pagedKeys;
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
        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysNotInCollection(
            Guid collectionKey,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysNotInCollection(collectionKey, term, page, itemsPerPage, orderExpression, sortDirection, false);
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {

            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] NOT IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return pagedKeys;
        }

        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysNotInAnyCollections(collectionKeys, term, page, itemsPerPage, orderExpression, sortDirection);
        }

        public Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] NOT IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
                .Append(")");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return pagedKeys;
        }

        /// <summary>
        /// Gets a collection of keys that exist in any one of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysThatExistInAnyCollections(collectionKeys, page, itemsPerPage, orderExpression, sortDirection, false);
        }

        /// <summary>
        /// Gets a collection of keys that exist in any one of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
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
        public Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {

            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] IN (")
               .Append("SELECT DISTINCT([productKey])")
               .Append("FROM [merchProduct2EntityCollection]")
               .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return pagedKeys;
        }

        /// <summary>
        /// Gets a collection of keys that exist in any one of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="term">
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
        Page<Guid> IStaticEntityCollectionRepository<IProduct>.GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetKeysThatExistInAnyCollections(collectionKeys, term, page, itemsPerPage, orderExpression, sortDirection, false);
        }

        /// <summary>
        /// Gets a collection of keys that exist in any one of the collections passed.
        /// </summary>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <param name="term">
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
        public Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
                .Append(")");

            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return pagedKeys;
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
            return GetFromCollection(collectionKey, page, itemsPerPage, orderExpression, sortDirection);
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var p = this.GetKeysFromCollection(collectionKey, page, itemsPerPage, orderExpression, sortDirection, includeUnavailable);

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
        /// The get products from collection.
        /// </summary>
        /// <param name="collectionKeys">
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
        Page<IProduct> IStaticEntityCollectionRepository<IProduct>.GetEntitiesThatExistInAllCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetEntitiesThatExistInAllCollections(collectionKeys, page, itemsPerPage, orderExpression, sortDirection, false);
        }

        /// <summary>
        /// The get products from collection.
        /// </summary>
        /// <param name="collectionKeys">
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
        public Page<IProduct> GetEntitiesThatExistInAllCollections(
            Guid[] collectionKeys,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var p = this.GetKeysThatExistInAllCollections(collectionKeys, page, itemsPerPage, orderExpression, sortDirection, includeUnavailable);

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
            return GetFromCollection(collectionKey, term, page, itemsPerPage, orderExpression, sortDirection, false);
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
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var p = GetKeysFromCollection(collectionKey, term, page, itemsPerPage, orderExpression, sortDirection, includeUnavailable);

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
        /// <param name="collectionKeys">
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
        Page<IProduct> IStaticEntityCollectionRepository<IProduct>.GetEntitiesThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            return GetEntitiesThatExistInAllCollections(collectionKeys, page, itemsPerPage, orderExpression, sortDirection, false);
        }

        /// <summary>
        /// The get from collection.
        /// </summary>
        /// <param name="collectionKeys">
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
        public Page<IProduct> GetEntitiesThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending,
            bool includeUnavailable = false)
        {
            var p = this.GetKeysThatExistInAllCollections(collectionKeys, term, page, itemsPerPage, orderExpression, sortDirection, includeUnavailable);

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
        protected Page<Guid> GetPagedKeys(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending, bool includeUnavailable = false)
        {
            var p = GetDtoPage(page, itemsPerPage, sql, orderExpression, sortDirection, includeUnavailable);

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

            var factory = new ProductFactory(
                _productOptionRepository.GetProductAttributeCollectionForVariant(dto.ProductVariantDto.Key),
                _productVariantRepository.GetCategoryInventoryCollection(dto.ProductVariantDto.Key),
                _productOptionRepository.GetProductOptionCollection,
                _productVariantRepository.GetProductVariantCollection,
                _productVariantRepository.GetDetachedContentCollection(dto.ProductVariantDto.Key));

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

            var dtos = new List<ProductDto>();

            if (keys.Any())
            {
                // This is to get around the WhereIn max limit of 2100 parameters and to help with performance of each WhereIn query
                var keyLists = keys.Split(400).ToList();

                // Loop the split keys and get them
                foreach (var keyList in keyLists)
                {
                    dtos.AddRange(Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false).WhereIn<ProductDto>(x => x.Key, keyList, SqlSyntax)));
                }
            }
            else
            {
                dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false));
            }

            foreach (var dto in dtos)
            {
                // TODO - Performance tune
                var factory = new ProductFactory(
                    _productOptionRepository.GetProductAttributeCollectionForVariant(dto.ProductVariantDto.Key),
                    _productVariantRepository.GetCategoryInventoryCollection(dto.ProductVariantDto.Key),
                    _productOptionRepository.GetProductOptionCollection,
                    _productVariantRepository.GetProductVariantCollection,
                    _productVariantRepository.GetDetachedContentCollection(dto.ProductVariantDto.Key));

                var product = factory.BuildEntity(dto);

                product.ResetDirtyProperties();

                yield return product;
            }



            //if (keys.Any())
            //{
            //    foreach (var id in keys)
            //    {
            //        yield return Get(id);
            //    }
            //}
            //else
            //{
            //    var dtos = Database.Fetch<ProductDto, ProductVariantDto, ProductVariantIndexDto>(GetBaseQuery(false));
            //    foreach (var dto in dtos)
            //    {
            //        yield return Get(dto.Key);
            //    }
            //}
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
               .Where<ProductVariantDto>(x => x.Master, SqlSyntax);

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
                    "DELETE FROM merchCatalogInventory WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Key)",
                    "DELETE FROM merchProductVariantDetachedContent WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Key)",
                    "DELETE FROM merchProductVariantIndex WHERE productVariantKey IN (SELECT pk FROM merchProductVariant WHERE productKey = @Key)",
                    "DELETE FROM merchProductVariant WHERE productKey = @Key",
                    "DELETE FROM merchProduct2EntityCollection WHERE productKey = @Key",
                    "DELETE FROM merchProduct WHERE pk = @Key"
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
            _productOptionRepository.SaveForProduct(entity);

            // synchronize the inventory
            _productVariantRepository.SaveCatalogInventory(((Product)entity).MasterVariant);

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

            _productOptionRepository.SaveForProduct(entity);

            // synchronize the inventory
            _productVariantRepository.SaveCatalogInventory(((Product)entity).MasterVariant);

            _productVariantRepository.SaveDetachedContents(((Product)entity).MasterVariant);

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
           _productOptionRepository.DeleteAllProductOptions(entity);

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
        private new Page<ProductVariantDto> GetDtoPage(long page, long itemsPerPage, Sql sql, string orderExpression, SortDirection sortDirection = SortDirection.Descending, bool includeUnavailable = false)
        {
            if (!includeUnavailable)
            {
                sql.Append("AND [merchProductVariant].[available] = 1");
            }

            if (!string.IsNullOrEmpty(orderExpression))
            {
                sql.Append(sortDirection == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            return Database.Page<ProductVariantDto>(page, itemsPerPage, sql);
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
        [Obsolete]
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
