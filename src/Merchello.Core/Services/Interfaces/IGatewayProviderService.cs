using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the GatewayProviderService
    /// </summary>
    public interface IGatewayProviderService : IService
    {

        #region GatewayProvider

        /// <summary>
        /// Saves a single instance of a <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IGatewayProvider gatewayProvider, bool raiseEvents = true);

        /// <summary>
        /// Deletes a <see cref="IGatewayProvider"/>
        /// </summary>
        /// <param name="gatewayProvider"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IGatewayProvider gatewayProvider, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IGatewayProvider"/> by it's unique 'Key' (Guid)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IGatewayProvider GetGatewayProviderByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/> by its type (Shipping, Taxation, Payment)
        /// </summary>
        /// <param name="gatewayProviderType"></param>
        /// <returns></returns>
        IEnumerable<IGatewayProvider> GetGatewayProvidersByType(GatewayProviderType gatewayProviderType);

        /// <summary>
        /// Gets a collection of <see cref="IGatewayProvider"/> by ship country
        /// </summary>
        /// <param name="shipCountry"></param>
        /// <returns></returns>
        IEnumerable<IGatewayProvider> GetGatewayProvidersByShipCountry(IShipCountry shipCountry); 

        /// <summary>
        /// Gets a collection containing all <see cref="IGatewayProvider"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGatewayProvider> GetAllGatewayProviders(); 

        #endregion

        #region ShipMethod

        /// <summary>
        /// Creates a <see cref="IShipMethod"/>.  This is useful due to the data constraint
        /// preventing two ShipMethods being created with the same ShipCountry and ServiceCode for any provider.
        /// </summary>
        /// <param name="providerKey">The unique gateway provider key (Guid)</param>
        /// <param name="shipCountryKey">The unique ship country key (Guid)</param>
        /// <param name="name">The required name of the <see cref="IShipMethod"/></param>
        /// <param name="serviceCode">The ShipMethods service code</param>
        Attempt<IShipMethod> CreateShipMethodWithKey(Guid providerKey, Guid shipCountryKey, string name, string serviceCode);

        /// <summary>
        /// Saves a single <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod"></param>
        void Save(IShipMethod shipMethod);

        /// <summary>
        /// Saves a collection of <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="IShipMethod"/></param>
        void Save(IEnumerable<IShipMethod> shipMethodList);

        /// <summary>
        /// Deletes a <see cref="IShipMethod"/>
        /// </summary>
        /// <param name="shipMethod"></param>
        void Delete(IShipMethod shipMethod);

        /// <summary>
        /// Gets a list of <see cref="IShipMethod"/> objects given a <see cref="IGatewayProvider"/> key and a <see cref="IShipCountry"/> key
        /// </summary>
        /// <returns>A collection of <see cref="IShipMethod"/></returns>
        IEnumerable<IShipMethod> GetGatewayProviderShipMethods(Guid providerKey, Guid shipCountryKey);

        #endregion

        #region ShipRateTier

        /// <summary>
        /// Saves a single <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier"></param>
        void Save(IShipRateTier shipRateTier);

        /// <summary>
        /// Saves a collection of <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTierList"></param>
        void Save(IEnumerable<IShipRateTier> shipRateTierList);

        /// <summary>
        /// Deletes a <see cref="IShipRateTier"/>
        /// </summary>
        /// <param name="shipRateTier"></param>
        void Delete(IShipRateTier shipRateTier);

        /// <summary>
        /// Gets a list of <see cref="IShipRateTier"/> objects given a <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">Guid</param>
        /// <returns>A collection of <see cref="IShipRateTier"/></returns>
        IEnumerable<IShipRateTier> GetShipRateTiersByShipMethodKey(Guid shipMethodKey);

        #endregion

        #region ShipCountry

        IShipCountry GetShipCountry(Guid catalogKey, string countryCode);

        #endregion

        #region CountryTaxRate

        /// <summary>
        /// Attempts to create a <see cref="ICountryTaxRate"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="providerKey">The unique 'key' (Guid) of the TaxationGatewayProvider</param>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        /// <returns><see cref="Attempt"/> indicating whether or not the creation of the <see cref="ICountryTaxRate"/> with respective success or fail</returns>
        Attempt<ICountryTaxRate> CreateCountryTaxRateWithKey(Guid providerKey, string countryCode, decimal percentageTaxRate);

        #endregion
    }
}