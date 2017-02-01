namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;
    using Merchello.Web.Models.Ui.Rendering;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Defines a CmsContentQueryBuilder.
    /// </summary>
    /// <typeparam name="TCollectionProxy">
    /// The type of the collection proxy
    /// </typeparam>
    /// <typeparam name="TFilterProxy">
    /// The type of the filter proxy
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result to be paged
    /// </typeparam>
    public interface ICmsContentQueryBuilder<in TCollectionProxy, in TFilterProxy, TResult> : ICmsContentQueryBuilder
        where TCollectionProxy : IEntityProxy
        where TFilterProxy : IEntityProxy
        where TResult : ICmsContent
    {
        /// <summary>
        /// Adds a  parameter.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        void AddConstraint(TCollectionProxy collection);

        /// <summary>
        /// Adds a collection of parameters.
        /// </summary>
        /// <param name="collections">
        /// The collections.
        /// </param>
        void AddConstraint(IEnumerable<TCollectionProxy> collections);

        /// <summary>
        /// Adds a parameter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        void AddConstraint(TFilterProxy filter);

        /// <summary>
        /// Adds a collection of filter parameters.
        /// </summary>
        /// <param name="filters">
        /// The filters.
        /// </param>
        void AddConstraint(IEnumerable<TFilterProxy> filters);


        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<TResult> Execute();
    }


    /// <summary>
    /// Defines a CmsContentQueryBuilder.
    /// </summary>
    public interface ICmsContentQueryBuilder
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
        /// Gets or sets the search term.
        /// </summary>
        string SearchTerm { get; set; }

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
        CollectionClusivity CollectionClusivity { get; set; }

        /// <summary>
        /// Adds a search term parameter.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        void AddConstraint(string searchTerm);

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
    }
}