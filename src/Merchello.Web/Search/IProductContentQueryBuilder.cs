namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Defines a query that can be used to filter products.
    /// </summary>
    public interface IProductContentQueryBuilder : ICacheQueryBuilder
    {
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        long Page { get; set; }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        long ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        ProductSortField SortBy { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        SortDirection SortDirection { get; set; }

        /// <summary>
        /// Gets or sets a setting for specifying how the query should treat collection clusivity in specified collections and filters.
        /// </summary>
        FilterQueryClusivity FilterQueryClusivity { get; set; }

        /// <summary>
        /// Adds a search term parameter.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        void AddConstraint(string searchTerm);

        /// <summary>
        /// Adds a <see cref="IProductCollection"/> parameter.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        void AddConstraint(IProductCollection collection);

        /// <summary>
        /// Adds a collection of <see cref="IProductCollection"/> parameters.
        /// </summary>
        /// <param name="collections">
        /// The collections.
        /// </param>
        void AddConstraint(IEnumerable<IProductCollection> collections);

        /// <summary>
        /// Adds a <see cref="IProductFilter"/> parameter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        void AddConstraint(IProductFilter filter);

        /// <summary>
        /// Adds a collection of <see cref="IProductFilter"/> parameters.
        /// </summary>
        /// <param name="filters">
        /// The filters.
        /// </param>
        void AddConstraint(IEnumerable<IProductFilter> filters);

        /// <summary>
        /// Allows for directly adding <see cref="IEntityCollection"/> keys.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        void AddCollectionKeyConstraint(Guid key);

        /// <summary>
        /// Allows for directly adding a collection of <see cref="IEntityCollection"/> keys.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        void AddCollectionKeyConstraint(IEnumerable<Guid> keys);

        /// <summary>
        /// Resets the builder to defaults
        /// </summary>
        void Reset();

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<IProductContent> Execute();
    }
}