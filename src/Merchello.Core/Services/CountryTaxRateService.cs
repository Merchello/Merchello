using System;
using System.Collections.Generic;
using System.Threading;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
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

        internal Attempt<ICountryTaxRate> CreateCountryTaxRateWithKey(Guid providerKey, string countryCode, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        internal Attempt<ICountryTaxRate> CreateCountryTaxRateWithKey(Guid providerKey, ICountry country, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Save(ICountryTaxRate countryTaxRate, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<ICountryTaxRate> countryTaxRateList, bool raiseEvents = true)
        {
            throw new NotImplementedException();
        }

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