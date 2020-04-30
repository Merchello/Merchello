namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// The tax method service.
    /// </summary>
    internal class TaxMethodService : MerchelloRepositoryService, ITaxMethodService
    {
        /// <summary>
        /// The thread locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxMethodService"/> class.
        /// </summary>
        public TaxMethodService()
            : this(LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxMethodService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public TaxMethodService(ILogger logger)
            : this(new RepositoryFactory(), logger, new StoreSettingService(logger))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxMethodService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        public TaxMethodService(RepositoryFactory repositoryFactory, ILogger logger, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger, storeSettingService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxMethodService"/> class.
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
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        public TaxMethodService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IStoreSettingService storeSettingService)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory(), storeSettingService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxMethodService"/> class.
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
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        public TaxMethodService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory, IStoreSettingService storeSettingService)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
            Ensure.ParameterNotNull(storeSettingService, "storeSettingService");
            _storeSettingService = storeSettingService;
        }

        #endregion

        #region Event Handlers



        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ITaxMethodService, Events.NewEventArgs<ITaxMethod>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ITaxMethodService, Events.NewEventArgs<ITaxMethod>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ITaxMethodService, SaveEventArgs<ITaxMethod>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ITaxMethodService, SaveEventArgs<ITaxMethod>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ITaxMethodService, DeleteEventArgs<ITaxMethod>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ITaxMethodService, DeleteEventArgs<ITaxMethod>> Deleted;

        #endregion              

        /// <summary>
        /// Saves a single <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(ITaxMethod taxMethod, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<ITaxMethod>(taxMethod), this))
            {
                ((TaxMethod)taxMethod).WasCancelled = true;
                return;
            }

            taxMethod.Name = string.IsNullOrEmpty(taxMethod.Name)
                                 ? GetTaxMethodName(taxMethod)
                                 : taxMethod.Name;

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateTaxMethodRepository(uow))
                {
                    repository.AddOrUpdate(taxMethod);
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ITaxMethod>(taxMethod), this);

        }

        /// <summary>
        /// Saves a collection of <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="countryTaxRateList">A collection of <see cref="ITaxMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ITaxMethod> countryTaxRateList, bool raiseEvents = true)
        {
            var taxMethodsArray = countryTaxRateList as ITaxMethod[] ?? countryTaxRateList.ToArray();
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ITaxMethod>(taxMethodsArray), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateTaxMethodRepository(uow))
                {
                    foreach (var countryTaxRate in taxMethodsArray)
                    {
                        repository.AddOrUpdate(countryTaxRate);    
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ITaxMethod>(taxMethodsArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ITaxMethod taxMethod, bool raiseEvents = true)
        {
            if (raiseEvents)
            if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<ITaxMethod>(taxMethod), this))
            {
                ((TaxMethod) taxMethod).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateTaxMethodRepository(uow))
                {
                    repository.Delete(taxMethod);
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<ITaxMethod>(taxMethod), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethods">The collection of <see cref="ITaxMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<ITaxMethod> taxMethods, bool raiseEvents = true)
        {
            var methods = taxMethods as ITaxMethod[] ?? taxMethods.ToArray();

            if (raiseEvents)
            Deleting.RaiseEvent(new DeleteEventArgs<ITaxMethod>(methods), this);

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateTaxMethodRepository(uow))
                {
                    foreach (var method in methods)
                    {
                        repository.Delete(method);
                    }

                    uow.Commit();
                }
            }

            if (raiseEvents)
            Deleted.RaiseEvent(new DeleteEventArgs<ITaxMethod>(methods), this);
        }

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="key">The unique 'key' (GUID) of the <see cref="ITaxMethod"/></param>
        /// <returns><see cref="ITaxMethod"/></returns>
        public ITaxMethod GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateTaxMethodRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets the collection of all tax methods
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ITaxMethod}"/>.
        /// </returns>
        public IEnumerable<ITaxMethod> GetAll()
        {
            using (var repository = RepositoryFactory.CreateTaxMethodRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> based on a provider and country code
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the <see cref="IGatewayProviderSettings"/></param>
        /// <param name="countryCode">The country code of the <see cref="ITaxMethod"/></param>
        /// <returns><see cref="ITaxMethod"/></returns>
        public ITaxMethod GetTaxMethodByCountryCode(Guid providerKey, string countryCode)
        {
            using (var repository = RepositoryFactory.CreateTaxMethodRepository(UowProvider.GetUnitOfWork()))
            {
                var allTaxMethods = repository.GetAll().ToArray();

                if (!allTaxMethods.Any()) return null;
                var specific = allTaxMethods.FirstOrDefault(x => x.ProviderKey.Equals(providerKey) && x.CountryCode.Equals(countryCode));

                return specific ??
                       allTaxMethods.FirstOrDefault(
                           x =>
                           x.ProviderKey.Equals(providerKey) &&
                           x.CountryCode.Equals(Core.Constants.CountryCodes.EverywhereElse));
            }
        }

        /// <summary>
        /// Get tax method for product pricing.
        /// </summary>
        /// <returns>
        /// The <see cref="ITaxMethod"/> or null if no tax method is found
        /// </returns>
        /// <remarks>
        /// There can be only one =)
        /// </remarks>
        public ITaxMethod GetTaxMethodForProductPricing()
        {
            using (var repository = RepositoryFactory.CreateTaxMethodRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<ITaxMethod>.Builder.Where(x => x.ProductTaxMethod);
                return repository.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets a collection <see cref="ITaxMethod"/> based on a provider and country code
        /// </summary>
        /// <param name="countryCode">The country code of the <see cref="ITaxMethod"/></param>
        /// <returns><see cref="ITaxMethod"/></returns>
        /// <remarks>
        /// 
        /// There should only ever be one - but we've left this open
        /// 
        /// </remarks>
        public IEnumerable<ITaxMethod> GetTaxMethodsByCountryCode(string countryCode)
        {
            using (var repository = RepositoryFactory.CreateTaxMethodRepository(UowProvider.GetUnitOfWork()))
            {
                var query =
                    Query<ITaxMethod>.Builder.Where(x => x.CountryCode == countryCode || x.CountryCode == Constants.CountryCodes.EverywhereElse);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="ITaxMethod"/> for a given TaxationGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the TaxationGatewayProvider</param>
        /// <returns>A collection of <see cref="ITaxMethod"/></returns>
        public IEnumerable<ITaxMethod> GetTaxMethodsByProviderKey(Guid providerKey)
        {
            using (var repository = RepositoryFactory.CreateTaxMethodRepository(UowProvider.GetUnitOfWork()))
            {
                var query = Query<ITaxMethod>.Builder.Where(x => x.ProviderKey == providerKey);

                return repository.GetByQuery(query);
            }
        }

        /// <summary>
        /// Attempts to create a <see cref="ITaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="providerKey">The unique 'key' (GUID) of the TaxationGatewayProvider</param>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="Attempt"/> indicating whether or not the creation of the <see cref="ITaxMethod"/> with respective success or fail</returns>
        internal Attempt<ITaxMethod> CreateTaxMethodWithKey(Guid providerKey, string countryCode, decimal percentageTaxRate, bool raiseEvents = true)
        {
            var country = _storeSettingService.GetCountryByCode(countryCode);
            return country == null
                ? Attempt<ITaxMethod>.Fail(new ArgumentException("Could not create a country for country code '" + countryCode + "'"))
                : CreateTaxMethodWithKey(providerKey, country, percentageTaxRate, raiseEvents);
        }

        /// <summary>
        /// The create tax method with key.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="country">
        /// The country.
        /// </param>
        /// <param name="percentageTaxRate">
        /// The percentage tax rate.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        internal Attempt<ITaxMethod> CreateTaxMethodWithKey(Guid providerKey, ICountry country, decimal percentageTaxRate, bool raiseEvents = true)
        {
            if (CountryTaxRateExists(providerKey, country.CountryCode)) return Attempt<ITaxMethod>.Fail(new ConstraintException("A TaxMethod already exists for the provider for the countryCode '" + country.CountryCode + "'"));

            var taxMethod = new TaxMethod(providerKey, country.CountryCode)
            {
                Name = country.CountryCode == "ELSE" ? "Everywhere Else" : country.Name,
                PercentageTaxRate = percentageTaxRate,
                Provinces = country.Provinces.ToTaxProvinceCollection()
            };

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ITaxMethod>(taxMethod), this))
                {
                    taxMethod.WasCancelled = false;
                    return Attempt<ITaxMethod>.Fail(taxMethod);
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateTaxMethodRepository(uow))
                {
                    repository.AddOrUpdate(taxMethod);
                    uow.Commit();
                }
            }

            if (raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<ITaxMethod>(taxMethod), this);

            return Attempt<ITaxMethod>.Succeed(taxMethod);
        }

        /// <summary>
        /// The get tax method name.
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTaxMethodName(ITaxMethod taxMethod)
        {
            if (taxMethod.CountryCode == "ELSE") return "Everywhere Else";
            var country = _storeSettingService.GetCountryByCode(taxMethod.CountryCode);
            return country.Name;
        }

        /// <summary>
        /// The country tax rate exists.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CountryTaxRateExists(Guid providerKey, string countryCode)
        {
            using (var repository = RepositoryFactory.CreateTaxMethodRepository(UowProvider.GetUnitOfWork()))
            {
                var allTaxMethods = repository.GetAll().ToArray();

                if (!allTaxMethods.Any()) return false;
                return allTaxMethods.FirstOrDefault(x => x.ProviderKey.Equals(providerKey) && x.CountryCode.Equals(countryCode)) != null;
            }
        }
    }
}