using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    public class GatewayProviderService : IGatewayProviderService
    {

        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IShipMethodService _shipMethodService;
        private readonly IShipRateTierService _shipRateTierService;
        private readonly IShipCountryService _shipCountryService;
        private readonly ICountryTaxRateService _countryTaxRateService;
        
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public GatewayProviderService()
            : this(new RepositoryFactory(), new ShipMethodService(), new ShipRateTierService(), new ShipCountryService(), new CountryTaxRateService())
        { }

        public GatewayProviderService(RepositoryFactory repositoryFactory, IShipMethodService shipMethodService, IShipRateTierService shipRateTierService, IShipCountryService shipCountryService, ICountryTaxRateService countryTaxRateService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, shipMethodService, shipRateTierService, shipCountryService, countryTaxRateService)
        { }

        public GatewayProviderService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IShipMethodService shipMethodService, IShipRateTierService shipRateTierService, IShipCountryService shipCountryService, ICountryTaxRateService countryTaxRateService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(shipMethodService, "shipMethodService");
            Mandate.ParameterNotNull(shipRateTierService, "shipRateTierService");
            Mandate.ParameterNotNull(shipCountryService, "shipCountryService");
            Mandate.ParameterNotNull(countryTaxRateService, "countryTaxRateService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _shipMethodService = shipMethodService;
            _shipRateTierService = shipRateTierService;
            _shipCountryService = shipCountryService;
            _countryTaxRateService = countryTaxRateService;
        }


        #region GatewayProvider

        /// <summary>
        /// Saves a single instance of a <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IGatewayProvider gatewayProvider, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IGatewayProvider>(gatewayProvider), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateGatewayProviderRepository(uow))
                {
                    repository.AddOrUpdate(gatewayProvider);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IGatewayProvider>(gatewayProvider), this);
            }
        }

        /// <summary>
        /// Deletes a <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IGatewayProvider gatewayProvider, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IGatewayProvider>(gatewayProvider), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateGatewayProviderRepository(uow))
                {
                    repository.Delete(gatewayProvider);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IGatewayProvider>(gatewayProvider), this);
        }

        /// <summary>
        /// Deletes a collection of <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProviderList"></param>
        /// <param name="raiseEvents"></param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal void Delete(IEnumerable<IGatewayProvider> gatewayProviderList, bool raiseEvents = true)
        {
            var gatewayProviderArray = gatewayProviderList as IGatewayProvider[] ?? gatewayProviderList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IGatewayProvider>(gatewayProviderArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateGatewayProviderRepository(uow))
                {
                    foreach (var gatewayProvider in gatewayProviderArray)
                    {
                        repository.Delete(gatewayProvider);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IGatewayProvider>(gatewayProviderArray), this);
        }

        /// <summary>
        /// Gets a <see cref="IGatewayProvider"/> by it's unique 'Key' (Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IGatewayProvider GetGatewayProviderByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/> by its type (Shipping, Taxation, Payment)
        /// </summary>
        /// <param name="gatewayProviderType"></param>
        /// <returns></returns>
        public IEnumerable<IGatewayProvider> GetGatewayProvidersByType(GatewayProviderType gatewayProviderType)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                var query =
                    Query<IGatewayProvider>.Builder.Where(
                        x =>
                            x.ProviderTfKey ==
                            EnumTypeFieldConverter.GatewayProvider.GetTypeField(gatewayProviderType).TypeKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/> by ship country
        /// </summary>
        /// <param name="shipCountry"></param>
        /// <returns></returns>
        public IEnumerable<IGatewayProvider> GetGatewayProvidersByShipCountry(IShipCountry shipCountry)
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetGatewayProvidersByShipCountryKey(shipCountry.Key);
            }
        }

        /// <summary>
        /// Gets a collection containing all <see cref="IGatewayProvider"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGatewayProvider> GetAllGatewayProviders()
        {
            using (var repository = _repositoryFactory.CreateGatewayProviderRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        #endregion

        #region ShipMethod

        /// <summary>
        /// Creates a <see cref="IShipMethod"/>.  This is useful due to the data constraint
        /// preventing two ShipMethods being created with the same ShipCountry and ServiceCode for any provider.
        /// </summary>
        /// <param name="providerKey">The unique gateway provider key (Guid)</param>
        /// <param name="shipCountry">The <see cref="IShipCountry"/> this ship method is to be associated with</param>
        /// <param name="name">The required name of the <see cref="IShipMethod"/></param>
        /// <param name="serviceCode">The ShipMethods service code</param>
        public Attempt<IShipMethod> CreateShipMethodWithKey(Guid providerKey, IShipCountry shipCountry, string name, string serviceCode)
        {            
            return ((ShipMethodService)_shipMethodService).CreateShipMethodWithKey(providerKey, shipCountry, name, serviceCode);
        }

        /// <summary>
        /// Saves a single <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod"></param>
        public void Save(IShipMethod shipMethod)
        {
           _shipMethodService.Save(shipMethod);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="IShipMethod"/></param>
        public void Save(IEnumerable<IShipMethod> shipMethodList)
        {
            _shipMethodService.Save(shipMethodList);
        }

        

        /// <summary>
        /// Deletes a <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod"></param>
        public void Delete(IShipMethod shipMethod)
        {
            _shipMethodService.Delete(shipMethod);
        }

        /// <summary>
        /// Gets a list of <see cref="IShipMethod"/> objects given a <see cref="IGatewayProvider"/> key and a <see cref="IShipCountry"/> key
        /// </summary>
        /// <returns>A collection of <see cref="IShipMethod"/></returns>
        public IEnumerable<IShipMethod> GetGatewayProviderShipMethods(Guid providerKey, Guid shipCountryKey)
        {
            return _shipMethodService.GetGatewayProviderShipMethods(providerKey, shipCountryKey);
        }

        
        #endregion

        #region ShipRateTier

        /// <summary>
        /// Saves a single <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier"></param>
        public void Save(IShipRateTier shipRateTier)
        {
            _shipRateTierService.Save(shipRateTier);
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTierList"></param>
        public void Save(IEnumerable<IShipRateTier> shipRateTierList)
        {
            _shipRateTierService.Save(shipRateTierList);
        }


        /// <summary>
        /// Deletes a <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier"></param>
        public void Delete(IShipRateTier shipRateTier)
        {
            _shipRateTierService.Delete(shipRateTier);
        }


        /// <summary>
        /// Gets a list of <see cref="IShipRateTier"/> objects given a <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">Guid</param>
        /// <returns>A collection of <see cref="IShipRateTier"/></returns>
        public IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey)
        {
            return _shipRateTierService.GetShipRateTiersByShipMethodKey(shipMethodKey);
        }

        #endregion

        #region ShipCountry

        /// <summary>
        /// Gets a <see cref="IShipCountry"/> by CatalogKey and CountryCode
        /// </summary>
        /// <param name="catalogKey"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public IShipCountry GetShipCountry(Guid catalogKey, string countryCode)
        {
            return _shipCountryService.GetShipCountryByCountryCode(catalogKey, countryCode);
        }

        /// <summary>
        /// Attempts to create a <see cref="ICountryTaxRate"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="providerKey">The unique 'key' (Guid) of the TaxationGatewayProvider</param>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        /// <returns><see cref="Attempt"/> indicating whether or not the creation of the <see cref="ICountryTaxRate"/> with respective success or fail</returns>
        public Attempt<ICountryTaxRate> CreateCountryTaxRateWithKey(Guid providerKey, string countryCode, decimal percentageTaxRate)
        {
            return ((CountryTaxRateService)_countryTaxRateService).CreateCountryTaxRateWithKey(providerKey, countryCode, percentageTaxRate);
        }

        /// <summary>
        /// Gets a <see cref="ICountryTaxRate"/> based on a provider and country code
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the <see cref="IGatewayProvider"/></param>
        /// <param name="countryCode">The country code of the <see cref="ICountryTaxRate"/></param>
        /// <returns><see cref="ICountryTaxRate"/></returns>
        public ICountryTaxRate GetCountryTaxRateByCountryCode(Guid providerKey, string countryCode)
        {
            return _countryTaxRateService.GetCountryTaxRateByCountryCode(providerKey, countryCode);
        }

        /// <summary>
        /// Saves a single <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRate">The <see cref="ICountryTaxRate"/> to be saved</param>
        public void Save(ICountryTaxRate countryTaxRate)
        {
            _countryTaxRateService.Save(countryTaxRate);
        }

        /// <summary>
        /// Gets a collection of <see cref="ICountryTaxRate"/> for a given TaxationGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the TaxationGatewayProvider</param>
        /// <returns>A collection of <see cref="ICountryTaxRate"/></returns>
        public IEnumerable<ICountryTaxRate> GetCountryTaxRatesByProviderKey(Guid providerKey)
        {
            return _countryTaxRateService.GetCountryTaxRatesByProviderKey(providerKey);
        }

        #endregion

        #region Event Handlers

        ///// <summary>
        ///// Occurs after Create
        ///// </summary>
        //public static event TypedEventHandler<IGatewayProviderService, Events.NewEventArgs<IGatewayProvider>> Creating;


        ///// <summary>
        ///// Occurs after Create
        ///// </summary>
        //public static event TypedEventHandler<IGatewayProviderService, Events.NewEventArgs<IGatewayProvider>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IGatewayProviderService, SaveEventArgs<IGatewayProvider>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IGatewayProviderService, SaveEventArgs<IGatewayProvider>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IGatewayProviderService, DeleteEventArgs<IGatewayProvider>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IGatewayProviderService, DeleteEventArgs<IGatewayProvider>> Deleted;

        #endregion
    }
}