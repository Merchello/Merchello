namespace Merchello.Web.Models.Usage
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the usage in terms of the number of results returned with respect to a filter.
    /// </summary>
    internal interface IFilterResultsCount
    {
        /// <summary>
        /// Gets the collection keys applied to the relevant.
        /// </summary>
        IEnumerable<Guid> CollectionKeys { get; }

        /// <summary>
        /// Gets the count of items in the filter set.
        /// </summary>
        int Count { get; }
    }
}