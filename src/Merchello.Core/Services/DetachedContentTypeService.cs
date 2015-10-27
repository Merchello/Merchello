namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a detached content type service.
    /// </summary>
    internal class DetachedContentTypeService : MerchelloRepositoryService, IDetachedContentTypeService
    {        
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentTypeService"/> class.
        /// </summary>
        public DetachedContentTypeService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentTypeService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DetachedContentTypeService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentTypeService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DetachedContentTypeService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentTypeService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DetachedContentTypeService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetachedContentTypeService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
        /// </param>
        public DetachedContentTypeService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IDetachedContentTypeService, Events.NewEventArgs<IDetachedContentType>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IDetachedContentTypeService, Events.NewEventArgs<IDetachedContentType>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IDetachedContentTypeService, SaveEventArgs<IDetachedContentType>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IDetachedContentTypeService, SaveEventArgs<IDetachedContentType>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IDetachedContentTypeService, DeleteEventArgs<IDetachedContentType>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IDetachedContentTypeService, DeleteEventArgs<IDetachedContentType>> Deleted;

        #endregion

        /// <summary>
        /// Creates a <see cref="IDetachedContentType"/> without saving it to the database.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        public IDetachedContentType CreateDetachedContentType(EntityType entityType, Guid contentTypeKey, string name, bool raiseEvents = true)
        {
            return CreateDetachedContentType(
                EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey,
                contentTypeKey,
                name,
                raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="IDetachedContentType"/> without saving it to the database.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        public IDetachedContentType CreateDetachedContentType(
            Guid entityTfKey,
            Guid contentTypeKey,
            string name,
            bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(entityTfKey), "entityTfKey");           

            var dt = new DetachedContentType(entityTfKey, contentTypeKey.Equals(Guid.Empty) ? (Guid?)null : contentTypeKey) { Name = name };

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IDetachedContentType>(dt), this))
            {
                dt.WasCancelled = true;
                return dt;
            }

            return dt;
        }

        /// <summary>
        /// Creates a <see cref="IDetachedContentType"/> and saves to the database.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        public IDetachedContentType CreateDetachedContentTypeWithKey(
            EntityType entityType,
            Guid contentTypeKey,
            string name,
            bool raiseEvents = true)
        {
            return CreateDetachedContentTypeWithKey(
                EnumTypeFieldConverter.EntityType.GetTypeField(entityType).TypeKey,
                contentTypeKey,
                name,
                raiseEvents);
        }

        /// <summary>
        /// Creates a <see cref="IDetachedContentType"/> and saves to the database.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <param name="contentTypeKey">
        /// The content type key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        public IDetachedContentType CreateDetachedContentTypeWithKey(
            Guid entityTfKey,
            Guid contentTypeKey,
            string name,
            bool raiseEvents = true)
        {
            var detachedContent = this.CreateDetachedContentType(entityTfKey, contentTypeKey, name, raiseEvents);
            
            if (((DetachedContentType)detachedContent).WasCancelled) return detachedContent;

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(uow))
                {
                    repository.AddOrUpdate(detachedContent);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Created.RaiseEvent(new Events.NewEventArgs<IDetachedContentType>(detachedContent), this);

            return detachedContent;
        }

        /// <summary>
        /// Saves a single instance of <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="detachedContentType">
        /// The detached content type.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Save(IDetachedContentType detachedContentType, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IDetachedContentType>(detachedContentType), this))
                {
                    ((DetachedContentType)detachedContentType).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(uow))
                {
                    repository.AddOrUpdate(detachedContentType);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Saved.RaiseEvent(new SaveEventArgs<IDetachedContentType>(detachedContentType), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="detachedContentTypes">
        /// The collection to be saved.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Save(IEnumerable<IDetachedContentType> detachedContentTypes, bool raiseEvents = true)
        {
            var detachedContentArray = detachedContentTypes as IDetachedContentType[] ?? detachedContentTypes.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IDetachedContentType>(detachedContentArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(uow))
                {
                    foreach (var detachedContent in detachedContentArray)
                    {
                        repository.AddOrUpdate(detachedContent);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IDetachedContentType>(detachedContentArray), this);
        }

        /// <summary>
        /// Deletes a single instance of <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="detachedContentType">
        /// The detached content type.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Delete(IDetachedContentType detachedContentType, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IDetachedContentType>(detachedContentType), this))
            {
                ((DetachedContentType)detachedContentType).WasCancelled = true;
                return;
            }


            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(uow))
                {
                    repository.Delete(detachedContentType);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IDetachedContentType>(detachedContentType), this);
        }        

        /// <summary>
        /// Gets a <see cref="IDetachedContentType"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IDetachedContentType"/>.
        /// </returns>
        public IDetachedContentType GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IDetachedContentType"/> by the entity type key.
        /// </summary>
        /// <param name="entityTfKey">
        /// The entity type field key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDetachedContentType}"/>.
        /// </returns>
        public IEnumerable<IDetachedContentType> GetDetachedContentTypesByEntityTfKey(Guid entityTfKey)
        {
            using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IDetachedContentType>.Builder.Where(x => x.EntityTfKey == entityTfKey);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IDetachedContentType"/> by the Umbraco content type key.
        /// </summary>
        /// <param name="contentTypeKey">
        /// The Umbraco content type key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDetachedContentType}"/>.
        /// </returns>
        public IEnumerable<IDetachedContentType> GetDetachedContentTypesByContentTypeKey(Guid contentTypeKey)
        {
            using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IDetachedContentType>.Builder.Where(x => x.ContentTypeKey == contentTypeKey);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets all <see cref="IDetachedContentType"/>s.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IDetachedContentType}"/>.
        /// </returns>
        public IEnumerable<IDetachedContentType> GetAll()
        {
            using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Deletes a collection of <see cref="IDetachedContentType"/>.
        /// </summary>
        /// <param name="detachedContentTypes">
        /// The collection to be deleted.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        internal void Delete(IEnumerable<IDetachedContentType> detachedContentTypes, bool raiseEvents = true)
        {
            var detachedContentArray = detachedContentTypes as IDetachedContentType[] ?? detachedContentTypes.ToArray();
            if (!detachedContentArray.Any()) return;
            if (raiseEvents)
                Deleting.RaiseEvent(new DeleteEventArgs<IDetachedContentType>(detachedContentArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDetachedContentTypeRepository(uow))
                {
                    foreach (var detachedContent in detachedContentArray)
                    {
                        repository.Delete(detachedContent);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents)
                Deleted.RaiseEvent(new DeleteEventArgs<IDetachedContentType>(detachedContentArray), this);
        }
    }
}