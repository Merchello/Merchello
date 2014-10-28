namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core.Services;

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
        /// <param name="storeSetting">The store setting to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IStoreSetting storeSetting, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IStoreSetting"/> by it's unique 'Key'
        /// </summary>
        /// <param name="key">The store setting key</param>
        /// <returns>The <see cref="IStoreSetting"/></returns>
        IStoreSetting GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of all <see cref="IStoreSetting"/>
        /// </summary>
        /// <returns>The collection of all <see cref="IStoreSetting"/></returns>
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
        /// <returns>
        /// The <see cref="IEnumerable{ICountry"/>.
        /// </returns>
        IEnumerable<ICountry> GetAllCountries();

        /// <summary>
        /// Gets a collection of all <see cref="ICurrency"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ICurrency}"/>.
        /// </returns>
        IEnumerable<ICurrency> GetAllCurrencies();

        /// <summary>
        /// Gets a <see cref="ICurrency"/> for the currency code passed
        /// </summary>
        /// <param name="currencyCode">The ISO Currency Code (ex. USD)</param>
        /// <returns>The <see cref="ICurrency"/></returns>
        ICurrency GetCurrencyByCode(string currencyCode);

        /// <summary>
        /// Returns a <see cref="ICountry"/> collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCountryCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="ICountry"/></returns>
        IEnumerable<ICountry> GetAllCountries(string[] excludeCountryCodes);

        /// <summary>
        /// Gets the next usable InvoiceNumber
        /// </summary>
        /// <param name="invoicesCount">
        /// The number of invoices.
        /// </param>
        /// <returns>
        /// The next invoice number
        /// </returns>
        int GetNextInvoiceNumber(int invoicesCount = 1);

        /// <summary>
        /// Gets the next usable OrderNumber
        /// </summary>
        /// <param name="ordersCount">The number of orders</param>
        /// <returns>The next order number</returns>
        int GetNextOrderNumber(int ordersCount = 1);

        /// <summary>
        /// Gets the next usable ShipmentNumber.
        /// </summary>
        /// <param name="shipmentsCount">
        /// The shipments count.
        /// </param>
        /// <returns>
        /// The next shipment number.
        /// </returns>
        int GetNextShipmentNumber(int shipmentsCount = 1);

        /// <summary>
        /// Gets the complete collection of registered type fields
        /// </summary>
        /// <returns>The collection of <see cref="ITypeField"/></returns>
        IEnumerable<ITypeField> GetTypeFields();
    }
}