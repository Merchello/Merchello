using System;
using System.Collections.Generic;
using System.Globalization;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the SettingsService, which provides access to operations involving configurable Merchello configurations and settings
    /// </summary>
    public interface IStoreSettingService : IService
    {
        /// <summary>
        /// Creates a store setting and persists it to the database
        /// </summary>
        /// <param name="name">The settings name</param>
        /// <param name="value">The settings value</param>
        /// <param name="typeName">The type name</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IStoreSetting"/></returns>
        IStoreSetting CreateStoreSettingWithKey(string name, string value, string typeName, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IStoreSetting"/> object
        /// </summary>
        /// <param name="storeSetting">The <see cref="IStoreSetting"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IStoreSetting storeSetting, bool raiseEvents = true);

        /// <summary>
        /// Deletes a <see cref="IStoreSetting"/>
        /// </summary>
        /// <param name="storeSetting"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IStoreSetting storeSetting, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IStoreSetting"/> by it's unique 'Key' (Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IStoreSetting GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of all <see cref="IStoreSetting"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<IStoreSetting> GetAll();

        /// <summary>
        /// Returns the <see cref="ICountry"/> for the country code passed.
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="ICountry"/> for the country corresponding the the country code passed</returns>
        ICountry GetCountryByCode(string countryCode);

        /// <summary>
        /// Gets a collection of all <see cref="ICountry"/>
        /// </summary>
        IEnumerable<ICountry> GetAllCountries();

        /// <summary>
        /// Gets a collection of all <see cref="ICurrency"/>
        /// </summary>
        IEnumerable<ICurrency> GetAllCurrencies();
            
        /// <summary>
        /// Returns a <see cref="ICountry"/> collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCountryCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="ICountry"/></returns>
        IEnumerable<ICountry> GetAllCountries(string[] excludeCountryCodes);

    }
}