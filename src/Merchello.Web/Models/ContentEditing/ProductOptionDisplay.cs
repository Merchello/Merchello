namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    /// <summary>
    /// The product option display.
    /// </summary>
    public class ProductOptionDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the UI element.
        /// </summary>
        public string UiElement { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the option is shared option.
        /// </summary>
        public bool Shared { get; set; }

        /// <summary>
        /// Gets or sets the share count.
        /// </summary>
        /// <remarks>
        /// This is a calculated field and never saved if set via Angular
        /// </remarks>
        public int ShareCount { get; set; }

        /// <summary>
        /// Gets or sets the detached content key.
        /// </summary>
        public Guid DetachedContentKey { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        public IEnumerable<ProductAttributeDisplay> Choices { get; set; }
    }
}
