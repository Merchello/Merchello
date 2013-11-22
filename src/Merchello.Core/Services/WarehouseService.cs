using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Merchello.Core.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Represents the Warehouse Service 
    /// </summary>
    public class WarehouseService : IWarehouseService
    {
        private readonly static ConcurrentDictionary<string, IEnumerable<IProvince>> RegionProvinceCache = new ConcurrentDictionary<string, IEnumerable<IProvince>>();

        private readonly IDatabaseUnitOfWorkProvider _uowProvider;
        private readonly RepositoryFactory _repositoryFactory;

        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public WarehouseService()
            : this(new RepositoryFactory())
        { }

        public WarehouseService(RepositoryFactory repositoryFactory)
            : this(new PetaPocoUnitOfWorkProvider(), repositoryFactory)
        { }

        public WarehouseService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory)
        {
            Mandate.ParameterNotNull(provider, "provider");
            Mandate.ParameterNotNull(repositoryFactory, "repositoryFactory");

            _uowProvider = provider;
            _repositoryFactory = repositoryFactory;

            if (!RegionProvinceCache.IsEmpty) return;

            foreach (RegionElement region in MerchelloConfiguration.Current.Section.RegionalProvinces)
            {
                CacheRegion(region.Code, (from ProvinceElement pe in region.ProvincesConfiguration
                    select new Province(pe.Code, pe.Name)).Cast<IProvince>().ToArray());
            }
        }

        private static void CacheRegion(string code, IProvince[] provinces)
        {
            RegionProvinceCache.AddOrUpdate(code, provinces, (x, y) => provinces);
        }
        

        #region IWarehouseService Members


        /// <summary>
        /// Creates an <see cref="IWarehouse"/> object
        /// </summary>
        public IWarehouse CreateWarehouse(string name)
        {
            Mandate.ParameterNotNull(name, "name");

            var warehouse = new Warehouse()
                {
                    Name = name
                };
                
            Created.RaiseEvent(new Events.NewEventArgs<IWarehouse>(warehouse), this);

            return warehouse;
        }

        /// <summary>
        /// Saves a single <see cref="IWarehouse"/> object
        /// </summary>
        /// <param name="warehouse">The <see cref="IWarehouse"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events.</param>
        public void Save(IWarehouse warehouse, bool raiseEvents = true)
        {
            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IWarehouse>(warehouse), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseRepository(uow))
                {
                    repository.AddOrUpdate(warehouse);
                    uow.Commit();
                }

                if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IWarehouse>(warehouse), this);
            }
        }

        /// <summary>
        /// Saves a collection of <see cref="IWarehouse"/> objects.
        /// </summary>
        /// <param name="warehouseList">Collection of <see cref="Warehouse"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Save(IEnumerable<IWarehouse> warehouseList, bool raiseEvents = true)
        {
            var warehouseArray = warehouseList as IWarehouse[] ?? warehouseList.ToArray();

            if (raiseEvents) Saving.RaiseEvent(new SaveEventArgs<IWarehouse>(warehouseArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseRepository(uow))
                {
                    foreach (var warehouse in warehouseArray)
                    {
                        repository.AddOrUpdate(warehouse);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Saved.RaiseEvent(new SaveEventArgs<IWarehouse>(warehouseArray), this);
        }

        /// <summary>
        /// Deletes a single <see cref="IWarehouse"/> object
        /// </summary>
        /// <param name="warehouse">The <see cref="IWarehouse"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IWarehouse warehouse, bool raiseEvents = true)
        {
            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IWarehouse>( warehouse), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseRepository(uow))
                {
                    repository.Delete( warehouse);
                    uow.Commit();
                }
            }
            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IWarehouse>( warehouse), this);
        }

        /// <summary>
        /// Deletes a collection <see cref="IWarehouse"/> objects
        /// </summary>
        /// <param name="warehouseList">Collection of <see cref="IWarehouse"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        public void Delete(IEnumerable<IWarehouse> warehouseList, bool raiseEvents = true)
        {
            var warehouseArray = warehouseList as IWarehouse[] ?? warehouseList.ToArray();

            if (raiseEvents) Deleting.RaiseEvent(new DeleteEventArgs<IWarehouse>(warehouseArray), this);

            using (new WriteLock(Locker))
            {
                var uow = _uowProvider.GetUnitOfWork();
                using (var repository = _repositoryFactory.CreateWarehouseRepository(uow))
                {
                    foreach (var warehouse in warehouseArray)
                    {
                        repository.Delete(warehouse);
                    }
                    uow.Commit();
                }
            }

            if (raiseEvents) Deleted.RaiseEvent(new DeleteEventArgs<IWarehouse>(warehouseArray), this);
        }

        /// <summary>
        /// Gets a Warehouse by its unique id - pk
        /// </summary>
        /// <param name="key">int Id for the Warehouse</param>
        /// <returns><see cref="IWarehouse"/></returns>
        public IWarehouse GetByKey(Guid key)
        {
            using (var repository = _repositoryFactory.CreateWarehouseRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.Get(key);
            }
        }

        /// <summary>
        /// Gets a list of Warehouse give a list of unique keys
        /// </summary>
        /// <param name="keys">List of unique keys</param>
        /// <returns></returns>
        public IEnumerable<IWarehouse> GetByKeys(IEnumerable<Guid> keys)
        {
            using (var repository = _repositoryFactory.CreateWarehouseRepository(_uowProvider.GetUnitOfWork()))
            {
                return repository.GetAll(keys.ToArray());
            }
        }

        /// <summary>
        /// Gets a list of warehouses associated with a ship method
        /// </summary>
        /// <param name="shipMethodKey">The key of the <see cref="IShipMethod"/></param>
        /// <returns>A collection of <see cref="IWarehouse"/></returns>
        public IEnumerable<IWarehouse> GetWarehousesForShipMethod(Guid shipMethodKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the <see cref="CountryBase" /> for the country code passed.
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="RegionInfo"/> for the country corresponding the the country code passed</returns>
        public ICountryBase GetCountryByCode(string countryCode)
        {
            return new Country(countryCode);
        }

        /// <summary>
        /// Returns a Region collection for all countries
        /// </summary>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<ICountryBase> GetAllCountries()
        {
            return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(culture => new RegionInfo(culture.Name))
                .Select(ri =>
                    new Country(ri.TwoLetterISORegionName, GetProvincesByCountryCode(ri.TwoLetterISORegionName))
                    {
                        ProvinceLabel = GetProvinceLabelForCountry(ri.TwoLetterISORegionName)
                    });
        }

        /// <summary>
        /// Returns a Region collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCountryCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        public IEnumerable<ICountryBase> GetAllCountries(string[] excludeCountryCodes)
        {
            return GetAllCountries().Where(x => !excludeCountryCodes.Contains(x.RegionInfo.TwoLetterISORegionName));
        }

        /// <summary>
        /// True/false indicating whether or not the region has provinces configured in the Merchello.config file
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns></returns>
        private bool CountryHasProvinces(string countryCode)
        {
            return RegionProvinceCache.ContainsKey(countryCode);
        }

        /// <summary>
        /// Returns the province label from the configuration file
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code</param>
        /// <returns></returns>
        private string GetProvinceLabelForCountry(string countryCode)
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
        private IEnumerable<IProvince> GetProvincesByCountryCode(string countryCode)
        {
            return CountryHasProvinces(countryCode) ?
                RegionProvinceCache[countryCode] :
                new List<IProvince>();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Occurs before Delete
        /// </summary>		
        public static event TypedEventHandler<IWarehouseService, DeleteEventArgs<IWarehouse>> Deleting;

        /// <summary>
        /// Occurs after Delete
        /// </summary>
        public static event TypedEventHandler<IWarehouseService, DeleteEventArgs<IWarehouse>> Deleted;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<IWarehouseService, SaveEventArgs<IWarehouse>> Saving;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<IWarehouseService, SaveEventArgs<IWarehouse>> Saved;

        /// <summary>
        /// Occurs before Create
        /// </summary>
        public static event TypedEventHandler<IWarehouseService, Events.NewEventArgs<IWarehouse>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<IWarehouseService, Events.NewEventArgs<IWarehouse>> Created;

        #endregion


     
    }
}