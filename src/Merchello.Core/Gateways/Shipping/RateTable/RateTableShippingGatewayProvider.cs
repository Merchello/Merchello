using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Shipping.RateTable
{
    /// <summary>
    /// Defines the RateTableLookupGateway
    /// </summary>
    public class RateTableShippingGatewayProvider : ShippingGatewayProviderBase
    {
        #region "Available Methods"
        
        // In this case, the GatewayResource can be used to create multiple shipmethods of the same resource type.
        private static readonly IEnumerable<IGatewayResource> AvailableResources  = new List<IGatewayResource>()
            {
                new GatewayResource("VBW", "Vary by Weight"),
                new GatewayResource("POT", "Percentage of Total")
            };

        #endregion


        public RateTableShippingGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Creates a GatewayShipMethod
        /// </summary>     
        /// <remarks>
        /// 
        /// This method is really specific to the RateTableShippingGateway due to the odd fact that additional shipmethods can be created 
        /// rather than defined up front.  
        /// 
        /// </remarks>   
        public IGatewayShipMethod CreateShipMethod(RateTableShipMethod.QuoteType quoteType, IShipCountry shipCountry, string name)
        {
            var resource = quoteType == RateTableShipMethod.QuoteType.VaryByWeight
                ? AvailableResources.First(x => x.ServiceCode == "VBW")
                : AvailableResources.First(x => x.ServiceCode == "POT");

            return CreateShipMethod(resource, shipCountry, name);
        }

        /// <summary>
        /// Creates an instance of a ship method
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// GatewayShipMethods (in general) should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>.  However, this is a
        /// a provider is sort of a unique case, sense we want to be able to add as many ship methods with rate tables as needed in order to facilitate 
        /// tiered rate tables for various ship methods without requiring a carrier based shipping provider.
        /// 
        /// </remarks>    
        public override IGatewayShipMethod CreateShipMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name)
        {

            Mandate.ParameterNotNull(gatewayResource, "gatewayResource");
            Mandate.ParameterNotNull(shipCountry, "shipCountry");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            var shipMethod = new ShipMethod(GatewayProvider.Key, shipCountry.Key)
                            {
                                Name = name,
                                ServiceCode = gatewayResource.ServiceCode + string.Format("-{0}", Guid.NewGuid().ToString()),
                                Taxable = false,
                                Surcharge = 0M,
                                Provinces = shipCountry.Provinces.ToShipProvinceCollection()
                            };

            GatewayProviderService.Save(shipMethod);

            return new RateTableShipMethod(gatewayResource, shipMethod, shipCountry);
        }

        /// <summary>
        /// Saves a <see cref="RateTableShipMethod"/> 
        /// </summary>
        /// <param name="gatewayShipMethod"></param>
        public override void SaveShipMethod(IGatewayShipMethod gatewayShipMethod)
        {
            GatewayProviderService.Save(gatewayShipMethod.ShipMethod);
            ShipRateTable.Save(GatewayProviderService, RuntimeCache, ((RateTableShipMethod) gatewayShipMethod).RateTable);
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return AvailableResources;
        }

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IGatewayShipMethod> GetActiveShipMethods(IShipCountry shipCountry)
        {
            var methods = GatewayProviderService.GetGatewayProviderShipMethods(GatewayProvider.Key, shipCountry.Key);
            return methods
                .Select(
                shipMethod => new RateTableShipMethod(AvailableResources.FirstOrDefault(x => shipMethod.ServiceCode.StartsWith(x.ServiceCode)), shipMethod, shipCountry, ShipRateTable.GetShipRateTable(GatewayProviderService, RuntimeCache, shipMethod.Key))
                ).OrderBy(x => x.ShipMethod.Name);
        }
    }
}