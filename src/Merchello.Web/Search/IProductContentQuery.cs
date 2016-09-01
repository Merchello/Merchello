namespace Merchello.Web.Search
{
    using System;

    using Merchello.Core.Persistence.Querying;
    using Merchello.Web.Models;

    /// <summary>
    /// Defines a product filter query.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type expected in the query result
    /// </typeparam>
    internal interface IProductContentQuery<TResult>
    {
        /// <summary>
        /// Gets a value indicating whether has the query has a search term.
        /// </summary>
        bool HasSearchTerm { get; }

        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        long Page { get; set; }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        long ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the sort by field.
        /// </summary>
        string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        SortDirection SortDirection { get; set; }

        /// <summary>
        /// Gets or sets the collection keys.
        /// </summary>
        Guid[] CollectionKeys { get; set; }

        /// <summary>
        ///  Gets or sets a setting for specifying how the query should treat a products inclusion in specified collections and filters.
        /// </summary>
        FilterQueryClusivity FilterQueryClusivity { get; set; }

        /// <summary>
        /// Performs the query.
        /// </summary>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        PagedCollection<TResult> Execute();
    }
}