namespace Merchello.Web.Models.VirtualContent
{
    using System;

    using Umbraco.Core.Models;

    /// <summary>
    /// Defines a product attribute with extended content.
    /// </summary>
    public interface IProductAttributeContent : IPublishedContent
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Gets the option key.
        /// </summary>
        Guid OptionKey { get; }

        /// <summary>
        /// Gets the SKU.
        /// </summary>
        string Sku { get; }

        /// <summary>
        /// Gets a value indicating whether is default choice.
        /// </summary>
        bool IsDefaultChoice { get;  }
    }
}