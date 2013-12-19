using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Shipping.RateTable
{
    /// <summary>
    /// Defines the RateTableLookupGateway
    /// </summary>
    public class RateTableShippingGatewayProvider : ShippingGatewayProvider
    {
        #region "Available Methods"
        
        private static readonly IEnumerable<IGatewayResource> AvailableMethods  = new List<IGatewayResource>()
            {
                new GatewayResource("VBW", "Vary by Weight"),
                new GatewayResource("POT", "Percentage of Total")
            };

        #endregion


        public RateTableShippingGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        public override IGatewayShipMethod CreateShipMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name)
        {
            Mandate.ParameterNotNull(gatewayResource, "gatewayResource");
            Mandate.ParameterNotNull(shipCountry, "shipCountry");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            

            var shipMethod = new ShipMethod(GatewayProvider.ProviderTfKey, shipCountry.Key)
                            {
                                Name = name,
                                ServiceCode = gatewayResource.ServiceCode,
                                Taxable = false,
                                Surcharge = 0M,
                                Provinces = shipCountry.Provinces.ToShipProvinceCollection()
                            };

            return new RateTableShipMethod(gatewayResource, shipMethod);
        }

        public override void SaveShipMethod(IGatewayShipMethod shipMethod)
        {
            GatewayProviderService.Save(shipMethod.ShipMethod);
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IGatewayResource> ListAvailableMethods()
        {
            return AvailableMethods;
        }

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IGatewayShipMethod> ActiveShipMethods(IShipCountry shipCountry)
        {
            var methods = GatewayProviderService.GetGatewayProviderShipMethods(GatewayProvider.Key, shipCountry.Key);
            return methods
                .Select(
                shipMethod => new RateTableShipMethod(AvailableMethods.FirstOrDefault(x => x.ServiceCode == shipMethod.ServiceCode), shipMethod)
                ).OrderBy(x => x.ShipMethod.Name);
        }
    }
}