namespace Merchello.Web.Models
{
    using System.Collections.Generic;

    using Merchello.Web.Models.Ui.Rendering;

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
        /// Gets the information about the managing provider.
        /// </summary>
        IProviderMeta ProviderMeta { get; }

        /// <summary>
        /// Gets the product filters.
        /// </summary>
        IEnumerable<IProductFilter> Filters { get; }
    }
}
