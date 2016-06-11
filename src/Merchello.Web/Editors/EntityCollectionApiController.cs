﻿namespace Merchello.Web.Editors
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
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing.Collections;
    using Merchello.Web.Models.Interfaces;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Search;
    using Merchello.Web.WebApi;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The entity collection api controller.
    /// </summary>
    [PluginController("Merchello")]
    public class EntityCollectionApiController : MerchelloApiController
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private readonly MerchelloHelper _merchello;

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

            _merchello = new MerchelloHelper(merchelloContext.Services, false);

            this.Initialize();
        }

        /// <summary>
        /// The get sortable provider keys.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        [HttpGet]
        public Guid[] GetSortableProviderKeys()
        {
            return new[]
                       {
                           _resolver.GetProviderKey<StaticProductCollectionProvider>(),
                           _resolver.GetProviderKey<StaticInvoiceCollectionProvider>(),
                           _resolver.GetProviderKey<StaticCustomerCollectionProvider>()
                       };
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
        /// The get entity collections by entity.
        /// </summary>
        /// <param name="model">
        /// The query parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionDisplay}"/>.
        /// </returns>
        [HttpPost]
        public IEnumerable<EntityCollectionDisplay> PostGetEntityCollectionsByEntity(EntityCollectionByEntityQuery model)
        {
            var empty = Enumerable.Empty<EntityCollectionDisplay>();
            
            var serviceContext = MerchelloContext.Services;

            switch (model.EntityType)
            {
                case EntityType.Product:

                    var product = serviceContext.ProductService.GetByKey(model.Key);

                    return product == null ? 
                        empty : 
                        product.GetCollectionsContaining().Select(x => x.ToEntityCollectionDisplay()).OrderBy(x => x.Name);

                case EntityType.Customer:
                    
                    var customer = serviceContext.CustomerService.GetByKey(model.Key);
                    return customer == null ? 
                        empty :
                        customer.GetCollectionsContaining().Select(x => x.ToEntityCollectionDisplay()).OrderBy(x => x.Name);
                                        
                case EntityType.Invoice:
                    var invoice = serviceContext.InvoiceService.GetByKey(model.Key);
                    return invoice == null
                               ? empty
                               : invoice.GetCollectionsContaining().Select(x => x.ToEntityCollectionDisplay()).OrderBy(x => x.Name);
            }

            return Enumerable.Empty<EntityCollectionDisplay>();
        }

            /// <summary>
        /// Gets the entities in the collection.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if either the collection key or entity type is not found in query parameters
        /// </exception>
        [HttpPost]
        public QueryResultDisplay PostGetCollectionEntities(QueryDisplay query)
        {
            var collectionKey = query.Parameters.FirstOrDefault(x => x.FieldName == "collectionKey");
            var entityTypeName = query.Parameters.FirstOrDefault(x => x.FieldName == "entityType");
            if (collectionKey == null || entityTypeName == null) throw new NullReferenceException("collectionKey and entityType must be included as a parameter");

            var key = new Guid(collectionKey.Value);

            var entityType = (EntityType)Enum.Parse(typeof(EntityType), entityTypeName.Value);

            var term = query.Parameters.FirstOrDefault(x => x.FieldName == "term");

            var cachedQuery = this.GetCachedQueryByEntityType(entityType);

            return term != null && !string.IsNullOrEmpty(term.Value)
              ?
               cachedQuery.GetFromCollection(
                  key,
                  term.Value,
                  query.CurrentPage + 1,
                  query.ItemsPerPage,
                  query.SortBy,
                  query.SortDirection)
              :
              cachedQuery.GetFromCollection(
                  key,
                  query.CurrentPage + 1,
                  query.ItemsPerPage,
                  query.SortBy,
                  query.SortDirection);
        }

        /// <summary>
        /// Gets entities not in the collection.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if either the collection key or entity type is not found in query parameters
        /// </exception>
        [HttpPost]
        public QueryResultDisplay PostGetEntitiesNotInCollection(QueryDisplay query)
        {
            var collectionKey = query.Parameters.FirstOrDefault(x => x.FieldName == "collectionKey");
            var entityTypeName = query.Parameters.FirstOrDefault(x => x.FieldName == "entityType");
            if (collectionKey == null || entityTypeName == null) throw new NullReferenceException("collectionKey and entityType must be included as a parameter");

            var key = new Guid(collectionKey.Value);

            var entityType = (EntityType)Enum.Parse(typeof(EntityType), entityTypeName.Value);

            var term = query.Parameters.FirstOrDefault(x => x.FieldName == "term");

            var cachedQuery = this.GetCachedQueryByEntityType(entityType);

            return term != null && !string.IsNullOrEmpty(term.Value)
              ?
               cachedQuery.GetNotInCollection(
                  key,
                  term.Value,
                  query.CurrentPage + 1,
                  query.ItemsPerPage,
                  query.SortBy,
                  query.SortDirection)
              :
              cachedQuery.GetNotInCollection(
                  key,
                  query.CurrentPage + 1,
                  query.ItemsPerPage,
                  query.SortBy,
                  query.SortDirection);
        }

        /// <summary>
        /// The post add entity to collections.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage PostAddEntityToCollections(IEnumerable<Entity2CollectionModel> model)
        {
            var status = Request.CreateResponse(HttpStatusCode.OK);

            var many2Many = model as Entity2CollectionModel[] ?? model.ToArray();
            if (!many2Many.Any()) return status;

            foreach (var item in many2Many)
            {
                status = this.PostAddEntityToCollection(item);
            }

            return status;
        }

        /// <summary>
        /// Adds an entity to a collection.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage PostAddEntityToCollection(Entity2CollectionModel model)
        {
            var collection = _entityCollectionService.GetByKey(model.CollectionKey);
            if (collection == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var provider = collection.ResolveProvider();
            var entityType = provider.EntityCollection.EntityType;
            switch (entityType)
            {
                case EntityType.Customer:
                    MerchelloContext.Services.CustomerService.AddToCollection(model.EntityKey, model.CollectionKey);
                    break;
                case EntityType.Invoice:
                    MerchelloContext.Services.InvoiceService.AddToCollection(model.EntityKey, model.CollectionKey);
                    break;
                case EntityType.Product:
                    MerchelloContext.Services.ProductService.AddToCollection(model.EntityKey, model.CollectionKey);
                    break;
                default:
                    var invalid =
                        new InvalidOperationException("Merchello service could not be found for the entity type");
                    MultiLogHelper.Error<EntityCollectionApiController>("An attempt was made to add an entity to a collection", invalid);
                    return Request.CreateResponse(HttpStatusCode.NotImplemented);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// The delete entity from collections.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpDelete, HttpPost]
        public HttpResponseMessage DeleteEntityFromCollections(IEnumerable<Entity2CollectionModel> model)
        {
            var status = Request.CreateResponse(HttpStatusCode.OK);

            var many2Many = model as Entity2CollectionModel[] ?? model.ToArray();
            if (!many2Many.Any()) return status;
            
            foreach (var item in many2Many)
            {
               status = this.DeleteEntityFromCollection(item);
            }

            return status;
        }

        /// <summary>
        /// Deletes an entity from a collection.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpDelete, HttpPost]
        public HttpResponseMessage DeleteEntityFromCollection(Entity2CollectionModel model)
        {
            var collection = _entityCollectionService.GetByKey(model.CollectionKey);
            if (collection == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var provider = collection.ResolveProvider();
            var entityType = provider.EntityCollection.EntityType;
            switch (entityType)
            {
                case EntityType.Customer:
                    MerchelloContext.Services.CustomerService.RemoveFromCollection(model.EntityKey, model.CollectionKey);
                    break;
                case EntityType.Invoice:
                    MerchelloContext.Services.InvoiceService.RemoveFromCollection(model.EntityKey, model.CollectionKey);
                    break;
                case EntityType.Product:
                    MerchelloContext.Services.ProductService.RemoveFromCollection(model.EntityKey, model.CollectionKey);
                    break;
                default:
                    var invalid =
                        new InvalidOperationException("Merchello service could not be found for the entity type");
                    MultiLogHelper.Error<EntityCollectionApiController>("An attempt was made to remove an entity to a collection", invalid);
                    return Request.CreateResponse(HttpStatusCode.NotImplemented);
            }

            return Request.CreateResponse(HttpStatusCode.OK);        
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
        /// Saves the entity collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionDisplay"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the collection is not found
        /// </exception>
        [HttpPut, HttpPost]
        public EntityCollectionDisplay PutEntityCollection(EntityCollectionDisplay collection)
        {
            var destination = _entityCollectionService.GetByKey(collection.Key);
            if (destination == null) throw new NullReferenceException("Collection was not found");

            destination = collection.ToEntityCollection(destination);
            _entityCollectionService.Save(destination);

            return destination.ToEntityCollectionDisplay();
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
        /// The get cached query by entity type.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="ICachedCollectionQuery"/>.
        /// </returns>
        private ICachedCollectionQuery GetCachedQueryByEntityType(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Customer:
                    return _merchello.Query.Customer;
                case EntityType.Invoice:
                    return _merchello.Query.Invoice;
                case EntityType.Product:
                    return _merchello.Query.Product;
                default:
                    throw new NotSupportedException("Customer, Invoice and Product queries are only supported.");
            }
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