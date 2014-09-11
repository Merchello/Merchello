using Merchello.Web.Search;

namespace Merchello.Web.Models.Querying
{
    using System.Collections.Generic;

    using Merchello.Core.Persistence.Querying;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The query display.
    /// </summary>
    public class QueryDisplay
    {       
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public IEnumerable<QueryDisplayParameter> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SortDirection SortDirection { get; set; } 
    }
}