namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents the ShipRateTierService
    /// </summary>
    internal class ShipRateTierService : MerchelloRepositoryService, IShipRateTierService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipRateTierService"/> class.
        /// </summary>
        public ShipRateTierService()
            : this(LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipRateTierService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        internal ShipRateTierService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipRateTierService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        internal ShipRateTierService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipRateTierService"/> class.
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
        internal ShipRateTierService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipRateTierService"/> class.
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
        internal ShipRateTierService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
        }

        #endregion

        #region Event Handlers

        ///// <summary>
        ///// Occurs after Create
        ///// </summary>
        //public static event TypedEventHandler<IShipRateTierService, Events.NewEventArgs<IShipRateTier>> Creating;


        ///// <summary>
        ///// Occurs after Create
        ///// </summary>
        //public static event TypedEventHandler<IShipRateTierService, Events.NewEventArgs<IShipRateTier>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShipRateTierService, SaveEventArgs<IShipRateTier>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShipRateTierService, SaveEventArgs<IShipRateTier>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShipRateTierService, DeleteEventArgs<IShipRateTier>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShipRateTierService, DeleteEventArgs<IShipRateTier>> Deleted;

        #endregion


        /// <summary>
        /// Saves a single <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier">The <see cref="IShipRateTier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IShipRateTier shipRateTier, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IShipRateTier>(shipRateTier), this))
            {
                ((ShipRateTier) shipRateTier).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateShipRateTierRepository(uow))
                {
                    repository.AddOrUpdate(shipRateTier);
                    uow.Commit();
                }
            }

            if(raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipRateTier>(shipRateTier), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTierList">The collection of <see cref="IShipRateTier"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IEnumerable<IShipRateTier> shipRateTierList, bool raiseEvents = true)
        {
            var shipRateTiersArray = shipRateTierList as IShipRateTier[] ?? shipRateTierList.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IShipRateTier>(shipRateTiersArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateShipRateTierRepository(uow))
                {
                    foreach (var shipRateTier in shipRateTiersArray)
                    {
                        repository.AddOrUpdate(shipRateTier);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipRateTier>(shipRateTiersArray), this);
        }

        /// <summary>
        /// Deletes a <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier">The <see cref="IShipRateTier"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Delete(IShipRateTier shipRateTier, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IShipRateTier>(shipRateTier), this))
            {
                ((ShipRateTier)shipRateTier).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateShipRateTierRepository(uow))
                {
                    repository.Delete(shipRateTier);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipRateTier>(shipRateTier), this);
        }

        /// <summary>
        /// Gets a list of <see cref="IShipRateTier"/> objects given a <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">The ship method key</param>
        /// <returns>A collection of <see cref="IShipRateTier"/></returns>
        public IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey)
        {
            using (var repository = RepositoryFactory.CreateShipRateTierRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<IShipRateTier>.Builder.Where(x => x.ShipMethodKey == shipMethodKey);
                return repository.GetByQuery(query);
            }
        }
    }
}