namespace Merchello.Web.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a group of product filters
    /// </summary>
    public interface IProductFilterGroup : IEntityCollectionProxy
    {
        /// <summary>
        /// Gets the product filters.
        /// </summary>
        IEnumerable<IProductFilter> Filters { get; }
    }
}
