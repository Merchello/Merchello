namespace Merchello.Web.Models.Querying
{
    using System.Collections.Generic;

    /// <summary>
    /// A wrapper to return query results
    /// </summary>
    public class QueryResultDisplay
    {
        /// <summary>
        /// Gets or sets the current page index
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the number of total results in returned by the query.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the results to be serialized
        /// </summary>
        public IEnumerable<object> Items { get; set; }
    }
}