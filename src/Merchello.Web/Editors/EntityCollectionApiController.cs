namespace Merchello.Web.Editors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.EntityCollections.Providers;
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
        /// Gets the root level entity collections with the type field specified.
        /// </summary>
        /// <param name="entityType">
        /// The entity Type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityCollectionDisplay> GetRootEntityCollections(EntityType entityType)
        {
            var collections = _entityCollectionService.GetRootLevelEntityCollections(entityType);
            return collections.Select(x => x.ToEntityCollectionDisplay()).OrderBy(x => x.SortOrder);
        }

        /// <summary>
        /// The get child entity collections.
        /// </summary>
        /// <param name="parentKey">
        /// The parent key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityCollectionDisplay> GetChildEntityCollections(Guid parentKey)
        {
            var collections = _entityCollectionService.GetChildren(parentKey);
            return collections.Select(x => x.ToEntityCollectionDisplay()).OrderBy(x => x.SortOrder);
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
            return collections.Select(x => x.ToEntityCollectionDisplay()).OrderBy(x => x.SortOrder);
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
            var ec = _entityCollectionService.CreateEntityCollection(
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
        /// Updates sort orders.
        /// </summary>
        /// <param name="collections">
        /// The collections.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [HttpPut, HttpPost]
        public IEnumerable<EntityCollectionDisplay> PutUpdateSortOrders(IEnumerable<EntityCollectionDisplay> collections)
        {
            var collectionsArray = collections.ToArray();
            if (!collectionsArray.Any()) return Enumerable.Empty<EntityCollectionDisplay>();

            var first = collectionsArray.FirstOrDefault();
            if (first != null)
            {
                var parentKey = first.ParentKey;
                var existing = parentKey == null
                                   ? _entityCollectionService.GetRootLevelEntityCollections(first.EntityType)
                                   : _entityCollectionService.GetChildren(parentKey.Value);

                var updates =
                    collectionsArray.Where(x => existing.Any(y => y.SortOrder != x.SortOrder && y.Key == x.Key));
                    

                _entityCollectionService.Save(updates.Select(x => x.ToEntityCollection(existing.FirstOrDefault(y => y.Key == x.Key))));
            }
         

            return collectionsArray;
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