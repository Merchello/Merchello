using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
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
        private readonly IStoreSettingService _storeSettingService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public GatewayProviderService()
            : this(new RepositoryFactory(), new StoreSettingService())
        { }

        public GatewayProviderService(RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, storeSettingService)
        { }

        public GatewayProviderService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(storeSettingService, "settingsService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _storeSettingService = storeSettingService;
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
        /// Saves a single <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod"></param>
        public void Save(IShipMethod shipMethod)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipMethodRepository(uow))
                {
                    repository.AddOrUpdate(shipMethod);
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="IShipMethod"/></param>
        public void Save(IEnumerable<IShipMethod> shipMethodList)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipMethodRepository(uow))
                {
                    foreach (var shipMethod in shipMethodList)
                    {
                        repository.AddOrUpdate(shipMethod);
                    }
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Saves a single <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier"></param>
        public void Save(IShipRateTier shipRateTier)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipRateTierRepository(uow))
                {
                    repository.AddOrUpdate(shipRateTier);
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTierList"></param>
        public void Save(IEnumerable<IShipRateTier> shipRateTierList)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipRateTierRepository(uow))
                {
                    foreach (var shipRateTier in shipRateTierList)
                    {
                        repository.AddOrUpdate(shipRateTier);    
                    }
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes a <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod"></param>
        public void Delete(IShipMethod shipMethod)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipMethodRepository(uow))
                {
                    repository.Delete(shipMethod);
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes a <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier"></param>
        public void Delete(IShipRateTier shipRateTier)
        {
            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateShipRateTierRepository(uow))
                {
                    repository.Delete(shipRateTier);
                    uow.Commit();
                }
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IShipMethod"/> objects given a <see cref="IGatewayProvider"/> key and a <see cref="IShipCountry"/> key
        /// </summary>
        /// <returns>A collection of <see cref="IShipMethod"/></returns>
        public IEnumerable<IShipMethod> GetGatewayProviderShipMethods(Guid providerKey, Guid shipCountryKey)
        {
            using (var repository = _repositoryFactory.CreateShipMethodRepository(_uowProvider.GetUnitOfWork()))
            {
                var query =
                    Query<IShipMethod>.Builder.Where(
                        x => x.ProviderKey == providerKey && x.ShipCountryKey == shipCountryKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="IShipRateTier"/> objects given a <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">Guid</param>
        /// <returns>A collection of <see cref="IShipRateTier"/></returns>
        public IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey)
        {
            using (var repository = _repositoryFactory.CreateShipRateTierRepository(_uowProvider.GetUnitOfWork()))
            {
                var query = Query<IShipRateTier>.Builder.Where(x => x.ShipMethodKey == shipMethodKey);
                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a <see cref="IShipCountry"/> by CatalogKey and CountryCode
        /// </summary>
        /// <param name="catalogKey"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// TODO this should be refactored out of this service, but it was needed for ShippingGatewayProviders
        /// 
        /// </remarks>
        public IShipCountry GetShipCountry(Guid catalogKey, string countryCode)
        {
            using (var repository = _repositoryFactory.CreateShipCountryRepository(_uowProvider.GetUnitOfWork(),
                                                                                new StoreSettingService(_uowProvider,
                                                                                                        _repositoryFactory))
                )
            {
                var query =
                    Query<IShipCountry>.Builder.Where(x => x.CatalogKey == catalogKey && x.CountryCode == countryCode);

                return repository.GetByQuery(query).FirstOrDefault();
            }
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