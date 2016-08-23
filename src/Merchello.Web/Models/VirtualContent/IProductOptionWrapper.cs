namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.ValueConverters;
    using Merchello.Web.Models.ContentEditing;

    using Newtonsoft.Json;

    /// <summary>
    /// Defines a product option with attribute content.
    /// </summary>
    public interface IProductOptionWrapper
    {
        /// <summary>
        /// Gets  the key.
        /// </summary>
         Guid Key { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
         string Name { get; }

        /// <summary>
        /// Gets the use name.
        /// </summary>
         string UseName { get; }

        /// <summary>
        /// Gets the UI option.
        /// </summary>
         string UiOption { get; }

        /// <summary>
        /// Gets a value indicating whether required.
        /// </summary>
         bool Required { get; }

        /// <summary>
        /// Gets the detached content key.
        /// </summary>
         Guid DetachedContentTypeKey { get; }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
         int SortOrder { get; }

        /// <summary>
        /// Gets the choices.
        /// </summary>
        IEnumerable<IProductAttributeContent> Choices { get; }
    }
}