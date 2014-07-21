﻿namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web.UI.WebControls;

    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// Represents a warehouse catalog service.
    /// </summary>
    internal class WarehouseCatalogService : IWarehouseCatalogService
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
        /// Initializes a new instance of the <see cref="WarehouseCatalogService"/> class.
        /// </summary>
        internal WarehouseCatalogService()
            : this(new RepositoryFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseCatalogService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        internal WarehouseCatalogService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseCatalogService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        internal WarehouseCatalogService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
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
        public static event TypedEventHandler<IWarehouseCatalogService, Events.NewEventArgs<IWarehouseCatalog>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IWarehouseCatalogService, Events.NewEventArgs<IWarehouseCatalog>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IWarehouseCatalogService, SaveEventArgs<IWarehouseCatalog>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IWarehouseCatalogService, SaveEventArgs<IWarehouseCatalog>> Saved;

        /// <summary>
        /// Occurs before an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IWarehouseCatalogService, StatusChangeEventArgs<IWarehouseCatalog>> StatusChanging;

        /// <summary>
        /// Occurs after an invoice status has changed
        /// </summary>
        public static event TypedEventHandler<IWarehouseCatalogService, StatusChangeEventArgs<IWarehouseCatalog>> StatusChanged;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IWarehouseCatalogService, DeleteEventArgs<IWarehouseCatalog>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IWarehouseCatalogService, DeleteEventArgs<IWarehouseCatalog>> Deleted;

        #endregion

        /// <summary>
        /// Creates warehouse catalog and persists it to the database.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>
        /// The <see cref="IWarehouseCatalog"/>.
        /// </returns>
        public IWarehouseCatalog CreateWarehouseCatalogWithKey(
            Guid warehouseKey,
            string name,
            string description = "",
            bool raiseEvents = true)
        {
            Mandate.ParameterCondition(!warehouseKey.Equals(Guid.Empty), "warehouseKey");

            var catalog = new WarehouseCatalog(warehouseKey) { Name = name, Description = description };

            if (raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IWarehouseCatalog>(catalog), this))
            {
                catalog.WasCancelled = true;
                return catalog;
            }

            using (new WriteLock(Locker))
            { 
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseCatalogRepository(uow))
                {
                    repository.AddOrUpdate(catalog);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<IWarehouseCatalog>(catalog), this);
            
            return catalog;
        }

        /// <summary>
        /// Saves a single <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalog">
        /// The warehouse catalog.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IWarehouseCatalog warehouseCatalog, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IWarehouseCatalog>(warehouseCatalog), this))
            {
                ((WarehouseCatalog)warehouseCatalog).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseCatalogRepository(uow))
                {
                    repository.AddOrUpdate(warehouseCatalog);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IWarehouseCatalog>(warehouseCatalog), this);
        }

        /// <summary>
        /// Saves a collection of <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalogs">
        /// The warehouse catalogs.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IWarehouseCatalog> warehouseCatalogs, bool raiseEvents = true)
        {
            var catalogsArray = warehouseCatalogs as IWarehouseCatalog[] ?? warehouseCatalogs.ToArray();
            if (raiseEvents)
            Saving.RaiseEvent(new SaveEventArgs<IWarehouseCatalog>(catalogsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseCatalogRepository(uow))
                {
                    foreach (var catalog in catalogsArray)
                    {
                        repository.AddOrUpdate(catalog);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IWarehouseCatalog>(catalogsArray), this);
        }

        public void Delete(IWarehouseCatalog warehouseCatalog, bool raiseEvents = true)
        {
            if (warehouseCatalog.Key == Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey) return;

            if (raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IWarehouseCatalog>(warehouseCatalog), this))
            {
                ((WarehouseCatalog)warehouseCatalog).WasCancelled = true;
                return;
            }


        }

        public void Delete(IEnumerable<IWarehouseCatalog> warehouseCatalogs, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public IWarehouseCatalog GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IWarehouseCatalog> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IWarehouseCatalog> GetByWarehouseKey(Guid warehouseKey)
        {
            throw new NotImplementedException();
        }
    }
}