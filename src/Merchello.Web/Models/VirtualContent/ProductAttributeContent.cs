namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Models.ContentEditing.Content;

    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web.Models;

    /// <summary>
    /// Represents a product attribute extended with Umbraco content.
    /// </summary>
    internal class ProductAttributeContent : PublishedContentBase, IProductAttributeContent
    {
        /// <summary>
        /// The <see cref="ProductAttributeDisplay"/>.
        /// </summary>
        private readonly ProductAttributeDisplay _display;

        /// <summary>
        /// The content type.
        /// </summary>
        private readonly PublishedContentType _contentType;

        /// <summary>
        /// The parent.
        /// </summary>
        private readonly IPublishedContent _parent;

        /// <summary>
        /// A value indicating that this is preview mode.
        /// </summary>
        /// <remarks>
        /// Not used in the current implementation
        /// </remarks>
        private readonly bool _isForPreview;

        /// <summary>
        /// The properties.
        /// </summary>
        private Lazy<IEnumerable<IPublishedProperty>> _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductAttributeContent"/> class.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="parent">
        /// The parent content - generally set to the <see cref="IProductContent"/> containing the option
        /// </param>
        /// <param name="isPreviewing">
        /// The is previewing.
        /// </param>
        public ProductAttributeContent(
            PublishedContentType contentType,
            ProductAttributeDisplay display,
            IPublishedContent parent = null,
            bool isPreviewing = false)
        {
            Mandate.ParameterNotNull(display, "display");
            _contentType = contentType;
            _display = display;
            _parent = parent;
            _isForPreview = isPreviewing;

            this.Initialize();
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
        public override string Name
        {
            get
            {
                return _display.Name;
            }
        }

        /// <summary>
        /// Gets the option key.
        /// </summary>
        public Guid OptionKey
        {
            get
            {
                return _display.OptionKey;
            }
        }

        /// <summary>
        /// Gets the SKU.
        /// </summary>
        public string Sku
        {
            get
            {
                return _display.Sku;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is default choice.
        /// </summary>
        public bool IsDefaultChoice
        {
            get
            {
                return _display.IsDefaultChoice;
            }
        }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        public override int SortOrder
        {
            get
            {
                return _display.SortOrder;
            }
        }

        /// <summary>
        /// Gets the item type.
        /// </summary>
        public override PublishedItemType ItemType
        {
            get
            {
                return PublishedItemType.Content;
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
        /// Gets the children.
        /// </summary>
        public override IEnumerable<IPublishedContent> Children
        {
            get
            {
                return Enumerable.Empty<IPublishedContent>();
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public override ICollection<IPublishedProperty> Properties
        {
            get
            {
                return _properties.Value.ToArray();
            }
        }

        /// <summary>
        /// Gets the content type.
        /// </summary>
        public override PublishedContentType ContentType
        {
            get
            {
                return _contentType;
            }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public override int Id
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the template id.
        /// </summary>
        public override int TemplateId
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the url name.
        /// </summary>
        public override string UrlName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the document type alias.
        /// </summary>
        public override string DocumentTypeAlias
        {
            get
            {
                return _contentType != null ? _contentType.Alias : null;
            }
        }

        /// <summary>
        /// Gets the document type id.
        /// </summary>
        public override int DocumentTypeId
        {
            get
            {
                return _contentType != null ? _contentType.Id : 0;
            }
        }

        /// <summary>
        /// Gets the writer name.
        /// </summary>
        public override string WriterName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the creator name.
        /// </summary>
        public override string CreatorName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the writer id.
        /// </summary>
        public override int WriterId
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the creator id.
        /// </summary>
        public override int CreatorId
        {
            get
            {
                return 0;
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
        /// Gets the create date.
        /// </summary>
        public override DateTime CreateDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Gets the update date.
        /// </summary>
        public override DateTime UpdateDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public override Guid Version
        {
            get
            {
                return _display.Key;
            }
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        public override int Level
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether uses override default.
        /// </summary>
        internal bool UsesOverrideDefault { get; set; }

        /// <summary>
        /// Gets a property by it's alias.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedProperty"/>.
        /// </returns>
        public override IPublishedProperty GetProperty(string alias)
        {
            return _properties.Value.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }

        /// <summary>
        /// The get property.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="recurse">
        /// The recurse.
        /// </param>
        /// <returns>
        /// The <see cref="IPublishedProperty"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Throws an exception if the parent is not set
        /// </exception>
        public override IPublishedProperty GetProperty(string alias, bool recurse)
        {
            if (recurse && Parent == null)
                throw new NotSupportedException("Parent must be set in order to recurse");

            var prop = GetProperty(alias);
            return prop == null && recurse ?
                Parent.GetProperty(alias, true)
                : prop;
        }

        /// <summary>
        /// The build properties.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IPublishedProperty}"/>.
        /// </returns>
        private IEnumerable<IPublishedProperty> BuildProperties()
        {
            var propDictionary = new List<IPublishedProperty>();
            _display.EnsureAttributeDetachedDataValues();
            if (!_display.DetachedDataValues.Any() || _contentType == null) return propDictionary;

           
            propDictionary.AddRange(_display.DataValuesAsPublishedProperties(_contentType));
            return propDictionary;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            _properties = new Lazy<IEnumerable<IPublishedProperty>>(this.BuildProperties);
        }
    }
}