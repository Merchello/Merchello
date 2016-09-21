namespace Merchello.Web.Models.Usage
{
    /// <summary>
    /// Represents the usage of a product filter in terms of the number of results returned with respect to a filter query.
    /// </summary>
    internal interface IProductFilterResultsCount : IFilterResultsCount
    {
        /// <summary>
        /// Gets the product filter related to result count.
        /// </summary>
        IProductFilter Filter { get; }
    }
}