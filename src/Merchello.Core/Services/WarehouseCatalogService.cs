namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Events;
    using Models;
    using Persistence;
    using Persistence.Querying;
    using Persistence.UnitOfWork;
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

        /// <summary>
        /// The product variant service.
        /// </summary>
        private readonly IProductVariantService _productVariantService;

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseCatalogService"/> class.
        /// </summary>
        internal WarehouseCatalogService()
            : this(new RepositoryFactory(), new ProductVariantService())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseCatalogService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="productVariantService">
        /// The product Variant Service.
        /// </param>
        internal WarehouseCatalogService(RepositoryFactory repositoryFactory, IProductVariantService productVariantService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, productVariantService)
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
        /// <param name="productVariantService">
        /// The product Variant Service.
        /// </param>
        internal WarehouseCatalogService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IProductVariantService productVariantService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(productVariantService, "productVariantService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _productVariantService = productVariantService;
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

        /// <summary>
        /// Deletes a single instance of a <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalog">
        /// The warehouse catalog.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(IWarehouseCatalog warehouseCatalog, bool raiseEvents = true)
        {
            if (warehouseCatalog.Key == Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey) return;

            if (raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IWarehouseCatalog>(warehouseCatalog), this))
            {
                ((WarehouseCatalog)warehouseCatalog).WasCancelled = true;
                return;
            }

            RemoveVariantsFromCatalogInventoryBeforeDeleting(warehouseCatalog);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseCatalogRepository(uow))
                {
                    repository.Delete(warehouseCatalog);
                    uow.Commit();
                }
            }

            if (raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<IWarehouseCatalog>(warehouseCatalog), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IWarehouseCatalog"/>
        /// </summary>
        /// <param name="warehouseCatalogs">
        /// The warehouse catalogs.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        public void Delete(IEnumerable<IWarehouseCatalog> warehouseCatalogs, bool raiseEvents = true)
        {
            var catalogs = warehouseCatalogs.Where(x => x.Key != Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey).ToArray();
            if (!catalogs.Any()) return;

            if (raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<IWarehouseCatalog>(catalogs), this);

            foreach (var catalog in catalogs)
            {
                RemoveVariantsFromCatalogInventoryBeforeDeleting(catalog);
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseCatalogRepository(uow))
                {
                    foreach (var catalog in catalogs)
                    {
                        repository.Delete(catalog);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<IWarehouseCatalog>(catalogs), this);
        }

        /// <summary>
        /// Gets a <see cref="IWarehouseCatalog"/> by its key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouseCatalog"/>.
        /// </returns>
        public IWarehouseCatalog GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateWarehouseCatalogRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of all <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        public IEnumerable<IWarehouseCatalog> GetAll()
        {
            using (var repository = _repositoryFactory.CreateWarehouseCatalogRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IWarehouseCatalog"/> for a given <see cref="IWarehouse"/>.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        public IEnumerable<IWarehouseCatalog> GetByWarehouseKey(Guid warehouseKey)
        {
            using (var repository = _repositoryFactory.CreateWarehouseCatalogRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetWarehouseCatalogsByWarehouseKey(warehouseKey);
            }
        }

        /// <summary>
        /// Removes variants from catalog inventory.
        /// </summary>
        /// <param name="catalog">
        /// The catalog.
        /// </param>
        private void RemoveVariantsFromCatalogInventoryBeforeDeleting(IWarehouseCatalog catalog)
        {
            var variants =
                _productVariantService.GetByWarehouseKey(catalog.WarehouseKey)
                    .Where(pv => pv.CatalogInventories.Any(inv => inv.CatalogKey == catalog.Key)).ToArray();

            if (!variants.Any()) return;

            foreach (var variant in variants)
            {
                variant.RemoveFromCatalogInventory(catalog);
            }

            _productVariantService.Save(variants);
        }
    }
}