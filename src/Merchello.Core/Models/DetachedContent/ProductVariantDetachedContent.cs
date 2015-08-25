namespace Merchello.Core.Models.DetachedContent
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    using Umbraco.Core;

    /// <summary>
    /// The product variant detached content.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class ProductVariantDetachedContent : Entity, IProductVariantDetachedContent
    {
        /// <summary>
        /// The template id selector.
        /// </summary>
        private static readonly PropertyInfo TemplateIdSelector = ExpressionHelper.GetPropertyInfo<ProductVariantDetachedContent, int?>(x => x.TemplateId);

        /// <summary>
        /// The slug selector.
        /// </summary>
        private static readonly PropertyInfo SlugSelector = ExpressionHelper.GetPropertyInfo<ProductVariantDetachedContent, string>(x => x.Slug);

        /// <summary>
        /// The template id.
        /// </summary>
        private int? _templateId;

        /// <summary>
        /// The slug.
        /// </summary>
        private string _slug;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantDetachedContent"/> class.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        internal ProductVariantDetachedContent(Guid productVariantKey, Guid detachedContentTypeKey, string cultureName)
            : this(productVariantKey, detachedContentTypeKey, cultureName, new DetachedDataValuesCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantDetachedContent"/> class.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <param name="detachedDataValuesCollection">
        /// The detached data values collection.
        /// </param>
        internal ProductVariantDetachedContent(
            Guid productVariantKey,
            Guid detachedContentTypeKey,
            string cultureName,
            DetachedDataValuesCollection detachedDataValuesCollection)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(productVariantKey), "productVariantKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(detachedContentTypeKey), "detachedContentTypeKey");
            Mandate.ParameterNotNullOrEmpty(cultureName, "cultureName");

            this.ProductVariantKey = productVariantKey;
            this.DetachedContentTypeKey = detachedContentTypeKey;
            this.CultureName = cultureName;
            this.DetachedDataValues = detachedDataValuesCollection;
        }

        /// <summary>
        /// Gets the detached content type key.
        /// </summary>
        [DataMember]
        public Guid DetachedContentTypeKey { get; private set; }

        /// <summary>
        /// Gets the culture name.
        /// </summary>
        [DataMember]
        public string CultureName { get; private set; }

        /// <summary>
        /// Gets the <see cref="DetachedDataValuesCollection"/>.
        /// </summary>
        [DataMember]
        public DetachedDataValuesCollection DetachedDataValues { get; private set; }

        /// <summary>
        /// Gets the product variant key.
        /// </summary>
        [DataMember]
        public Guid ProductVariantKey { get; private set; }

        /// <summary>
        /// Gets or sets the template id.
        /// </summary>
        [DataMember]
        public int? TemplateId
        {
            get
            {
                return this._templateId;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        this._templateId = value;
                        return this._templateId;
                    },
                    this._templateId,
                    TemplateIdSelector);
            }
        }

        /// <summary>
        /// Gets or sets the slug.
        /// </summary>
        [DataMember]
        public string Slug
        {
            get
            {
                return this._slug;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        this._slug = value;
                        return this._slug;
                    },
                    this._slug,
                    SlugSelector);
            }
        }
    }
}