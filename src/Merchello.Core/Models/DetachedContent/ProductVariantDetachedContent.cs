namespace Merchello.Core.Models.DetachedContent
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductVariantDetachedContent : Entity, IProductVariantDetachedContent
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The template id.
        /// </summary>
        private int? _templateId;

        /// <summary>
        /// The slug.
        /// </summary>
        private string _slug;

        /// <summary>
        /// A value indicating whether or not we should all the virtual content to render.
        /// </summary>
        private bool _canBeRendered;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantDetachedContent"/> class.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <param name="detachedContentType">
        /// The detached content type.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        public ProductVariantDetachedContent(Guid productVariantKey, IDetachedContentType detachedContentType, string cultureName)
            : this(productVariantKey, detachedContentType, cultureName, new DetachedDataValuesCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductVariantDetachedContent"/> class.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <param name="detachedContentType">
        /// The detached content type.
        /// </param>
        /// <param name="cultureName">
        /// The culture name.
        /// </param>
        /// <param name="detachedDataValuesCollection">
        /// The detached data values collection.
        /// </param>
        public ProductVariantDetachedContent(
            Guid productVariantKey,
            IDetachedContentType detachedContentType,
            string cultureName,
            DetachedDataValuesCollection detachedDataValuesCollection)
        {
            Ensure.ParameterCondition(!Guid.Empty.Equals(productVariantKey), "productVariantKey");
            Ensure.ParameterNotNull(detachedContentType, "detachedContentType");
            Ensure.ParameterNotNullOrEmpty(cultureName, "cultureName");

            this.ProductVariantKey = productVariantKey;
            this.DetachedContentType = detachedContentType;
            this.CultureName = cultureName;
            this.DetachedDataValues = detachedDataValuesCollection;
        }


        /// <inheritdoc/>
        [DataMember]
        public IDetachedContentType DetachedContentType { get; private set; }

        /// <inheritdoc/>
        [DataMember]
        public string CultureName { get; private set; }

        /// <inheritdoc/>
        [DataMember]
        public DetachedDataValuesCollection DetachedDataValues { get; private set; }

        /// <inheritdoc/>
        [DataMember]
        public Guid ProductVariantKey { get; private set; }

        /// <inheritdoc/>
        [DataMember]
        public int? TemplateId
        {
            get
            {
                return this._templateId;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref _templateId, _ps.Value.TemplateIdSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Slug
        {
            get
            {
                return this._slug;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref _slug, _ps.Value.SlugSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool CanBeRendered
        {
            get
            {
                return this._canBeRendered;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref _canBeRendered, _ps.Value.CanBeRenderedSelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The template id selector.
            /// </summary>
            public readonly PropertyInfo TemplateIdSelector = ExpressionHelper.GetPropertyInfo<ProductVariantDetachedContent, int?>(x => x.TemplateId);

            /// <summary>
            /// The slug selector.
            /// </summary>
            public readonly PropertyInfo SlugSelector = ExpressionHelper.GetPropertyInfo<ProductVariantDetachedContent, string>(x => x.Slug);

            /// <summary>
            /// The can be rendered selector.
            /// </summary>
            public readonly PropertyInfo CanBeRenderedSelector = ExpressionHelper.GetPropertyInfo<ProductVariantDetachedContent, bool>(x => x.CanBeRendered);
        }
    }
}