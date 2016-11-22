namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;

    /// <summary>
    /// Represents a product option used in IProductContent.
    /// </summary>
    internal class ProductOptionWrapper : IProductOptionWrapper
    {
        /// <summary>
        /// The option display object.
        /// </summary>
        private readonly ProductOptionDisplay _display;

        /// <summary>
        /// The <see cref="PublishedContentType"/>.
        /// </summary>
        private readonly PublishedContentType _contentType;

        /// <summary>
        /// The choices.
        /// </summary>
        private IEnumerable<IProductAttributeContent> _choices;

        private bool _usesContentTypeOverride;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOptionWrapper"/> class.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="_parent">
        /// The parent content.
        /// </param>
        /// <param name="contentType">
        /// The content Type.
        /// </param>
        /// <param name="usesContentTypeOverride">
        /// The uses Content Type Override.
        /// </param>
        public ProductOptionWrapper(ProductOptionDisplay display, IPublishedContent _parent, PublishedContentType contentType = null, bool usesContentTypeOverride = false)
        {
            _display = display;
            _contentType = contentType;
            _usesContentTypeOverride = usesContentTypeOverride;

            Initialize(_parent);
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Guid Key
        {
            get
            {
                return _display.Key;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return _display.Name;
            }
        }

        /// <summary>
        /// Gets the use name.
        /// </summary>
        public string UseName
        {
            get
            {
                return _display.UseName;
            }
        }

        /// <summary>
        /// Gets the UI option.
        /// </summary>
        public string UiOption
        {
            get
            {
                return _display.UiOption;
            }
        }

        /// <summary>
        /// Gets a value indicating whether required.
        /// </summary>
        public bool Required
        {
            get
            {
                return _display.Required;
            }
        }

        /// <summary>
        /// Gets the detached content type key.
        /// </summary>
        public Guid DetachedContentTypeKey
        {
            get
            {
                return _display.DetachedContentTypeKey;
            }
        }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public int SortOrder
        {
            get
            {
                return _display.SortOrder;
            }
        }

        /// <summary>
        /// Gets the choices.
        /// </summary>
        public IEnumerable<IProductAttributeContent> Choices
        {
            get
            {
                return _choices.OrderBy(x => x.SortOrder);
            }
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        private void Initialize(IPublishedContent parent)
        {
            var ct = _contentType ?? parent.ContentType;
            _choices = _display.Choices.Select(choice => new ProductAttributeContent(ct, choice, parent) { UsesOverrideDefault = _usesContentTypeOverride });
        }
    }
}