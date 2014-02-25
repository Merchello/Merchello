using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines a base taxation gateway provider
    /// </summary>
    public abstract class TaxationGatewayProviderBase : GatewayProviderBase, ITaxationGatewayProvider
    {
        
        protected TaxationGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Attempts to create a <see cref="ITaxationGatewayMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        public virtual ITaxationGatewayMethod CreateTaxMethod(string countryCode)
        {
            return CreateTaxMethod(countryCode, 0);
        }

        /// <summary>
        /// Creates a <see cref="ITaxationGatewayMethod"/>
        /// </summary>
        /// <param name="countryCode">The two letter ISO Country Code</param>
        /// <param name="taxPercentageRate">The decimal percentage tax rate</param>
        /// <returns>The <see cref="ITaxationGatewayMethod"/></returns>
        public abstract ITaxationGatewayMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate);

        /// <summary>
        /// Saves a <see cref="ITaxationGatewayMethod"/>
        /// </summary>
        /// <param name="taxationGatewayMethod">The <see cref="ITaxationGatewayMethod"/> to be saved</param>
        public void SaveTaxMethod(ITaxationGatewayMethod taxationGatewayMethod)
        {
            GatewayProviderService.Save(taxationGatewayMethod.TaxMethod);
        }

        /// <summary>
        /// Deletes a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxationGatewayMethod">The <see cref="ITaxationGatewayMethod"/> to be deleted</param>
        public void DeleteTaxMethod(ITaxationGatewayMethod taxationGatewayMethod)
        {
            GatewayProviderService.Delete(taxationGatewayMethod.TaxMethod);
        }

        /// <summary>
        /// Deletes all <see cref="ITaxMethod"/>s associated with the provider
        /// </summary>
        internal void DeleteAllTaxMethods()
        {
            foreach(var taxMethod in TaxMethods) GatewayProviderService.Delete(taxMethod);
        }
        
        /// <summary>
        /// Gets a <see cref="ITaxationGatewayMethod"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ITaxationGatewayMethod"/></returns>
        public abstract ITaxationGatewayMethod GetGatewayTaxMethodByCountryCode(string countryCode);

        /// <summary>
        /// Gets a collection of all <see cref="ITaxationGatewayMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ITaxationGatewayMethod"/> </returns>
        public abstract IEnumerable<ITaxationGatewayMethod> GetAllGatewayTaxMethods();


        private IEnumerable<ITaxMethod> _taxMethods;
        /// <summary>
        /// Gets a collection of <see cref="ITaxMethod"/> assoicated with this provider
        /// </summary>
        public IEnumerable<ITaxMethod> TaxMethods
        {
            get {
                return _taxMethods ??
                       (_taxMethods = GatewayProviderService.GetTaxMethodsByProviderKey(GatewayProvider.Key));
            }
        }
    }
}