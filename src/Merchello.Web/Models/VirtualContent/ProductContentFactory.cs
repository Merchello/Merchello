namespace Merchello.Web.Models.VirtualContent
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;

    using umbraco.cms.businesslogic.web;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web;
    using Umbraco.Web.Routing;

    using Constants = Merchello.Core.Constants;

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
        /// The parent.
        /// </summary>
        private IPublishedContent _parent;

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
        {
            _storeSettingService = storeSettingService;

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

            return new ProductContent(publishedContentType, display, _parent, _defaultStoreLanguage);
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

            _defaultStoreLanguage = _parentCulture.IsNullOrWhiteSpace() ?
                _storeSettingService.GetByKey(Constants.StoreSettingKeys.DefaultExtendedContentCulture).Value :
                _parentCulture;

            if (_allLanguages.Any())
            {

                _defaultStoreLanguage = _allLanguages.Any(x => x.CultureInfo.Name == _defaultStoreLanguage)
                                            ? _defaultStoreLanguage
                                            : _allLanguages.First().CultureInfo.Name;
            }

        }
    }
}