namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <inheritdoc/>
    internal partial class ProductRepository : IProductCollectionPriceQueries
    {
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
    }
}
