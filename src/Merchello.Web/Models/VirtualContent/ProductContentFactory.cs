namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web;

    using Constants = Merchello.Core.Constants;
    using StringExtensions = Merchello.Core.StringExtensions;

    /// <summary>
    /// Represents a ProductContentFactory.
    /// </summary>
    public class ProductContentFactory : IProductContentFactory
    {
        /// <summary>
        /// The <see cref="IStoreSettingService"/>.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// The <see cref="IDetachedContentTypeService"/>.
        /// </summary>
        private readonly IDetachedContentTypeService _detachedContentTypeService;

        /// <summary>
        /// The parent.
        /// </summary>
        private IPublishedContent _parent;

        /// <summary>
        /// The parent culture.
        /// </summary>
        private string _parentCulture;

        /// <summary>
        /// The collection of all languages.
        /// </summary>
        private ILanguage[] _allLanguages;

        /// <summary>
        /// The default store language.
        /// </summary>
        private string _defaultStoreLanguage;

        /// <summary>
        /// The detached content types.
        /// </summary>
        private Lazy<IEnumerable<IDetachedContentType>> _detachedContentTypes;

        // private readonly VirtualProductContentCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentFactory"/> class.
        /// </summary>
        public ProductContentFactory()
            : this(MerchelloContext.Current.Services.StoreSettingService)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentFactory"/> class.
        /// </summary>
        /// <param name="storeSettingService">
        /// The <see cref="IStoreSettingService"/>.
        /// </param>
        internal ProductContentFactory(IStoreSettingService storeSettingService)
            : this(storeSettingService, MerchelloContext.Current.Services.DetachedContentTypeService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContentFactory"/> class.
        /// </summary>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        /// <param name="detachedContentTypeService">
        /// The detached content type service.
        /// </param>
        internal ProductContentFactory(IStoreSettingService storeSettingService, IDetachedContentTypeService detachedContentTypeService)
        {
            _storeSettingService = storeSettingService;
            _detachedContentTypeService = detachedContentTypeService;
            this.Initialize();
        }

        /// <summary>
        /// The initializing.
        /// </summary>
        public static event TypedEventHandler<ProductContentFactory, VirtualContentEventArgs> Initializing; 

        /// <summary>
        /// The build content.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <returns>
        /// The <see cref="IProductContent"/>.
        /// </returns>
        public IProductContent BuildContent(ProductDisplay display)
        {
            if (!display.DetachedContents.Any(x => x.CanBeRendered)) return null;

            // assert there is at least one the can be rendered
            var detachedContent = display.DetachedContents.FirstOrDefault(x => x.CanBeRendered);
            
            if (detachedContent == null) return null;

            var publishedContentType = PublishedContentType.Get(PublishedItemType.Content, detachedContent.DetachedContentType.UmbContentType.Alias);

            var optionContentTypes = GetProductOptionContentTypes(display);

            var clone = CloneHelper.JsonClone<ProductDisplay>(display);

            return new ProductContent(publishedContentType, optionContentTypes, clone, _parent, _defaultStoreLanguage);
        }

        /// <summary>
        /// Gets the collection of <see cref="PublishedContentType"/> associated with product options.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PublishedItemType}"/>.
        /// </returns>
        private IDictionary<Guid, PublishedContentType> GetProductOptionContentTypes(ProductDisplay display)
        {
            var keys =
                display.ProductOptions.Where(x => !x.DetachedContentTypeKey.Equals(Guid.Empty))
                    .Select(x => x.DetachedContentTypeKey)
                    .Distinct().ToArray();

            var publishedContentTypes = new Dictionary<Guid, PublishedContentType>();

            if (!keys.Any()) return publishedContentTypes;

            var contentTypeKeys = _detachedContentTypes.Value
                    .Where(x => keys.Any(y => y == x.Key)).Where(x => x.ContentTypeKey != null)
                    .Select(x => x.ContentTypeKey.Value);

            var contentTypes = ApplicationContext.Current.Services.ContentTypeService.GetAllContentTypes(contentTypeKeys);

            foreach (var ct in contentTypes)
            {
                var dct = _detachedContentTypes.Value.FirstOrDefault(x => x.ContentTypeKey != null && x.ContentTypeKey.Value == ct.Key);
                if (dct != null)
                publishedContentTypes.Add(dct.Key, PublishedContentType.Get(PublishedItemType.Content, ct.Alias));
            }

            return publishedContentTypes;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            var args = new VirtualContentEventArgs(_parent);
            Initializing.RaiseEvent(args, this);
            _parent = args.Parent;

            //// http://issues.merchello.com/youtrack/issue/M-878
            _allLanguages = ApplicationContext.Current.Services.LocalizationService.GetAllLanguages().ToArray();

            _parentCulture = _parent != null ? _parent.GetCulture().Name : string.Empty;

            _defaultStoreLanguage = StringExtensions.IsNullOrWhiteSpace(this._parentCulture) ?
                _storeSettingService.GetByKey(Constants.StoreSettingKeys.DefaultExtendedContentCulture).Value :
                _parentCulture;

            _detachedContentTypes = new Lazy<IEnumerable<IDetachedContentType>>(() => _detachedContentTypeService.GetAll().Where(x => x.ContentTypeKey != null));

            if (_allLanguages.Any())
            {

                _defaultStoreLanguage = _allLanguages.Any(x => x.CultureInfo.Name == _defaultStoreLanguage)
                                            ? _defaultStoreLanguage
                                            : _allLanguages.First().CultureInfo.Name;
            }

        }
    }
}