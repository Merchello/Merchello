namespace Merchello.Web.Editors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Web.Models.ContentEditing.Content;
    using Merchello.Web.WebApi;

    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// An API controller for handling detached content related operations in the Merchello back office.
    /// </summary>
    [PluginController("Merchello")]
    public class DetachedContentApiController : MerchelloApiController
    {
        /// <summary>
        /// Umbraco's <see cref="IContentTypeService"/>.
        /// </summary>
        private readonly IContentTypeService _contentTypeService;

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
        }

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
                    .OrderBy(x => x.SortOrder)
                    .Select(x => x.ToEmbeddedContentTypeDisplay());
        }

        #endregion
    }
}