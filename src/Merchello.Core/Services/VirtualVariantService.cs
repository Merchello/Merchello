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
    /// Represents a <see cref="IVirtualVariantService"/>.
    /// </summary>
    public class VirtualVariantService : MerchelloRepositoryService, IVirtualVariantService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualVariantService"/> class.
        /// </summary>
        public VirtualVariantService()
            : this(LoggerResolver.Current.Logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualVariantService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public VirtualVariantService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualVariantService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public VirtualVariantService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualVariantService"/> class.
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
        public VirtualVariantService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualVariantService"/> class.
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
        public VirtualVariantService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IVirtualVariantService, Events.NewEventArgs<IVirtualVariant>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IVirtualVariantService, Events.NewEventArgs<IVirtualVariant>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IVirtualVariantService, SaveEventArgs<IVirtualVariant>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IVirtualVariantService, SaveEventArgs<IVirtualVariant>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IVirtualVariantService, DeleteEventArgs<IVirtualVariant>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IVirtualVariantService, DeleteEventArgs<IVirtualVariant>> Deleted;

        #endregion

        /// <summary>
        /// Creates a <see cref="IVirtualVariant"/> and saves it to the database.
        /// </summary>
        /// <param name="name">
        /// Tkey for the item to work
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IDigitalMedia"/>.
        /// </returns>
        public IVirtualVariant CreateVirtualVariant(string name, bool raiseEvents = true)
        {
            var virtualVariant = new VirtualVariant() {
                Sku = "",
                ProductKey = Guid.NewGuid(),
                Choices = new Dictionary<string, string>()
            };

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IVirtualVariant>(virtualVariant), this))
            {
                virtualVariant.WasCancelled = true;
                return virtualVariant;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateVirtualVariantRepository(uow))
                {
                    repository.AddOrUpdate(virtualVariant);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IVirtualVariant>(virtualVariant), this);

            return virtualVariant;
        }

        /// <summary>
        /// Saves a single <see cref="IVirtualVariant"/>
        /// </summary>
        /// <param name="virtualVariant">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        ///  Optional boolean indicating whether or not to raise events
        /// </param>
        public void Save(IVirtualVariant virtualVariant, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IVirtualVariant>(virtualVariant), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateVirtualVariantRepository(uow))
                {
                    repository.AddOrUpdate(virtualVariant);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IVirtualVariant>(virtualVariant), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IVirtualVariant"/> from the database.
        /// </summary>
        /// <param name="virtualVariant">
        /// The digital media.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events.
        /// </param>
        public void Delete(IVirtualVariant virtualVariant, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IVirtualVariant>(virtualVariant), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateVirtualVariantRepository(uow))
                {
                    repository.Delete(virtualVariant);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IVirtualVariant>(virtualVariant), this);
        }

        /// <summary>
        /// Gets a <see cref="IVirtualVariant"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IVirtualVariant"/>.
        /// </returns>
        public IVirtualVariant GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateVirtualVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IVirtualVariant"/> given a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IVirtualVariant}"/>.
        /// </returns>
        public IEnumerable<IVirtualVariant> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = RepositoryFactory.CreateVirtualVariantRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }
    }
}
