namespace Merchello.Web.Models.Querying
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// A wrapper to return query results
    /// </summary>
    public class QueryResultDisplay
    {
        /// <summary>
        /// Gets or sets the current page index
        /// </summary>
        public long CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public long ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public long TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the number of total results in returned by the query.
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the results to be serialized
        /// </summary>
        public IEnumerable<object> Items { get; set; }
    }
}