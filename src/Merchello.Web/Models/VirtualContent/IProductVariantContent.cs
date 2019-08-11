namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core.Models;

    /// <summary>
    /// Defines ProductVariantContent
    /// </summary>
    public interface IProductVariantContent : IProductContentBase, IPublishedContent
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Gets the product key.
        /// </summary>
        Guid ProductKey { get; }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        IEnumerable<ProductAttributeDisplay> Attributes { get; }

        /// <summary>
        /// The has property.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the property exists.
        /// </returns>
        /// <remarks>
        /// This is a hack to override Umbraco's static extension method that requires
        /// that a content type be present.  For ProductVariantContent this may not always be true.  For example 
        /// content is defined for the containing ProductContent but some or all variants have not been extended.
        /// </remarks>
        bool HasProperty(string alias);

        /// <summary>
        /// Gets the default variant to display
        /// </summary>
        bool IsDefault { get; }
    }
}