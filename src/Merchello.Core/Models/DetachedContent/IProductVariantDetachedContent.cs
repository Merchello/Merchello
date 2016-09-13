namespace Merchello.Core.Models.DetachedContent
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents product variant detached content.
    /// </summary>
    public interface IProductVariantDetachedContent : IDetachedContent
    {
        /// <summary>
        /// Gets the product variant key.
        /// </summary>
        [DataMember]
        Guid ProductVariantKey { get; }        

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        [DataMember]
        int? TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        [DataMember]
        string Slug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether virtual content can be rendered.
        /// </summary>
        [DataMember]
        bool CanBeRendered { get; set; }
    }
}