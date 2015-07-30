namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents an entity collection service.
    /// </summary>
    internal class EntityCollectionService : IEntityCollectionService
    {
        #region Fields

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The unit of work provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionService"/> class.
        /// </summary>
        internal EntityCollectionService()
            : this(new RepositoryFactory())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        internal EntityCollectionService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollectionService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        internal EntityCollectionService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
        }

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IEntityCollectionService, Events.NewEventArgs<IEntityCollection>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IEntityCollectionService, Events.NewEventArgs<IEntityCollection>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IEntityCollectionService, SaveEventArgs<IEntityCollection>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IEntityCollectionService, SaveEventArgs<IEntityCollection>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IEntityCollectionService, DeleteEventArgs<IEntityCollection>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IEntityCollectionService, DeleteEventArgs<IEntityCollection>> Deleted;

        #endregion

        /// <summary>
        /// The create entity collection.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        public IEntityCollection CreateEntityCollection(EntityType entityType, Guid providerKey, string name, bool raiseEvents = true)
        {
            var entityTfKey = EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey;

            return CreateEntityCollection(entityTfKey, providerKey, name, raiseEvents);
        }

        /// <summary>
        /// The create entity collection with key.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        public IEntityCollection CreateEntityCollectionWithKey(
            EntityType entityType,
            Guid providerKey,
            string name,
            bool raiseEvents = true)
        {
            var entityTfKey = EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey;

            return CreateEntityCollectionWithKey(entityTfKey, providerKey, name, raiseEvents);
        }

        /// <summary>
        /// Saves a single entity collection.
        /// </summary>
        /// <param name="entityCollection">
        /// The entity collection.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IEntityCollection entityCollection, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IEntityCollection>(entityCollection), this))
            {
                ((EntityCollection)entityCollection).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateEntityCollectionRepository(uow))
                {
                    repository.AddOrUpdate(entityCollection);
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Saved.RaiseEvent(new SaveEventArgs<IEntityCollection>(entityCollection), this);
        }

        /// <summary>
        /// Saves a collection of entity collections.
        /// </summary>
        /// <param name="entityCollections">
        /// The entity collections.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IEnumerable<IEntityCollection> entityCollections, bool raiseEvents = true)
        {
            var collectionsArray = entityCollections as IEntityCollection[] ?? entityCollections.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IEntityCollection>(collectionsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateEntityCollectionRepository(uow))
                {
                    foreach (var collection in collectionsArray)
                    {
                        repository.AddOrUpdate(collection);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IEntityCollection>(collectionsArray), this);
        }

        /// <summary>
        /// Deletes a single entity collection.
        /// </summary>
        /// <param name="entityCollection">
        /// The entity collection.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Delete(IEntityCollection entityCollection, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IEntityCollection>(entityCollection), this))
            {
                ((EntityCollection)entityCollection).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateEntityCollectionRepository(uow))
                {
                    repository.Delete(entityCollection);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IEntityCollection>(entityCollection), this);
        }

        /// <summary>
        /// Deletes a collection of entity collections.
        /// </summary>
        /// <param name="entityCollections">
        /// The entity collections.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Delete(IEnumerable<IEntityCollection> entityCollections, bool raiseEvents = true)
        {
            var collectionsArray = entityCollections as IEntityCollection[] ?? entityCollections.ToArray();
            if (raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<IEntityCollection>(collectionsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateEntityCollectionRepository(uow))
                {
                    foreach (var collection in collectionsArray)
                    {
                        repository.Delete(collection);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<IEntityCollection>(collectionsArray), this);
        }

        /// <summary>
        /// The get by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        public IEntityCollection GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateEntityCollectionRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// The get by entity type field key.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        public IEnumerable<IEntityCollection> GetByEntityTfKey(Guid entityTfKey)
        {
            using (var repository = _repositoryFactory.CreateEntityCollectionRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IEntityCollection>.Builder.Where(x => x.EntityTfKey == entityTfKey);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// The get by provider key.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        public IEnumerable<IEntityCollection> GetByProviderKey(Guid providerKey)
        {
            using (var repository = _repositoryFactory.CreateEntityCollectionRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IEntityCollection>.Builder.Where(x => x.ProviderKey == providerKey);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        public IEnumerable<IEntityCollection> GetAll()
        {
            using (var repository = _repositoryFactory.CreateEntityCollectionRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }


        /// <summary>
        /// The create entity collection.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        internal IEntityCollection CreateEntityCollection(Guid entityTfKey, Guid providerKey, string name, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(entityTfKey), "entityTfKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            var collection = new EntityCollection(entityTfKey, providerKey) { Name = name };

            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IEntityCollection>(collection), this))
            {
                collection.WasCancelled = true;
                return collection;
            }

            return collection;
        }

        /// <summary>
        /// The create entity collection with key.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        internal IEntityCollection CreateEntityCollectionWithKey(Guid entityTfKey, Guid providerKey, string name, bool raiseEvents = true)
        {
            var collection = this.CreateEntityCollection(entityTfKey, providerKey, name, raiseEvents);

            if (((EntityCollection)collection).WasCancelled) return collection;

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateEntityCollectionRepository(uow))
                {
                    repository.AddOrUpdate(collection);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Created.RaiseEvent(new Events.NewEventArgs<IEntityCollection>(collection), this);

            return collection;
        }

        /// <summary>
        /// The get entity collections by product key.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IEntityCollection}"/>.
        /// </returns>
        internal IEnumerable<IEntityCollection> GetEntityCollectionsByProductKey(Guid productKey)
        {
            using (var repository = _repositoryFactory.CreateEntityCollectionRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetEntityCollectionsByProductKey(productKey);                
            }
        } 
    }
}