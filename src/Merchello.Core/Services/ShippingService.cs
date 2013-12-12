using System;
using System.Collections;
using System.Collections.Generic;
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
    public class ShippingService : IShippingService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly ISettingsService _settingsService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public ShippingService()
            : this(new RepositoryFactory(), new SettingsService())
        { }

        public ShippingService(RepositoryFactory repositoryFactory, ISettingsService settingsService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, settingsService)
        { }

        public ShippingService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ISettingsService settingsService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(settingsService, "settingsService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _settingsService = settingsService;
        }


        #region Shipment


        public void Save(IShipment shipment, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IShipment shipment, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<IShipment> shipmentList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public IShipment GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipment> GetShipmentsForShipMethod(Guid shipMethodKey)
        {
            throw new NotImplementedException();
        }



        public IEnumerable<IShipment> GetByKeys(IEnumerable<Guid> keys)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ShipCountry

        /// <summary>
        /// Saves a single <see cref="shipCountry"/>
        /// </summary>
        /// <param name="shipCountry"></param>
        public void Save(IShipCountry shipCountry)
        {            
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipCountryRepository(uow, _settingsService))
                {
                    repository.AddOrUpdate(shipCountry);
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes a single <see cref="IShipCountry"/> object
        /// </summary>
        /// <param name="shipCountry"></param>
        public void Delete(IShipCountry shipCountry)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipCountryRepository(uow,_settingsService))
                {
                    repository.Delete(shipCountry);
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IShipCountry"/> objects given a <see cref="IWarehouseCatalog"/> key
        /// </summary>
        /// <param name="catalogKey">Guid</param>
        /// <returns>A collection of <see cref="IShipCountry"/></returns>
        public IEnumerable<IShipCountry> GetShipCountriesByCatalogKey(Guid catalogKey)
        {
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(),_settingsService))
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
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(),_settingsService))
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
        public IShipCountry GetShipCountryByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(),_settingsService))
            {
                return repository.Get(key);
            }
        }

        // used for testing
        internal IEnumerable<IShipCountry> GetAllShipCountries()
        {
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(),_settingsService))
            {
                return repository.GetAll();
            }
        }

        #endregion

        #region ShipMethod


        public void Save(IShipMethod shipMethod)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<IShipMethod> shipMethodList)
        {
            throw new NotImplementedException();
        }

        public void Delete(IShipMethod shipMethod)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipMethod> GetShipMethodsByShipCountryKey(Guid shipCountryKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipMethod> GetShipMethodsByGatewayProviderKey(Guid gatewayProviderKey)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region ShipRateTier

        public void Save(IShipRateTier shipRateTier)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<IShipRateTier> shipRateTierList)
        {
            throw new NotImplementedException();
        }

        public void Delete(IShipRateTier shipRateTier)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey)
        {
            throw new NotImplementedException();
        }

        #endregion



        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShippingService, Events.NewEventArgs<IShipment>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IShippingService, Events.NewEventArgs<IShipment>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IShippingService, SaveEventArgs<IShipment>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IShippingService, SaveEventArgs<IShipment>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IShippingService, DeleteEventArgs<IShipment>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IShippingService, DeleteEventArgs<IShipment>> Deleted;

        #endregion
     
    }
}