namespace Merchello.Core.Models.DetachedContent
{
    using System;

    /// <summary>
    /// Represents product variant detached content.
    /// </summary>
    public interface IProductVariantDetachedContent : IDetachedContent
    {
        /// <summary>
        /// Gets the product variant key.
        /// </summary>
        Guid ProductVariantKey { get; }        

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        int? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        string Slug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether virtual content can be rendered.
        /// </summary>
        bool CanBeRendered { get; set; }
    }
}