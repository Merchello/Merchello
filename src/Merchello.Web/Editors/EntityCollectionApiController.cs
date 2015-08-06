namespace Merchello.Web.Editors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
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
        /// The static collection providers.
        /// </summary>
        private readonly IList<EntityCollectionProviderAttribute> _staticProviderAtts = new List<EntityCollectionProviderAttribute>();

        /// <summary>
        /// The <see cref="EntityCollectionProviderResolver"/>.
        /// </summary>
        private readonly EntityCollectionProviderResolver _resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionApiController"/> class.
        /// </summary>
        public EntityCollectionApiController()
            : this(Core.MerchelloContext.Current)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public EntityCollectionApiController(IMerchelloContext merchelloContext)
            : this(merchelloContext, UmbracoContext.Current)
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
            
            _resolver = EntityCollectionProviderResolver.Current;

            this.Initialize();
        }

        /// <summary>
        /// Gets an entity collection by its key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionDisplay"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the collection is not found
        /// </exception>
        [HttpGet]
        public EntityCollectionDisplay GetByKey(Guid key)
        {
            var collection = _entityCollectionService.GetByKey(key);
            if (collection == null) throw new NullReferenceException("EntityCollection does not exist");

            return collection.ToEntityCollectionDisplay();
        }

        /// <summary>
        /// The get entity collection providers.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityCollectionProviderDisplay> GetDefaultEntityCollectionProviders()
        {
            var providers = _staticProviderAtts.Select(x => x.ToEntityCollectionProviderDisplay());
            return providers;
        }

        /// <summary>
        /// The get entity collection providers.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityCollectionProviderDisplay> GetEntityCollectionProviders()
        {
            return _resolver.GetProviderAttributes().Select(x => x.ToEntityCollectionProviderDisplay());
        }

        /// <summary>
        /// Gets all entity collections with the type field specified.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityCollectionDisplay> GetEntityCollections(Guid entityTfKey)
        {
            var collections = _entityCollectionService.GetByEntityTfKey(entityTfKey);
            throw new NotImplementedException();
        }

        /// <summary>
        /// The post add entity collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionDisplay"/>.
        /// </returns>
        [HttpPost]
        public EntityCollectionDisplay PostAddEntityCollection(EntityCollectionDisplay collection)
        {
            var ec = _entityCollectionService.CreateEntityCollectionWithKey(
                collection.EntityType,
                collection.ProviderKey,
                collection.Name);
            if (collection.ParentKey != null)
            {
                ec.ParentKey = collection.ParentKey;
            }

            _entityCollectionService.Save(ec);

            return ec.ToEntityCollectionDisplay();
        }

        /// <summary>
        /// The delete entity collection.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost, HttpDelete, HttpGet]
        public HttpResponseMessage DeleteEntityCollection(Guid key)
        {
            var collectionToDelete = _entityCollectionService.GetByKey(key);
            if (collectionToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            _entityCollectionService.Delete(collectionToDelete);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void Initialize()
        {
            _staticProviderAtts.Add(_resolver.GetProviderAttribute<StaticCustomerCollectionProvider>());
            _staticProviderAtts.Add(_resolver.GetProviderAttribute<StaticInvoiceCollectionProvider>());
            _staticProviderAtts.Add(_resolver.GetProviderAttribute<StaticProductCollectionProvider>());            
        }
    }
}