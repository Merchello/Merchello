using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Plugin.Shipping.FOA.Provider
{
    [GatewayProviderActivation("9ACEE07C-94A7-4D28-8193-6BD7D221A902", "Free Over Amount Shipping Provider", "Free Over Amount Shipping Provider")]
    [GatewayProviderEditor("Free Over Amount configuration", "~/App_Plugins/Merchello.FOA/editor.html")]
    public class FoaShippingGatewayProvider : ShippingGatewayProviderBase, IFoaShippingGatewayProvider
    {
        private static readonly GatewayResource GatewayResource = new GatewayResource("01", "Free Shipping Over Amount");

        public FoaShippingGatewayProvider(IGatewayProviderService gatewayProviderService, 
            IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) : 
            base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return new List<IGatewayResource> { GatewayResource };
        }

        public override IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource, IShipCountry shipCountry,
            string name)
        {
            var attempt = GatewayProviderService.CreateShipMethodWithKey(GatewayProviderSettings.Key, shipCountry, name,
                gatewayResource.ServiceCode);

            if (!attempt.Success) throw attempt.Exception;

            return new FoaShippingGatewayMethod(gatewayResource, attempt.Result, shipCountry, new ExtendedDataCollection());
        }

        public override void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod)
        {                                                                
            GatewayProviderService.Save(shippingGatewayMethod.ShipMethod);
        }

        public override IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry)
        {
            var methods = GatewayProviderService.GetShipMethodsByShipCountryKey(GatewayProviderSettings.Key, shipCountry.Key);
            return methods
                .Select(
                    shipMethod =>
                        new FoaShippingGatewayMethod(
                            GatewayResource,
                            shipMethod, shipCountry, new ExtendedDataCollection())
                ).OrderBy(x => x.ShipMethod.Name);
        }
    }
}
