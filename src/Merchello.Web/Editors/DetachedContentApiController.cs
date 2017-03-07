namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.WebApi;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    using ServiceContext = Merchello.Core.Services.ServiceContext;

    /// <summary>
    /// An API controller for handling detached content related operations in the Merchello back office.
    /// </summary>
    [PluginController("Merchello")]
    public sealed class DetachedContentApiController : MerchelloApiController
    {
        /// <summary>
        /// Umbraco's <see cref="IContentTypeService"/>.
        /// </summary>
        private readonly IContentTypeService _contentTypeService;

        /// <summary>
        /// The <see cref="IDetachedContentTypeService"/>.
        /// </summary>
        private readonly IDetachedContentTypeService _detachedContentTypeService;

        /// <summary>
        /// The collection of all languages.
        /// </summary>
        private Lazy<ILanguage[]> _allLanguages;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentApiController"/> class.
        /// </summary>
        public DetachedContentApiController()
            : this(Core.MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        public DetachedContentApiController(IMerchelloContext merchelloContext)
            : this(merchelloContext, UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>.
        /// </param>
        public DetachedContentApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            if (ApplicationContext == null) throw new NotFiniteNumberException("Umbraco ApplicationContext is null");

            _contentTypeService = ApplicationContext.Services.ContentTypeService;

            _detachedContentTypeService = ((ServiceContext)merchelloContext.Services).DetachedContentTypeService;

            this.Initialize();
        }

        #region Localization

        /// <summary>
        /// Gets all languages.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Language}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<object> GetAllLanguages()
        {
            return _allLanguages.Value
                .Select(x => CultureInfo.GetCultureInfo(x.IsoCode))
                .Select(x => new 
                            {
                                IsoCode = x.Name,
                                Name = x.DisplayName, 
                                x.NativeName
                            });
        }

        #endregion

        #region ContentTypes

        /// <summary>
        /// The get content types.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{UmbContentTypeDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<UmbContentTypeDisplay> GetContentTypes()
        {
            return
                _contentTypeService.GetAllContentTypes()
                    .OrderBy(x => x.Name)
                    .Select(x => x.ToUmbContentTypeDisplay());
        }

        /// <summary>
        /// Gets a collection of <see cref="DetachedContentTypeDisplay"/> by entity type.
        /// </summary>
        /// <param name="enumValue">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{DetachedContentTypeDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<DetachedContentTypeDisplay> GetDetachedContentTypesByEntityType(string enumValue)
        {
            EntityType entityType;
            if (!Enum.TryParse(enumValue, true, out entityType))
            {
                LogHelper.Debug<DetachedContentApiController>("Failed to parse enum value");
                return Enumerable.Empty<DetachedContentTypeDisplay>();
            }

            var entityTfKey = EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey;

            var list = _detachedContentTypeService.GetDetachedContentTypesByEntityTfKey(entityTfKey).OrderBy(x => x.Name);
            
            return list
                .Where(x => x.Key != Core.Constants.DetachedPublishedContentType.DefaultProductVariantDetachedPublishedContentTypeKey)
                .Select(x => x.ToDetachedContentTypeDisplay());
        }

            /// <summary>
        /// The post add content type.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public DetachedContentTypeDisplay PostAddDetachedContentType(DetachedContentTypeDisplay contentType)
        {            
            var detachedContentType = _detachedContentTypeService.CreateDetachedContentType(
                contentType.EntityType,
                contentType.UmbContentType.Key,
                contentType.Name);

                detachedContentType.Description = contentType.Description;

            _detachedContentTypeService.Save(detachedContentType);

            var display = detachedContentType.ToDetachedContentTypeDisplay();

            return display;
        }

        /// <summary>
        /// The put save detached content type.
        /// </summary>
        /// <param name="contentType">
        /// The content type.
        /// </param>
        /// <returns>
        /// The <see cref="DetachedContentTypeDisplay"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the matching content type could not be found
        /// </exception>
        [HttpPut, HttpPost]
        public DetachedContentTypeDisplay PutSaveDetachedContentType(DetachedContentTypeDisplay contentType)
        {
            var destination = _detachedContentTypeService.GetByKey(contentType.Key);
            if (destination == null) throw new NullReferenceException("Existing DetachedContentType was not found");

            _detachedContentTypeService.Save(contentType.ToDetachedContentType(destination));

            return destination.ToDetachedContentTypeDisplay();
        }

        /// <summary>
        /// The delete detached content type.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpGet]
        public HttpResponseMessage DeleteDetachedContentType(Guid key)
        {
            var existing = _detachedContentTypeService.GetByKey(key);
            if (existing == null) return Request.CreateResponse(HttpStatusCode.NotFound);

            _detachedContentTypeService.Delete(existing);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #endregion



        /// <summary>
        /// Initializes the controller
        /// </summary>
        private void Initialize()
        {
            _allLanguages = new Lazy<ILanguage[]>(() => ApplicationContext.Current.Services.LocalizationService.GetAllLanguages().ToArray());
        }
    }
}