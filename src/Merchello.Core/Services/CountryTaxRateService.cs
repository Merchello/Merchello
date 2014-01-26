using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    internal class CountryTaxRateService : ICountryTaxRateService
    {
        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IStoreSettingService _storeSettingService;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

         public CountryTaxRateService()
            : this(new RepositoryFactory(), new StoreSettingService())
        { }

        public CountryTaxRateService(RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory, storeSettingService)
        { }

        public CountryTaxRateService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");
            Mandate.ParameterNotNull(storeSettingService, "settingsService");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;
            _storeSettingService = storeSettingService;
        }

        /// <summary>
        /// Attempts to create a <see cref="ICountryTaxRate"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="providerKey">The unique 'key' (Guid) of the TaxationGatewayProvider</param>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="Attempt"/> indicating whether or not the creation of the <see cref="ICountryTaxRate"/> with respective success or fail</returns>
        internal Attempt<ICountryTaxRate> CreateCountryTaxRateWithKey(Guid providerKey, string countryCode, decimal percentageTaxRate, bool raiseEvents = true)
        {

            var country = _storeSettingService.GetCountryByCode(countryCode);
            return country == null
                ? Attempt<ICountryTaxRate>.Fail(new ArgumentException("Could not create a country for country code '" + countryCode + "'"))
                : CreateCountryTaxRateWithKey(providerKey, country, percentageTaxRate, raiseEvents);
        }

        internal Attempt<ICountryTaxRate> CreateCountryTaxRateWithKey(Guid providerKey, ICountry country, decimal percentageTaxRate, bool raiseEvents = true)
        {
            if(CountryTaxRateExists(providerKey, country.CountryCode)) return Attempt<ICountryTaxRate>.Fail(new ConstraintException("A CountryTaxRate already exists for the provider for the countryCode '" + country.CountryCode + "'"));

            var countryTaxRate = new CountryTaxRate(providerKey, country.CountryCode)
                {
                    PercentageTaxRate = percentageTaxRate
                };

            if(raiseEvents)
            if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<ICountryTaxRate>(countryTaxRate), this))
            {
                countryTaxRate.WasCancelled = false;
                return Attempt<ICountryTaxRate>.Fail(countryTaxRate);
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCountryTaxRateRepository(uow))
                {
                    repository.AddOrUpdate(countryTaxRate);
                    uow.Commit();
                }
            }

            if(raiseEvents) Created.RaiseEvent(new Events.NewEventArgs<ICountryTaxRate>(countryTaxRate), this);

            return Attempt<ICountryTaxRate>.Succeed(countryTaxRate);
        }

        private bool CountryTaxRateExists(Guid providerKey, string countryCode)
        {
            using (var repository = _repositoryFactory.CreateCountryTaxRateRepository(_uowProvider.GetUnitOfWork()))
            {
                var query =
                    Query<ICountryTaxRate>.Builder.Where(x => x.Code == countryCode && x.ProviderKey == providerKey);

                return repository.GetByQuery(query).Any();
            }
        }

        /// <summary>
        /// Saves a single <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRate">The <see cref="ICountryTaxRate"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(ICountryTaxRate countryTaxRate, bool raiseEvents = true)
        {
            if(raiseEvents)
            if (Saving.IsRaisedEventCancelled(new SaveEventArgs<ICountryTaxRate>(countryTaxRate), this))
            {
                ((CountryTaxRate) countryTaxRate).WasCancelled = true;
                return;
            }

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCountryTaxRateRepository(uow))
                {
                    repository.AddOrUpdate(countryTaxRate);
                    uow.Commit();
                }
            }

            if(raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICountryTaxRate>(countryTaxRate), this);

        }

        /// <summary>
        /// Saves a collection of <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRateList">A collection of <see cref="ICountryTaxRate"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<ICountryTaxRate> countryTaxRateList, bool raiseEvents = true)
        {
            var countryTaxRatesArray = countryTaxRateList as ICountryTaxRate[] ?? countryTaxRateList.ToArray();
            if(raiseEvents) Saving.RaiseEvent(new SaveEventArgs<ICountryTaxRate>(countryTaxRatesArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateCountryTaxRateRepository(uow))
                {
                    foreach (var countryTaxRate in countryTaxRatesArray)
                    {
                        repository.AddOrUpdate(countryTaxRate);    
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<ICountryTaxRate>(countryTaxRatesArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRate">The <see cref="ICountryTaxRate"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(ICountryTaxRate countryTaxRate, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public ICountryTaxRate GetByKey(Guid key)
        {
            throw new NotImplementedException();
        }

        public ICountryTaxRate GetCountryTaxRateByCountryCode(Guid providerKey, string countryCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICountryTaxRate> GetCountryTaxRatesByProviderKey(Guid providerKey)
        {
            throw new NotImplementedException();
        }


        #region Event Handlers

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICountryTaxRateService, Events.NewEventArgs<ICountryTaxRate>> Creating;


        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<ICountryTaxRateService, Events.NewEventArgs<ICountryTaxRate>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<ICountryTaxRateService, SaveEventArgs<ICountryTaxRate>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<ICountryTaxRateService, SaveEventArgs<ICountryTaxRate>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<ICountryTaxRateService, DeleteEventArgs<ICountryTaxRate>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<ICountryTaxRateService, DeleteEventArgs<ICountryTaxRate>> Deleted;

        #endregion

    }
}