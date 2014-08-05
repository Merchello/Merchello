namespace Merchello.Web.Models.ContentEditing
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A wrapper to return query results
    /// </summary>
    public class QueryResultDisplay
    {
        /// <summary>
        /// Gets or sets the results to be serialized
        /// </summary>
        public IEnumerable<object> Results { get; set; }

        /// <summary>
        /// Gets or sets the current page index
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the number of total results in returned by the query.
        /// </summary>
        public int TotalResults { get; set; }
    }
}