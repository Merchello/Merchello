using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    internal class ShipCountryService : IShipCountryService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IStoreSettingService _storeSettingService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public ShipCountryService()
            : this(new RepositoryFactory(), new StoreSettingService())
        { }

        public ShipCountryService(RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, storeSettingService)
        { }

        public ShipCountryService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(storeSettingService, "settingsService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _storeSettingService = storeSettingService;
        }

        internal Attempt<IShipCountry> CreateShipCountryWithKey(Guid warehouseCatalogKey, string countryCode, bool raiseEvents = true)
        {
            var country = _storeSettingService.GetCountryByCode(countryCode);
            return country == null 
                ? Attempt<IShipCountry>.Fail(new ArgumentException("Could not create a country for country code '" + countryCode + "'")) 
                : CreateShipCountryWithKey(warehouseCatalogKey, country, raiseEvents);
        }

        internal Attempt<IShipCountry> CreateShipCountryWithKey(Guid warehouseCatalogKey, ICountry country, bool raiseEvents = true)
        {
            Mandate.ParameterCondition(warehouseCatalogKey != Guid.Empty, "warehouseCatalog");
            if (country == null) return Attempt<IShipCountry>.Fail(new ArgumentNullException("country"));

            var shipCountry = new ShipCountry(warehouseCatalogKey, country);

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IShipCountry>(shipCountry), this))
                {
                    shipCountry.WasCancelled = true;
                    return Attempt<IShipCountry>.Fail(shipCountry);
                }

            // verify that a ShipCountry does not already exist for this pair
            var sc = GetShipCountryByCountryCode(warehouseCatalogKey, country.CountryCode);
            if (sc != null)
                return
                    Attempt<IShipCountry>.Fail(
                        new ConstraintException("A ShipCountry with CountryCode '" + country.CountryCode +
                                                "' is already associate with this WarehouseCatalog"));

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipCountryRepository(uow, _storeSettingService))
                {
                    repository.AddOrUpdate(shipCountry);
                    uow.Commit();
                }
            }

            if(raiseEvents)
                Created.RaiseEvent(new Events.NewEventArgs<IShipCountry>(shipCountry), this);

            return Attempt<IShipCountry>.Succeed(shipCountry);
        }

        /// <summary>
        /// Saves a single <see cref="shipCountry"/>
        /// </summary>
        /// <param name="shipCountry"></param>
        /// <param name="raiseEvents"></param>
        public void Save(IShipCountry shipCountry, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IShipCountry>(shipCountry), this))
            {
                ((ShipCountry) shipCountry).WasCancelled = true;
                return;
            }
            
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipCountryRepository(uow, _storeSettingService))
                {
                    repository.AddOrUpdate(shipCountry);
                    uow.Commit();
                }
            }

            if(raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IShipCountry>(shipCountry), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IShipCountry"/> object
        /// </summary>
        /// <param name="shipCountry"></param>
        /// <param name="raiseEvents"></param>
        public void Delete(IShipCountry shipCountry, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IShipCountry>(shipCountry), this))
            {
                ((ShipCountry) shipCountry).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipCountryRepository(uow, _storeSettingService))
                {
                    repository.Delete(shipCountry);
                    uow.Commit();
                }
            }
            
            if(raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IShipCountry>(shipCountry), this);
        }

        /// <summary>
        /// Gets a list of <see cref="IShipCountry"/> objects given a <see cref="IWarehouseCatalog"/> key
        /// </summary>
        /// <param name="catalogKey">Guid</param>
        /// <returns>A collection of <see cref="IShipCountry"/></returns>
        public IEnumerable<IShipCountry> GetShipCountriesByCatalogKey(Guid catalogKey)
        {
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(), _storeSettingService))
            {
                var query = Query<IShipCountry>.Builder.Where(x => x.CatalogKey == catalogKey);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a single <see cref="IShipCountry"/>
        /// </summary>
        /// <param name="catalogKey">The warehouse catalog key (guid)</param>
        /// <param name="countryCode">The two letter ISO country code</param>
        /// <returns></returns>
        public IShipCountry GetShipCountryByCountryCode(Guid catalogKey, string countryCode)
        {
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(), _storeSettingService))
            {
                var query = Query<IShipCountry>.Builder.Where(x => x.CatalogKey == catalogKey && x.CountryCode == countryCode);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }


        /// <summary>
        /// Gets a single <see cref="IShipCountry"/> by it's unique key (Guid pk)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IShipCountry GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(), _storeSettingService))
            {
                return repository.Get(key);
            }
        }

        // used for testing
        internal IEnumerable<IShipCountry> GetAllShipCountries()
        {
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(), _storeSettingService))
            {
                return repository.GetAll();
            }
        }


        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShipCountryService, Events.NewEventArgs<IShipCountry>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShipCountryService, Events.NewEventArgs<IShipCountry>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShipCountryService, SaveEventArgs<IShipCountry>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShipCountryService, SaveEventArgs<IShipCountry>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShipCountryService, DeleteEventArgs<IShipCountry>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShipCountryService, DeleteEventArgs<IShipCountry>> Deleted;

        #endregion

    }
}