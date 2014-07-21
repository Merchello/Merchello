namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Models;
    using Models.Interfaces;
    using Persistence;
    using Persistence.Querying;
    using Persistence.UnitOfWork;
    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// The ship country service.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    internal class ShipCountryService : IShipCountryService
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The database unit of work provider.
        /// </summary>
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;

        /// <summary>
        /// The repository factory.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;


        /// <summary>
        /// Initializes a new instance of the <see cref="ShipCountryService"/> class.
        /// </summary>
        public ShipCountryService()
            : this(new RepositoryFactory(), new StoreSettingService())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipCountryService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        public ShipCountryService(RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, storeSettingService)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipCountryService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        public ShipCountryService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(storeSettingService, "settingsService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _storeSettingService = storeSettingService;
        }

        /// <summary>
        /// The create ship country with key.
        /// </summary>
        /// <param name="warehouseCatalogKey">
        /// The warehouse catalog key.
        /// </param>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        internal Attempt<IShipCountry> CreateShipCountryWithKey(Guid warehouseCatalogKey, string countryCode, bool raiseEvents = true)
        {
            var country = _storeSettingService.GetCountryByCode(countryCode);
            return country == null 
                ? Attempt<IShipCountry>.Fail(new ArgumentException("Could not create a country for country code '" + countryCode + "'")) 
                : CreateShipCountryWithKey(warehouseCatalogKey, country, raiseEvents);
        }

        /// <summary>
        /// The create ship country with key.
        /// </summary>
        /// <param name="warehouseCatalogKey">
        /// The warehouse catalog key.
        /// </param>
        /// <param name="country">
        /// The country.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
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

            var sc = GetShipCountriesByCatalogKey(warehouseCatalogKey).FirstOrDefault(x => x.CountryCode.Equals(country.CountryCode));
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

            if (raiseEvents)
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
            var shipCountries = GetShipCountriesByCatalogKey(catalogKey).ToArray();

            if (!shipCountries.Any()) return null;
            var specific = shipCountries.FirstOrDefault(x => x.CountryCode.Equals(countryCode));

            return specific ?? shipCountries.FirstOrDefault(x => x.CountryCode.Equals(Core.Constants.CountryCodes.EverywhereElse));
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