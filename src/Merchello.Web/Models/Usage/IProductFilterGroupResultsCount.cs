namespace Merchello.Web.Models.Usage
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the usage of a product filter in terms of the number of results returned with respect to a filter query.
    /// </summary>
    internal interface IProductFilterGroupResultsCount : IEntityCollectionProxy, IFilterResultsCount
    {
        /// <inheritdoc/>
        IEnumerator<IProductFilterResultsCount> Filters { get; }
    }
}