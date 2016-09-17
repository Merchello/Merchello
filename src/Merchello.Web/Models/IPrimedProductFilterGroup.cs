namespace Merchello.Web.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a product filter that includes pre queried counts.
    /// </summary>
    public interface IPrimedProductFilterGroup : IEntityCollectionProxy
    { 
        /// <inheritdoc/>
        IEnumerable<IPrimedProductFilter> Filters { get; set; }

        /// <summary>
        /// Gets or sets the count of items in the filter group would matches in the current context.
        /// </summary>
        int Count { get; set; }
    }
}