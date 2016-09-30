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
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
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

            _merchello = new MerchelloHelper(merchelloContext, false);

            this.Initialize();
        }

        /// <summary>
        /// The get sortable provider keys.
        /// </summary>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        /// TODO update this to resolve from a common Marker inteface
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
        /// Gets the entity filter group providers for a given type of entity.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityCollectionProviderDisplay> GetEntityFilterGroupProviders(EntityType entityType)
        {
            if (entityType != EntityType.Product) throw new NotImplementedException("Only Product types have been implemented");

            return
                _resolver.GetProviderAttributes<IProductEntityFilterGroupProvider>()
                    .Select(x => x.ToEntityCollectionProviderDisplay());
        }

        /// <summary>
        /// The get entity specified filter collection attribute provider.
        /// </summary>
        /// <param name="key">
        /// The entity collection key.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderDisplay"/>.
        /// </returns>
        [HttpGet]
        public EntityCollectionProviderDisplay GetEntityFilterGroupFilterProvider(Guid key)
        {
            var att = 
                _resolver.GetProviderAttributeForFilter(key);

            if (att == null) throw new NullReferenceException("Could not find Attribute Provider");

            return att.ToEntityCollectionProviderDisplay();
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
        /// Gets a list of entity specification collections by entity type.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityFilterGroupDisplay> GetEntityFilterGroups(EntityType entityType)
        {
            if (entityType != EntityType.Product) throw new NotImplementedException();

            var keys = EntityCollectionProviderResolver.Current.GetProviderKeys<IEntityFilterGroupProvider>();

            // TODO service call will need to be updated to respect entity type if ever opened up to other entity types
            var collections = ((EntityCollectionService)_entityCollectionService).GetEntityFilterGroupsByProviderKeys(keys);
            
            return collections.Select(c => c.ToEntitySpecificationCollectionDisplay()).OrderBy(x => x.SortOrder);
        }

        /// <summary>
        /// Gets a collection of <see cref="IEntityFilterGroup"/> by a collection of keys that are not associated
        /// with a product
        /// </summary>
        /// <param name="entityType">
        /// The entity Type.
        /// </param>
        /// <param name="entityKey">
        /// The entity Key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntitySpecifiedFilterCollection}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityFilterGroupDisplay> GetEntityFilterGroupsContaining(EntityType entityType, Guid entityKey)
        {
            if (entityType != EntityType.Product) throw new NotImplementedException();

            var key = EntityCollectionProviderResolver.Current.GetProviderKey<ProductFilterGroupProvider>();

            // TODO service call will need to be updated to respect entity type if ever opened up to other entity types
            var collections = ((EntityCollectionService)_entityCollectionService).GetEntityFilterGroupsContainingProduct(new[] { key }, entityKey);

            return collections.Select(c => c.ToEntitySpecificationCollectionDisplay()).OrderBy(x => x.SortOrder);
        }

        /// <summary>
        /// Gets a collection of <see cref="IEntityFilterGroup"/> by a collection of keys that are not associated
        /// with a product
        /// </summary>
        /// <param name="entityType">
        /// The entity Type.
        /// </param>
        /// <param name="entityKey">
        /// The entity Key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntitySpecifiedFilterCollection}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<EntityFilterGroupDisplay> GetEntityFilterGroupsNotContaining(EntityType entityType, Guid entityKey)
        {
            if (entityType != EntityType.Product) throw new NotImplementedException();

            var key = EntityCollectionProviderResolver.Current.GetProviderKey<ProductFilterGroupProvider>();

            // TODO service call will need to be updated to respect entity type if ever opened up to other entity types
            var collections = ((EntityCollectionService)_entityCollectionService).GetEntityFilterGroupsNotContainingProduct(new[] { key }, entityKey);

            return collections.Select(c => c.ToEntitySpecificationCollectionDisplay()).OrderBy(x => x.SortOrder);
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
                        product.GetCollectionsContaining(model.IsFilter).Select(x => x.ToEntityCollectionDisplay()).OrderBy(x => x.Name);

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
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, invalid);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Adds entity associations with specified filters.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage PostAssociateEntityWithFilters(Entity2FilterCollectionsModel model)
        {
            var collections = _entityCollectionService.GetAll(model.CollectionKeys).ToArray();

            foreach (var collection in collections)
            {
                var provider = collection.ResolveProvider();
                var entityType = provider.EntityCollection.EntityType;
                switch (entityType)
                {
                    case EntityType.Customer:
                        var noCustomer = new NotImplementedException("Customer filters not implemented");
                        return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, noCustomer);
                    case EntityType.Invoice:
                        var noInvoice = new NotImplementedException("Invoice filters not implemented");
                        return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, noInvoice);
                    case EntityType.Product:
                        MerchelloContext.Services.ProductService.AddToCollection(model.EntityKey, collection.Key);
                        break;
                    default:
                        var invalid =
                            new InvalidOperationException("Merchello service could not be found for the entity type");
                        MultiLogHelper.Error<EntityCollectionApiController>("An attempt was made to add an entity to a collection", invalid);
                        return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, invalid);
                }
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

            ec.IsFilter = collection.IsFilter;
            ((EntityCollection)ec).ExtendedData = collection.ExtendedData.AsExtendedDataCollection();

            _entityCollectionService.Save(ec);

            return ec.ToEntityCollectionDisplay();
        }

        /// <summary>
        /// Saves an entity filter group.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntityFilterGroupDisplay"/>.
        /// </returns>
        [HttpPut, HttpPost]
        public EntityFilterGroupDisplay PutEntityFilterGroup(EntityFilterGroupDisplay collection)
        {
            var currentVersion = ((EntityCollectionService)_entityCollectionService).GetEntityFilterGroupByKey(collection.Key);
            if (currentVersion == null) throw new NullReferenceException("Collection was not found");

            // update the root (filter) collection
            var filter = (IEntityCollection)currentVersion;
            filter = collection.ToEntityCollection(filter);

            _entityCollectionService.Save(filter);

            var removers =
                currentVersion.Filters.Where(
                    x =>
                    collection.Filters.Where(adds => !adds.Key.Equals(Guid.Empty)).All(y => y.Key != x.Key));

            // remove the removers
            foreach (var rm in removers)
            {
                _entityCollectionService.Delete(rm);
            }

            var operations = new List<IEntityCollection>();

            // new attribute collections
            foreach (var op in collection.Filters)
            {
                if (op.Key.Equals(Guid.Empty))
                {
                    var ec = _entityCollectionService.CreateEntityCollection(op.EntityType, op.ProviderKey, op.Name);

                    ec.ParentKey = op.ParentKey ?? collection.Key;
                    ec.IsFilter = collection.IsFilter;

                    operations.Add(ec);
                }
                else
                {
                    var exist = _entityCollectionService.GetByKey(op.Key);
                    exist = op.ToEntityCollection(exist);
                    operations.Add(exist);
                }

            }

            _entityCollectionService.Save(operations);


            return
                ((EntityCollectionService)_entityCollectionService).GetEntityFilterGroupByKey(collection.Key)
                    .ToEntitySpecificationCollectionDisplay();
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

            var existing = _entityCollectionService.GetAll(collectionsArray.Select(x => x.Key).ToArray()).ToArray();

            var updates = new List<IEntityCollection>();

            foreach (var update in collectionsArray)
            {
                var match = existing.FirstOrDefault(x => x.Key == update.Key);
                if (match == null) continue;
                updates.Add(update.ToEntityCollection(match));
            }

            _entityCollectionService.Save(updates);


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