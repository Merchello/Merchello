using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the 
    /// </summary>
    public interface ICountryTaxRateService : IService
    {
        ///// <summary>
        ///// Attempts to create a <see cref="ICountryTaxRate"/> for a given provider and country.  If the provider already 
        ///// defines a tax rate for the country, the creation fails.
        ///// </summary>
        ///// <param name="providerKey">The unique 'key' (Guid) of the TaxationGatewayProvider</param>
        ///// <param name="countryCode">The two character ISO country code</param>
        ///// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        ///// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        ///// <returns><see cref="Attempt"/> indicating whether or not the creation of the <see cref="ICountryTaxRate"/> with respective success or fail</returns>
        //Attempt<ICountryTaxRate> CreateCountryTaxRateWithKey(Guid providerKey, string countryCode, decimal percentageTaxRate, bool raiseEvents = true);
        
        //Attempt<ICountryTaxRate> CreateCountryTaxRateWithKey(Guid providerKey, ICountry country, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRate">The <see cref="ICountryTaxRate"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ICountryTaxRate countryTaxRate, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRateList">A collection of <see cref="ICountryTaxRate"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<ICountryTaxRate> countryTaxRateList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRate">The <see cref="ICountryTaxRate"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ICountryTaxRate countryTaxRate, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="key">The unique 'key' (Guid) of the <see cref="ICountryTaxRate"/></param>
        /// <returns><see cref="ICountryTaxRate"/></returns>
        ICountryTaxRate GetByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="ICountryTaxRate"/> based on a provider and country code
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the <see cref="IGatewayProvider"/></param>
        /// <param name="countryCode">The country code of the <see cref="ICountryTaxRate"/></param>
        /// <returns><see cref="ICountryTaxRate"/></returns>
        ICountryTaxRate GetCountryTaxRateByCountryCode(Guid providerKey, string countryCode);

        /// <summary>
        /// Gets a collection of <see cref="ICountryTaxRate"/> for a given TaxationGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the TaxationGatewayProvider</param>
        /// <returns>A collection of <see cref="ICountryTaxRate"/></returns>
        IEnumerable<ICountryTaxRate> GetCountryTaxRatesByProviderKey(Guid providerKey);
        

    }
}