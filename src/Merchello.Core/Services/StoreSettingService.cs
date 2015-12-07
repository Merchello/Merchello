namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Merchello.Core.Configuration;
    using Merchello.Core.Configuration.Outline;
    using Merchello.Core.Events;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents the Store Settings Service
    /// </summary>
    public class StoreSettingService : MerchelloRepositoryService, IStoreSettingService
    {
        #region Fields

        /// <summary>
        /// The region province cache.
        /// </summary>
        private static readonly ConcurrentDictionary<string, IEnumerable<IProvince>> RegionProvinceCache = new ConcurrentDictionary<string, IEnumerable<IProvince>>();

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreSettingService"/> class. 
        /// </summary>
        public StoreSettingService()
            : this(LoggerResolver.Current.Logger)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreSettingService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public StoreSettingService(ILogger logger)
            : this(new RepositoryFactory(), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreSettingService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        public StoreSettingService(ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : this(new RepositoryFactory(logger, sqlSyntax), logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreSettingService"/> class.
        /// </summary>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public StoreSettingService(RepositoryFactory repositoryFactory, ILogger logger)
            : this(new PetaPocoUnitOfWorkProvider(logger), repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreSettingService"/> class.
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
        public StoreSettingService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : this(provider, repositoryFactory, logger, new TransientMessageFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreSettingService"/> class.
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
        public StoreSettingService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger, IEventMessagesFactory eventMessagesFactory)
            : base(provider, repositoryFactory, logger, eventMessagesFactory)
        {
            if (!RegionProvinceCache.IsEmpty) return;

            foreach (RegionElement region in MerchelloConfiguration.Current.Section.RegionalProvinces)
            {
                CacheRegion(
                    region.Code,
                    (from ProvinceElement pe in region.ProvincesConfiguration select new Province(pe.Code, pe.Name)).Cast<IProvince>().ToArray());
            }
        }

        #endregion

        #region Events



        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IStoreSettingService, Events.NewEventArgs<IStoreSetting>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IStoreSettingService, Events.NewEventArgs<IStoreSetting>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IStoreSettingService, SaveEventArgs<IStoreSetting>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IStoreSettingService, SaveEventArgs<IStoreSetting>> Saved;

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IStoreSettingService, DeleteEventArgs<IStoreSetting>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IStoreSettingService, DeleteEventArgs<IStoreSetting>> Deleted;

        #endregion

        /// <summary>
        /// True/false indicating whether or not the region has provinces configured in the Merchello.config file
        /// </summary>
        /// <param name="countryCode">
        /// The two letter ISO Region code (country code)
        /// </param>
        /// <returns>
        /// A value indicating whether or not the country has provinces.
        /// </returns>
        public static bool CountryHasProvinces(string countryCode)
        {
            return RegionProvinceCache.ContainsKey(countryCode);
        }

        /// <summary>
        /// Returns the province label from the configuration file
        /// </summary>
        /// <param name="countryCode">
        /// The two letter ISO Region code
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetProvinceLabelForCountry(string countryCode)
        {
            return CountryHasProvinces(countryCode)
                ? MerchelloConfiguration.Current.Section.RegionalProvinces[countryCode].ProvinceLabel
                : string.Empty;
        }

        /// <summary>
        /// Returns a collection of <see cref="IProvince"/> given a region code
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns>A collection of <see cref="IProvince"/></returns>
        public static IEnumerable<IProvince> GetProvincesByCountryCode(string countryCode)
        {
            return CountryHasProvinces(countryCode) ?
                RegionProvinceCache[countryCode] :
                new List<IProvince>();
        }

        /// <summary>
        /// Returns the currency format
        /// </summary>
        /// <param name="currency">
        /// The <see cref="ICurrency"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICurrencyFormat"/>.
        /// </returns>
        public ICurrencyFormat GetCurrencyFormat(ICurrency currency)
        {
            var query = MerchelloConfiguration.Current.Section.CurrencyFormats
                        .Cast<CurrencyFormatElement>()
                        .FirstOrDefault(cf => cf.CurrencyCode.Equals(currency.CurrencyCode, StringComparison.OrdinalIgnoreCase));

            return query != null ? 
                new CurrencyFormat(query.Format, query.Symbol) : 
                CurrencyFormat.CreateDefault(currency.Symbol);
        }

        /// <summary>
        /// Creates a store setting and persists it to the database
        /// </summary>
        /// <param name="name">The settings name</param>
        /// <param name="value">The settings value</param>
        /// <param name="typeName">The type name</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IStoreSetting"/></returns>
        public IStoreSetting CreateStoreSettingWithKey(string name, string value, string typeName, bool raiseEvents)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(value, "value");
            Mandate.ParameterNotNullOrEmpty(typeName, "typeName");

            var storeSetting = new StoreSetting()
            {
                Name = name,
                Value = value,
                TypeName = typeName
            };

            if (raiseEvents)
                if (Creating.IsRaisedEventCancelled(new Events.NewEventArgs<IStoreSetting>(storeSetting), this))
                {
                    storeSetting.WasCancelled = true;
                    return storeSetting;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreSettingRepository(uow))
                {
                    repository.AddOrUpdate(storeSetting);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Created.RaiseEvent(new Events.NewEventArgs<IStoreSetting>(storeSetting), this);            

            return storeSetting;
        }

        /// <summary>
        /// Saves a single <see cref="IStoreSetting"/> object
        /// </summary>
        /// <param name="storeSetting">The <see cref="IStoreSetting"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IStoreSetting storeSetting, bool raiseEvents = true)
        {
            if (raiseEvents)
                if (Saving.IsRaisedEventCancelled(new SaveEventArgs<IStoreSetting>(storeSetting), this))
                {
                    ((StoreSetting)storeSetting).WasCancelled = true;
                    return;
                }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreSettingRepository(uow))
                {
                    repository.AddOrUpdate(storeSetting);
                    uow.Commit();
                }
            }

            if (raiseEvents)
                Saved.IsRaisedEventCancelled(new SaveEventArgs<IStoreSetting>(storeSetting), this);
        }

        /// <summary>
        /// Deletes a <see cref="IStoreSetting"/>
        /// </summary>
        /// <param name="storeSetting">
        /// The store Setting.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        public void Delete(IStoreSetting storeSetting, bool raiseEvents = true)
        {
            if (raiseEvents)
            {
                if (Deleting.IsRaisedEventCancelled(new DeleteEventArgs<IStoreSetting>(storeSetting), this))
                {
                    ((StoreSetting)storeSetting).WasCancelled = true;
                    return;
                }
            }

            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreSettingRepository(uow))
                {
                    repository.Delete(storeSetting);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IStoreSetting>(storeSetting), this);
        }

        /// <summary>
        /// Gets a <see cref="IStoreSetting"/> by it's unique 'Key' (GUID)
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IStoreSetting"/>.
        /// </returns>
        public IStoreSetting GetByKey(Guid key)
        {
            using (var repository = RepositoryFactory.CreateStoreSettingRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a collection of all <see cref="IStoreSetting"/>
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IStoreSetting"/>.
        /// </returns>
        public IEnumerable<IStoreSetting> GetAll()
        {
            using (var repository = RepositoryFactory.CreateStoreSettingRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetAll();
            }
        }

        /// <summary>
        /// Gets the next usable InvoiceNumber
        /// </summary>
        /// <param name="invoicesCount">
        /// The invoices Count.
        /// </param>
        /// <returns>
        /// The next invoice number.
        /// </returns>
        public virtual int GetNextInvoiceNumber(int invoicesCount = 1)
        {
            var invoiceNumber = 0;
            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreSettingRepository(uow))
                {
                    using (var validationRepository = RepositoryFactory.CreateInvoiceRepository(uow))
                    { 
                        invoiceNumber = repository.GetNextInvoiceNumber(Core.Constants.StoreSettingKeys.NextInvoiceNumberKey, validationRepository.GetMaxDocumentNumber, invoicesCount);
                    }

                    uow.Commit();
                }
            }

            return invoiceNumber;
        }

        /// <summary>
        /// Gets the next usable OrderNumber
        /// </summary>
        /// <param name="ordersCount">
        /// The orders Count.
        /// </param>
        /// <returns>
        /// The next order number.
        /// </returns>
        public virtual int GetNextOrderNumber(int ordersCount = 1)
        {
            var orderNumber = 0;
            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreSettingRepository(uow))
                {
                    using (var validationRepository = RepositoryFactory.CreateOrderRepository(uow))
                    {
                        orderNumber = repository.GetNextOrderNumber(Core.Constants.StoreSettingKeys.NextOrderNumberKey, validationRepository.GetMaxDocumentNumber, ordersCount);
                    }

                    uow.Commit();
                }
            }

            return orderNumber;
        }

        /// <summary>
        /// Gets the next usable ShipmentNumber.
        /// </summary>
        /// <param name="shipmentsCount">
        /// The shipments count.
        /// </param>
        /// <returns>
        /// The next shipment number.
        /// </returns>
        public int GetNextShipmentNumber(int shipmentsCount = 1)
        {
            var shipmentNumber = 0;
            using (new WriteLock(Locker))
            {
                var uow = UowProvider.GetUnitOfWork();
                using (var repository = RepositoryFactory.CreateStoreSettingRepository(uow))
                {
                    using (var validationRepository = RepositoryFactory.CreateShipmentRepository(uow))
                    {
                        shipmentNumber = repository.GetNextShipmentNumber(Core.Constants.StoreSettingKeys.NextShipmentNumberKey, validationRepository.GetMaxDocumentNumber, shipmentsCount);
                    }

                    uow.Commit();
                }
            }

            return shipmentNumber;
        }

        /// <summary>
        /// Gets the complete collection of registered type fields
        /// </summary>
        /// <returns>
        /// Gets the collection of all type fields.
        /// </returns>
        public IEnumerable<ITypeField> GetTypeFields()
        {
            using (var repository = RepositoryFactory.CreateStoreSettingRepository(UowProvider.GetUnitOfWork()))
            {
                return repository.GetTypeFields();
            }
        }

        /// <summary>
        /// Returns the <see cref="CountryBase" /> for the country code passed.
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="RegionInfo"/> for the country corresponding the the country code passed</returns>
        public ICountry GetCountryByCode(string countryCode)
        {
            return new Country(countryCode, GetProvincesByCountryCode(countryCode))
            {
                ProvinceLabel = GetProvinceLabelForCountry(countryCode)
            };
        }

        /// <summary>
        /// Gets a collection of all  <see cref="ICountry"/>
        /// </summary>
        /// <returns>
        /// The collection of all countries.
        /// </returns>
        public IEnumerable<ICountry> GetAllCountries()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name))
                .Select(ri => GetCountryByCode(ri.TwoLetterISORegionName)).DistinctBy(x => x.CountryCode);
        }

        /// <summary>
        /// Gets a collection of all <see cref="ICurrency"/>
        /// </summary>
        /// <returns>
        /// The collection of all currencies.
        /// </returns>
        public IEnumerable<ICurrency> GetAllCurrencies()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                              .Select(culture => new RegionInfo(culture.Name))
                              .Select(
                                  ri => new Currency(ri.ISOCurrencySymbol, ri.CurrencySymbol, ri.CurrencyEnglishName))
                              .DistinctBy(x => x.CurrencyCode);
        }

        /// <summary>
        /// Gets a <see cref="ICurrency"/> for the currency code passed
        /// </summary>
        /// <param name="currencyCode">The ISO Currency Code (e.g. USD)</param>
        /// <returns>The <see cref="ICurrency"/></returns>
        public ICurrency GetCurrencyByCode(string currencyCode)
        {
            return GetAllCurrencies().FirstOrDefault(x => x.CurrencyCode == currencyCode);
        }

        /// <summary>
        /// Returns a Region collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCountryCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<ICountry> GetAllCountries(string[] excludeCountryCodes)
        {
            return GetAllCountries().Where(x => !excludeCountryCodes.Contains(x.CountryCode));
        }        

        /// <summary>
        /// The cache region.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="provinces">
        /// The provinces.
        /// </param>
        private static void CacheRegion(string code, IProvince[] provinces)
        {
            RegionProvinceCache.AddOrUpdate(code, provinces, (x, y) => provinces);
        }
    }
}