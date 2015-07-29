namespace Merchello.Core.Models.Interfaces
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a ProductVariantDetachedContent.
    /// </summary>
    public interface IProductVariantDetachedContent : IEntity
    {
        /// <summary>
        /// Gets the product variant key.
        /// </summary>
        Guid ProductVariantKey { get; }

        /// <summary>
        /// Gets the detached content type key.
        /// </summary>
        Guid DetachedContentTypeKey { get; }

        /// <summary>
        /// Gets the culture name.
        /// </summary>
        string CultureName { get; }

        /// <summary>
        /// Gets or sets the detached content values.
        /// </summary>
        string DetachedContentValues { get; set; }

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        int? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        string Slug { get; set; }
    }
}