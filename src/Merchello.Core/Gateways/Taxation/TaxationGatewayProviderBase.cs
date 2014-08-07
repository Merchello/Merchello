namespace Merchello.Core.Gateways.Taxation
{
    using System.Collections.Generic;
    using Models;
    using Services;
    using Umbraco.Core.Cache;

    /// <summary>
    /// Defines a base taxation gateway provider
    /// </summary>
    public abstract class TaxationGatewayProviderBase : GatewayProviderBase, ITaxationGatewayProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxationGatewayProviderBase"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="gatewayProviderSettings">
        /// The gateway provider settings.
        /// </param>
        /// <param name="runtimeCacheProvider">
        /// The runtime cache provider.
        /// </param>
        protected TaxationGatewayProviderBase(
            IGatewayProviderService gatewayProviderService,
            IGatewayProviderSettings gatewayProviderSettings, 
            IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {            
        }

        /// <summary>
        /// Gets a collection of <see cref="ITaxMethod"/> assoicated with this provider
        /// </summary>
        public IEnumerable<ITaxMethod> TaxMethods
        {
            get { return GatewayProviderService.GetTaxMethodsByProviderKey(GatewayProviderSettings.Key); }
        }

        /// <summary>
        /// Attempts to create a <see cref="ITaxationGatewayMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">
        /// The two character ISO country code
        /// </param>
        /// <returns>
        /// The <see cref="ITaxationGatewayMethod"/>.
        /// </returns>
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

            // reset the TaxMethods so that they need to be pulled on the next request
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
        /// Gets a <see cref="ITaxationGatewayMethod"/> by it's unique 'key' (GUID)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns>The <see cref="ITaxationGatewayMethod"/></returns>
        public abstract ITaxationGatewayMethod GetGatewayTaxMethodByCountryCode(string countryCode);

        /// <summary>
        /// Gets a collection of all <see cref="ITaxationGatewayMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ITaxationGatewayMethod"/> </returns>
        public abstract IEnumerable<ITaxationGatewayMethod> GetAllGatewayTaxMethods();

        /// <summary>
        /// Deletes all <see cref="ITaxMethod"/>s associated with the provider
        /// </summary>
        internal void DeleteAllTaxMethods()
        {
            foreach (var taxMethod in TaxMethods) GatewayProviderService.Delete(taxMethod);
        }        
    }
}