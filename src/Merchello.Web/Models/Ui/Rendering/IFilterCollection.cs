namespace Merchello.Web.Models.Ui.Rendering
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a product filter group
    /// </summary>
    public interface IProductFilterGroup : IEntityProxy
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// Gets the product filters.
        /// </summary>
        IEnumerable<IProductFilter> Filters { get; }
    }
}
