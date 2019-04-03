namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Search;

    /// <summary>
    /// Extensions methods for filter query builders.
    /// </summary>
    public static class QueryBuilderExtensions
    {
        /// <summary>
        /// Adds a constraint by a <see cref="IProductFilter"/>.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder ConstrainBy(this IProductContentQueryBuilder builder, IProductFilter filter)
        {
            builder.AddConstraint(filter);
            return builder;
        }

        /// <summary>
        /// Adds a constraint by a collection of <see cref="IProductFilter"/>.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="filters">
        /// The filters.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder ConstrainBy(this IProductContentQueryBuilder builder, IEnumerable<IProductFilter> filters)
        {
            builder.AddConstraint(filters);
            return builder;
        }

        /// <summary>
        /// Adds a constraint by a <see cref="IProductCollection"/>.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder ConstrainBy(this IProductContentQueryBuilder builder, IProductCollection collection)
        {
            builder.AddConstraint(collection);
            return builder;
        }

        /// <summary>
        /// Adds a constraint by a collection of <see cref="IProductCollection"/>.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="collections">
        /// The collections.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder ConstrainBy(this IProductContentQueryBuilder builder, IEnumerable<IProductCollection> collections)
        {
            builder.AddConstraint(collections);
            return builder;
        }

        /// <summary>
        /// Directly adds a constraint by a <see cref="IEntityCollection"/> key.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        /// <remarks>
        /// Filters and Collections are actually both <see cref="IEntityCollection"/>s and the underlying query just uses the 
        /// collection key(s) so they can be added directly.
        /// </remarks>
        public static IProductContentQueryBuilder ConstrainByCollectionKey(this IProductContentQueryBuilder builder, Guid collectionKey)
        {
            builder.AddCollectionKeyConstraint(collectionKey);
            return builder;
        }

        /// <summary>
        /// Directly adds a constraint by a collection of <see cref="IEntityCollection"/> keys.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="collectionKeys">
        /// The collection keys.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder ConstrainByCollectionKey(this IProductContentQueryBuilder builder, IEnumerable<Guid> collectionKeys)
        {
            builder.AddCollectionKeyConstraint(collectionKeys);
            return builder;
        }

        /// <summary>
        /// Assigns the page in the paging query result to be returned.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder Page(this IProductContentQueryBuilder builder, long page)
        {
            if (page < 1) page = 1;
            builder.Page = page;
            return builder;
        }

        /// <summary>
        /// Assigns the number of items per page in the query result to be returned.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder ItemsPerPage(this IProductContentQueryBuilder builder, long itemsPerPage)
        {
            if (itemsPerPage < 1) itemsPerPage = 10;
            builder.ItemsPerPage = itemsPerPage;
            return builder;
        }

        /// <summary>
        /// Sets the ordering of the query result.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder OrderBy(this IProductContentQueryBuilder builder, ProductSortField sortBy, SortDirection sortDirection = SortDirection.Ascending)
        {
            builder.SortBy = sortBy;
            builder.SortDirection = sortDirection;
            return builder;
        }

        /// <summary>
        /// Sets the ordering of the query result.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="orderByExpression">
        /// The order by expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        public static IProductContentQueryBuilder OrderByCustomExpression(this IProductContentQueryBuilder builder, string orderByExpression, SortDirection sortDirection = SortDirection.Ascending)
        {
            builder.CustomOrderByExpression = orderByExpression;
            builder.SortDirection = sortDirection;
            return builder;
        }

        /// <summary>
        /// Sets a value indicating whether the query should be inclusive or exclusive with respect to the collection constraints.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="clusivity">
        /// The clusivity.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContentQueryBuilder"/>.
        /// </returns>
        /// <remarks>
        /// Only relevant in queries that are constrained by collections
        /// </remarks>
        public static IProductContentQueryBuilder Clusivity(this IProductContentQueryBuilder builder, CollectionClusivity clusivity)
        {
            builder.CollectionClusivity = clusivity;
            return builder;
        }
    }
}