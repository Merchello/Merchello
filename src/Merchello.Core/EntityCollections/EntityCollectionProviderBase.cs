namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// The entity collection provider base.
    /// </summary>
    public abstract class EntityCollectionProviderBase : IEntityCollectionProvider
    {
        /// <summary>
        /// The entity collection.
        /// </summary>
        private Lazy<IEntityCollection> _entityCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionProviderBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection Key.
        /// </param>
        protected EntityCollectionProviderBase(IMerchelloContext merchelloContext, Guid collectionKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(collectionKey), "collectionKey");
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            MerchelloContext = merchelloContext;
            this.CollectionKey = collectionKey;
            this.Initialize();
        }

        /// <summary>
        /// Gets the entity collection.
        /// </summary>
        public IEntityCollection EntityCollection
        {
            get
            {
                return _entityCollection.Value;
            }
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        protected ICacheProvider Cache
        {
            get
            {
                return MerchelloContext.Cache.RequestCache;
            }
        }

        /// <summary>
        /// Gets the <see cref="IMerchelloContext"/>.
        /// </summary>
        protected IMerchelloContext MerchelloContext { get; private set; }


        /// <summary>
        /// Gets the collection key.
        /// </summary>
        protected Guid CollectionKey { get; private set; }


        /// <summary>
        /// The get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Object}"/>.
        /// </returns>
        public abstract IEnumerable<object> GetEntities();
        

        /// <summary>
        /// The get entities.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="IEntity"/>
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<T> GetEntities<T>() where T : class, IEntity
        {
            this.ValidateType(typeof(T)); 
            
            return this.GetEntities().Select(x => x as T);
        }

        /// <summary>
        /// Gets a generic page of entities.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Object"/>.
        /// </returns>
        public abstract Page<object> GetPagedEntities(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending);

        /// <summary>
        /// Gets a page of typed entities
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <typeparam name="T">
        /// The type of <see cref="IEntity"/>
        /// </typeparam>
        /// <returns>
        /// The <see cref="Page{T}"/>.
        /// </returns>
        public Page<T> GetPagedEntities<T>(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
            where T : class, IEntity
        {
            this.ValidateType(typeof(T));

            var p = GetPagedEntities(page, itemsPerPage, sortBy, sortDirection);

            return new Page<T>()
                {
                    Context = p.Context,
                    CurrentPage = p.CurrentPage,
                    ItemsPerPage = p.ItemsPerPage,
                    TotalItems = p.TotalItems,
                    TotalPages = p.TotalPages,
                    Items = p.Items.Select(x => x as T).ToList()
                };
        }

        /// <summary>
        /// Ensures this is the provider by <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool EnsureType(Type type)
        {
            return EntityCollection.TypeOfEntities() == type;
        }

        /// <summary>
        /// Ensures this is the provider for the <see cref="EntityType"/>.
        /// </summary>
        /// <param name="entityType">
        /// The entity Type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws an exception if the EntityCollectionProviderAttribute was not found
        /// </exception>
        /// <remarks>
        /// Used in classes such as the MerchelloHelper
        /// </remarks>
        internal bool EnsureEntityType(EntityType entityType)
        {
            var att = this.ProviderAttribute();

            if (att == null)
            {
                var nullReference =
                    new NullReferenceException(
                        "EntityCollectionProvider was not decorated with an EntityCollectionProviderAttribute");
                MultiLogHelper.Error<EntityCollectionProviderBase>("Provider must be decorated with an attribute", nullReference);
                throw nullReference;
            }

            return att.EntityTfKey.Equals(EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey);
        }

        /// <summary>
        /// Validates the type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <exception cref="InvalidCastException">
        /// Throws an exception if the type does not match the collection
        /// </exception>
        protected void ValidateType(Type type)
        {
            if (this.EnsureType(type)) return;

            var invalidType = new InvalidCastException("Cannot cast " + type.FullName + " to " + EntityCollection.TypeOfEntities().FullName);
            MultiLogHelper.Error<EntityCollectionProviderBase>("Invalid type", invalidType);
            throw invalidType;
        }

        /// <summary>
        /// Gets an instance of the <see cref="IEntityCollection"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        private IEntityCollection GetInstance()
        {
            var cacheKey = string.Format("merch.entitycollection.{0}", CollectionKey);
            var provider = Cache.GetCacheItem(cacheKey);
            if (provider != null) return (IEntityCollection)provider;

            return
                (IEntityCollection)
                Cache.GetCacheItem(
                    cacheKey,
                    () => MerchelloContext.Services.EntityCollectionService.GetByKey(CollectionKey));
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        private void Initialize()
        {
            _entityCollection = new Lazy<IEntityCollection>(this.GetInstance);
        }
    }
}