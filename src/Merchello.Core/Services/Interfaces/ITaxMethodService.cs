using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the TaxMethodService
    /// </summary>
    internal interface ITaxMethodService : IService
    {
       
        /// <summary>
        /// Saves a single <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(ITaxMethod taxMethod, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="countryTaxRateList">A collection of <see cref="ITaxMethod"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<ITaxMethod> countryTaxRateList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(ITaxMethod taxMethod, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethods">The collection of <see cref="ITaxMethod"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<ITaxMethod> taxMethods, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="key">The unique 'key' (Guid) of the <see cref="ITaxMethod"/></param>
        /// <returns><see cref="ITaxMethod"/></returns>
        ITaxMethod GetByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> based on a provider and country code
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the <see cref="IGatewayProviderSettings"/></param>
        /// <param name="countryCode">The country code of the <see cref="ITaxMethod"/></param>
        /// <returns><see cref="ITaxMethod"/></returns>
        ITaxMethod GetTaxMethodByCountryCode(Guid providerKey, string countryCode);

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> based on a provider and country code
        /// </summary>
        /// <param name="countryCode">The country code of the <see cref="ITaxMethod"/></param>
        /// <returns><see cref="ITaxMethod"/></returns>
        IEnumerable<ITaxMethod> GetTaxMethodsByCountryCode(string countryCode);

        /// <summary>
        /// Gets a collection of <see cref="ITaxMethod"/> for a given TaxationGatewayProvider
        /// </summary>
        /// <param name="providerKey">The unique 'key' of the TaxationGatewayProvider</param>
        /// <returns>A collection of <see cref="ITaxMethod"/></returns>
        IEnumerable<ITaxMethod> GetTaxMethodsByProviderKey(Guid providerKey);
        

    }
}