namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a <see cref="IDigitalMediaService"/>.
    /// </summary>
    public class DigitalMediaService : MerchelloRepositoryService, IDigitalMediaService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaService"/> class.
        /// </summary>
        public DigitalMediaService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DigitalMediaService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DigitalMediaService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaService"/> class.
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
        public DigitalMediaService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalMediaService"/> class.
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
        public DigitalMediaService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, Events.NewEventArgs<IDigitalMedia>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, Events.NewEventArgs<IDigitalMedia>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, SaveEventArgs<IDigitalMedia>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, SaveEventArgs<IDigitalMedia>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IDigitalMediaService, DeleteEventArgs<IDigitalMedia>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IDigitalMediaService, DeleteEventArgs<IDigitalMedia>> Deleted;

        #endregion

        /// <summary>
        /// Creates a <see cref="IDigitalMedia"/> and saves it to the database.
        /// </summary>
        /// <param name="productVariantKey">
        /// Tkey for the item to work
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        public IDigitalMedia CreateDigitalMediaForProductVariant(Guid productVariantKey, bool raiseEvents = true)
        {
            var digitalMedia = new DigitalMedia() { 
                FirstAccessed = null,
                ProductVariantKey = productVariantKey,
                ExtendedData = new ExtendedDataCollection()
            };

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IDigitalMedia>(digitalMedia), this))
            {
                digitalMedia.WasCancelled = true;
                return digitalMedia;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    repository.AddOrUpdate(digitalMedia);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IDigitalMedia>(digitalMedia), this);

            return digitalMedia;
        }

        /// <summary>
        /// Saves a single <see cref="IDigitalMedia"/>
        /// </summary>
        /// <param name="digitalMedia">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IDigitalMedia digitalMedia, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IDigitalMedia>(digitalMedia), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    repository.AddOrUpdate(digitalMedia);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IDigitalMedia>(digitalMedia), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IDigitalMedia"/>.
        /// </summary>
        /// <param name="digitalMedias">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Save(IEnumerable<IDigitalMedia> digitalMedias, bool raiseEvents = true)
        {
            var digitalMediaArray = digitalMedias as IDigitalMedia[] ?? digitalMedias.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IDigitalMedia>(digitalMediaArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();

                using (var repository = RepositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    foreach (var digitalMedia in digitalMediaArray)
                    {
                        repository.AddOrUpdate(digitalMedia);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IDigitalMedia>(digitalMediaArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IDigitalMedia"/> from the database.
        /// </summary>
        /// <param name="digitalMedia">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Delete(IDigitalMedia digitalMedia, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IDigitalMedia>(digitalMedia), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    repository.Delete(digitalMedia);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IDigitalMedia>(digitalMedia), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IDigitalMedia"/> from the database.
        /// </summary>
        /// <param name="digitalMedias">
        /// The digital medias.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(IEnumerable<IDigitalMedia> digitalMedias, bool raiseEvents = true)
        {
            var digitalMediaArray = digitalMedias as IDigitalMedia[] ?? digitalMedias.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IDigitalMedia>(digitalMediaArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateDigitalMediaRepository(uow))
                {
                    foreach (var digitalMedia in digitalMediaArray)
                    {
                        repository.Delete(digitalMedia);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IDigitalMedia>(digitalMediaArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IDigitalMedia"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        public IDigitalMedia GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateDigitalMediaRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IDigitalMedia"/> given a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IDigitalMedia}"/>.
        /// </returns>
        public IEnumerable<IDigitalMedia> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateDigitalMediaRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Returns a collection of all <see cref="IDigitalMedia"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IDigitalMedia}"/>.
        /// </returns>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal IEnumerable<IDigitalMedia> GetAll()
        {
            using (var repository = RepositoryFactory.CreateDigitalMediaRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        } 
    }
}
