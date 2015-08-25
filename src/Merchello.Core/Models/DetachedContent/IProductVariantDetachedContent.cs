namespace Merchello.Core.Models.DetachedContent
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a ProductVariantDetachedContent.
    /// </summary>
    internal interface IProductVariantDetachedContent : IDetachedContent
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
    }
}