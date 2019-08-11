namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// Represents Product Variant Content.
    /// </summary>
    public class ProductVariantContent : ProductContentBase, IProductVariantContent
    {
        /// <summary>
        /// The parent <see cref="IProductContent"/>.
        /// </summary>
        private readonly IPublishedContent _parent;

        /// <summary>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </summary>
        private readonly ProductVariantDisplay _variant;

        /// <summary>
        /// Attribute content.
        /// </summary>
        private readonly IEnumerable<IProductAttributeContent> _attibutes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantContent"/> class.
        /// </summary>
        /// <param name="variant">
        /// The variant.
        /// </param>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="optionContentTypes">
        /// The option Content Types.
        /// </param>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public ProductVariantContent(ProductVariantDisplay variant, PublishedContentType contentType, IDictionary<Guid, PublishedContentType> optionContentTypes, IEnumerable<IProductAttributeContent> attributes, string cultureName = "en-US", IPublishedContent parent = null)
            : base(variant, contentType, optionContentTypes, cultureName)
        {
            _variant = variant;
            _attibutes = attributes;
            _parent = parent;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key
        {
            get
            {
                return _variant.Key;
            }
        }

        /// <summary>
        /// Gets the product key.
        /// </summary>
        public Guid ProductKey
        {
            get
            {
                return _variant.ProductKey;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return _variant.Name;
            }
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        public IEnumerable<ProductAttributeDisplay> Attributes
        {
            get
            {
                return _variant.Attributes;
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public override string Path
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the template id.
        /// </summary>
        /// <remarks>
        /// ProductVariantContent cannot be rendered
        /// </remarks>
        public override int TemplateId
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is draft.
        /// </summary>
        public override bool IsDraft
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the update date.
        /// </summary>
        public override DateTime UpdateDate
        {
            get
            {
                return _variant.UpdateDate;
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public override IPublishedContent Parent
        {
            get
            {
                return _parent;
            }
        }

        /// <summary>
        /// Gets the <see cref="IProductAttributeContent"/>.
        /// </summary>
        internal IEnumerable<IProductAttributeContent> AttributeContent
        {
            get
            {
                return _attibutes;
            }
        }

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
        public bool HasProperty(string alias)
        {
            if (ContentType == null) return false;

            return ContentType.GetPropertyType(alias) != null;
        }

        /// <summary>
        /// Is this the default variant to display
        /// </summary>
        public bool IsDefault {
            get
            {
                return _variant.IsDefault;
            }
        }

        /// <summary>
        /// Gets the product variant display.
        /// </summary>
        internal ProductVariantDisplay ProductVariantDisplay
        {
            get
            {
                return _variant;
            }
        }
    }
}