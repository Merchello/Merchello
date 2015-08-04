namespace Merchello.Web.Editors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing.Collections;
    using Merchello.Web.WebApi;

    using Umbraco.Core;
    using Umbraco.Web;

    /// <summary>
    /// The entity collection api controller.
    /// </summary>
    public class EntityCollectionApiController : MerchelloApiController
    {
        /// <summary>
        /// The <see cref="IEntityCollectionService"/>.
        /// </summary>
        private readonly IEntityCollectionService _entityCollectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionApiController"/> class.
        /// </summary>
        public EntityCollectionApiController()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public EntityCollectionApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        public EntityCollectionApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            _entityCollectionService = merchelloContext.Services.EntityCollectionService;
        }

        [HttpGet]
        public IEnumerable<EntityCollectionProviderDisplay> GetEntityCollectionProviders()
        {
            return Enumerable.Empty<EntityCollectionProviderDisplay>();
        } 

        public IEnumerable<EntityCollectionDisplay> GetEntityCollections(Guid entityTfKey)
        {
            var collections = _entityCollectionService.GetByEntityTfKey(entityTfKey);
            throw new NotImplementedException();
        }
    }
}