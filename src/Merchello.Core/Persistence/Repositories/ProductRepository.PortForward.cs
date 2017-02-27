namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <inheritdoc/>
    internal partial class ProductRepository : IPortForwardProductRepository
    {
        /// <summary>
        /// A list of valid search fields.
        /// </summary>
        private static readonly string[] ValidSearchFields = { "name", "sku" };

        ///// <inheritdoc/>
        //public Page<Guid> GetKeysNotInCollection(
        //    Guid collectionKey,
        //    decimal min,
        //    decimal max,
        //    long page,
        //    long itemsPerPage,
        //    string orderExpression,
        //    SortDirection sortDirection = SortDirection.Descending)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <inheritdoc/>
        //public Page<Guid> GetKeysNotInCollection(
        //    Guid collectionKey,
        //    string term,
        //    decimal min,
        //    decimal max,
        //    long page,
        //    long itemsPerPage,
        //    string orderExpression,
        //    SortDirection sortDirection = SortDirection.Descending)
        //{
        //    throw new NotImplementedException();
        //}

        /// <inheritdoc/>
        public Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = GetPagedDtoCacheKey(
                                       "GetKeysFromCollection",
                                       page,
                                       itemsPerPage,
                                       orderExpression,
                                       sortDirection,
                                       new Dictionary<string, string>
                                           {
                                                { "collectionKeys", string.Join(string.Empty, collectionKeys) },
                                                { "minmax", string.Join(string.Empty, new object[] { min, max }) }
                                           });

            var pagedKeys = TryGetCachedPageOfKeys(cacheKey);
            if (pagedKeys != null) return pagedKeys;

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
               .Append("AND (([merchProductVariant].[onSale] = 0 AND [merchProductVariant].[price] BETWEEN @low AND @high)", new { @low = min, @high = max })
              .Append("OR")
              .Append("([merchProductVariant].[onSale] = 1 AND [merchProductVariant].[salePrice] BETWEEN @low AND @high)", new { @low = min, @high = max })
              .Append(")")
              .Append("AND [merchProductVariant].[master] = 1");

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);

            return CachePageOfKeys(cacheKey, pagedKeys);
        }

        /// <inheritdoc/>
        public Page<Guid> GetKeysThatExistInAllCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = GetPagedDtoCacheKey(
                            "GetKeysThatExistInAllCollections",
                            page,
                            itemsPerPage,
                            orderExpression,
                            sortDirection,
                            new Dictionary<string, string>
                                {
                                    { "collectionKey", string.Join(string.Empty, collectionKeys) },
                                    { "term", term },
                                    { "minmax", string.Join(string.Empty, new object[] { min, max }) }
                                });

            var pagedKeys = TryGetCachedPageOfKeys(cacheKey);
            if (pagedKeys != null) return pagedKeys;

            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckey = collectionKeys })
                .Append("GROUP BY productKey")
                .Append("HAVING COUNT(*) = @keyCount", new { @keyCount = collectionKeys.Count() })
                .Append(")")
                .Append("AND (([merchProductVariant].[onSale] = 0 AND [merchProductVariant].[price] BETWEEN @low AND @high)", new { @low = min, @high = max })
                .Append("OR")
                .Append("([merchProductVariant].[onSale] = 1 AND [merchProductVariant].[salePrice] BETWEEN @low AND @high)", new { @low = min, @high = max })
                .Append(")");

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);

            return CachePageOfKeys(cacheKey, pagedKeys);
        }

        /// <inheritdoc/>
        public Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = GetPagedDtoCacheKey(
                                       "GetKeysNotInAnyCollections",
                                       page,
                                       itemsPerPage,
                                       orderExpression,
                                       sortDirection,
                                       new Dictionary<string, string>
                                           {
                                                { "collectionKeys", string.Join(string.Empty, collectionKeys) },
                                                { "minmax", string.Join(string.Empty, new object[] { min, max }) }
                                           });

            var pagedKeys = TryGetCachedPageOfKeys(cacheKey);
            if (pagedKeys != null) return pagedKeys;

            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] NOT IN (")
               .Append("SELECT DISTINCT([productKey])")
               .Append("FROM [merchProduct2EntityCollection]")
               .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append(")")
               .Append("AND (([merchProductVariant].[onSale] = 0 AND [merchProductVariant].[price] BETWEEN @low AND @high)", new { @low = min, @high = max })
               .Append("OR")
               .Append("([merchProductVariant].[onSale] = 1 AND [merchProductVariant].[salePrice] BETWEEN @low AND @high)", new { @low = min, @high = max })
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return CachePageOfKeys(cacheKey, pagedKeys);
        }

        /// <inheritdoc/>
        public Page<Guid> GetKeysNotInAnyCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = GetPagedDtoCacheKey(
                                      "GetKeysNotInAnyCollections",
                                      page,
                                      itemsPerPage,
                                      orderExpression,
                                      sortDirection,
                                      new Dictionary<string, string>
                                          {
                                            { "collectionKeys", string.Join(string.Empty, collectionKeys) },
                                            { "term", term },
                                            { "minmax", string.Join(string.Empty, new object[] { min, max }) }
                                          });

            var pagedKeys = TryGetCachedPageOfKeys(cacheKey);
            if (pagedKeys != null) return pagedKeys;


            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] NOT IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
                .Append(")")
                .Append("AND (([merchProductVariant].[onSale] = 0 AND [merchProductVariant].[price] BETWEEN @low AND @high)", new { @low = min, @high = max })
                .Append("OR")
                .Append("([merchProductVariant].[onSale] = 1 AND [merchProductVariant].[salePrice] BETWEEN @low AND @high)", new { @low = min, @high = max })
                .Append(")");

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return CachePageOfKeys(cacheKey, pagedKeys);
        }

        /// <inheritdoc/>
        public Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = GetPagedDtoCacheKey(
                                       "GetKeysThatExistInAnyCollectionss",
                                       page,
                                       itemsPerPage,
                                       orderExpression,
                                       sortDirection,
                                       new Dictionary<string, string>
                                           {
                                                { "collectionKeys", string.Join(string.Empty, collectionKeys) },
                                                { "minmax", string.Join(string.Empty, new object[] { min, max }) }
                                           });

            var pagedKeys = TryGetCachedPageOfKeys(cacheKey);
            if (pagedKeys != null) return pagedKeys;

            var sql = new Sql();
            sql.Append("SELECT *")
              .Append("FROM [merchProductVariant]")
               .Append("WHERE [merchProductVariant].[productKey] IN (")
               .Append("SELECT DISTINCT([productKey])")
               .Append("FROM [merchProduct2EntityCollection]")
               .Append("WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)", new { @eckeys = collectionKeys })
               .Append(")")
               .Append("AND (([merchProductVariant].[onSale] = 0 AND [merchProductVariant].[price] BETWEEN @low AND @high)", new { @low = min, @high = max })
               .Append("OR")
               .Append("([merchProductVariant].[onSale] = 1 AND [merchProductVariant].[salePrice] BETWEEN @low AND @high)", new { @low = min, @high = max })
               .Append(")")
               .Append("AND [merchProductVariant].[master] = 1");

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return CachePageOfKeys(cacheKey, pagedKeys);
        }

        /// <inheritdoc/>
        public Page<Guid> GetKeysThatExistInAnyCollections(
            Guid[] collectionKeys,
            string term,
            decimal min,
            decimal max,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending)
        {
            var cacheKey = GetPagedDtoCacheKey(
                                       "GetKeysThatExistInAnyCollections",
                                       page,
                                       itemsPerPage,
                                       orderExpression,
                                       sortDirection,
                                       new Dictionary<string, string>
                                           {
                                                { "collectionKeys", string.Join(string.Empty, collectionKeys) },
                                                { "term", term },
                                                { "minmax", string.Join(string.Empty, new object[] { min, max }) }
                                           });

            var pagedKeys = TryGetCachedPageOfKeys(cacheKey);
            if (pagedKeys != null) return pagedKeys;


            var sql = this.BuildProductSearchSql(term);
            sql.Append("AND [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] IN (@eckeys)",
                    new { @eckeys = collectionKeys })
                .Append(")")
                .Append("AND (([merchProductVariant].[onSale] = 0 AND [merchProductVariant].[price] BETWEEN @low AND @high)", new { @low = min, @high = max })
                .Append("OR")
                .Append("([merchProductVariant].[onSale] = 1 AND [merchProductVariant].[salePrice] BETWEEN @low AND @high)", new { @low = min, @high = max })
                .Append(")");

            pagedKeys = GetPagedKeys(page, itemsPerPage, sql, orderExpression, sortDirection);
            return CachePageOfKeys(cacheKey, pagedKeys);
        }

        /// <summary>
        /// Gets a list of currently listed Manufacturers.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> (manufacturer names).
        /// </returns>
        public IEnumerable<string> GetAllManufacturers()
        {
            var sql = new Sql("SELECT DISTINCT(manufacturer)")
                .From<ProductVariantDto>(SqlSyntax)
                .Where<ProductVariantDto>(x => x.Manufacturer != string.Empty, SqlSyntax)
                .OrderBy<ProductVariantDto>(x => x.Manufacturer, SqlSyntax);

            var results = Database.Fetch<string>(sql);

            return results;
        }

        /// <inheritdoc/> 
        public PagedCollection<IProduct> GetByAdvancedSearch(
            Guid collectionKey,
            string[] includeFields,
            string term,
            string manufacturer,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection direction = SortDirection.Ascending)
        {

            var sql = BuildAdvancedProductSearchSql(term, includeFields);
            if (includeFields.Contains("manufacturer") && !manufacturer.IsNullOrWhiteSpace())
            {
                sql.Where<ProductVariantDto>(x => x.Manufacturer == manufacturer, SqlSyntax);
            }

            if (!collectionKey.Equals(Guid.Empty))
            {
                sql.Append("AND [merchProductVariant].[productKey] IN (")
                .Append("SELECT DISTINCT([productKey])")
                .Append("FROM [merchProduct2EntityCollection]")
                .Append(
                    "WHERE [merchProduct2EntityCollection].[entityCollectionKey] = @eckey",
                    new { @eckey = collectionKey })
                .Append(")");
            }

            if (!string.IsNullOrEmpty(orderExpression))
            {
                sql.Append(direction == SortDirection.Ascending
                    ? string.Format("ORDER BY {0} ASC", orderExpression)
                    : string.Format("ORDER BY {0} DESC", orderExpression));
            }

            var results = Database.Page<ProductVariantDto>(page, itemsPerPage, sql);

            var products = GetAll(results.Items.Select(x => x.ProductKey).ToArray());


            return new PagedCollection<IProduct>
            {
                CurrentPage = results.CurrentPage,
                Items = products,
                PageSize = results.ItemsPerPage,
                TotalItems = results.TotalItems,
                TotalPages = results.TotalPages,
                SortField = orderExpression
            };
        }

        private Sql BuildAdvancedProductSearchSql(string searchTerm, string[] includeFields)
        {
            searchTerm = searchTerm.Replace(",", " ");
            var invidualTerms = searchTerm.Split(' ');

            var terms = invidualTerms.Where(x => !string.IsNullOrEmpty(x)).ToList();

            var validFields = ValidSearchFields.Where(includeFields.Contains).ToArray();

            var sql = new Sql();
            sql.Select("*").From<ProductVariantDto>(SqlSyntax);

            if (terms.Any())
            {
                var preparedTerms = string.Format("%{0}%", string.Join("% ", terms)).Trim();

                var fieldSql = string.Empty;
                foreach (var field in validFields)
                {
                    if (!fieldSql.IsNullOrWhiteSpace()) fieldSql += " OR ";
                    fieldSql += string.Format("{0} LIKE @prepped", field);
                }

                sql.Where(fieldSql, new { @prepped = preparedTerms });
            }

            sql.Where("master = @master", new { @master = true });

            return sql;
        }
    }
}
