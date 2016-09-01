namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Search;

    public static class QueryBuilderExtensions
    {
        public static IProductContentQueryBuilder ConstrainBy(this IProductContentQueryBuilder builder, IProductFilter filter)
        {
            builder.AddConstraint(filter);
            return builder;
        }

        public static IProductContentQueryBuilder ConstrainBy(this IProductContentQueryBuilder builder, IEnumerable<IProductFilter> filters)
        {
            builder.AddConstraint(filters);
            return builder;
        }

        public static IProductContentQueryBuilder ConstrainBy(this IProductContentQueryBuilder builder, IProductCollection collection)
        {
            builder.AddConstraint(collection);
            return builder;
        }

        public static IProductContentQueryBuilder ConstrainBy(this IProductContentQueryBuilder builder, IEnumerable<IProductCollection> collections)
        {
            builder.AddConstraint(collections);
            return builder;
        }

        public static IProductContentQueryBuilder ConstrainByCollectionKey(this IProductContentQueryBuilder builder, Guid collectionKey)
        {
            builder.AddCollectionKeyConstraint(collectionKey);
            return builder;
        }

        public static IProductContentQueryBuilder ConstrainByCollectionKey(this IProductContentQueryBuilder builder, IEnumerable<Guid> collectionKeys)
        {
            builder.AddCollectionKeyConstraint(collectionKeys);
            return builder;
        }

        public static IProductContentQueryBuilder Page(this IProductContentQueryBuilder builder, long page)
        {
            if (page < 1) page = 1;
            builder.Page = page;
            return builder;
        }

        public static IProductContentQueryBuilder ItemsPerPage(this IProductContentQueryBuilder builder, long itemsPerPage)
        {
            if (itemsPerPage < 1) itemsPerPage = 10;
            builder.ItemsPerPage = itemsPerPage;
            return builder;
        }

        public static IProductContentQueryBuilder OrderBy(this IProductContentQueryBuilder builder, ProductSortField sortBy, SortDirection sortDirection = SortDirection.Ascending)
        {
            builder.SortBy = sortBy;
            builder.SortDirection = sortDirection;
            return builder;
        }

        public static IProductContentQueryBuilder Clusivity(this IProductContentQueryBuilder builder, FilterQueryClusivity clusivity)
        {
            builder.FilterQueryClusivity = clusivity;
            return builder;
        }
    }
}