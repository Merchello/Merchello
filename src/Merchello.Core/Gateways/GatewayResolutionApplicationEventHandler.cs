using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways
{
    public class GatewayResolutionApplicationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);
            
            LogHelper.Info<GatewayResolutionApplicationEventHandler>("Starting Merchello GatewayProvider Resolution");
            
            PaymentGatewayProviderResolver.Current = new PaymentGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<PaymentGatewayProviderBase>());
            TaxationGatewayProviderResolver.Current = new TaxationGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<TaxationGatewayProviderBase>());
            ShippingGatewayProviderResolver.Current = new ShippingGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<ShippingGatewayProviderBase>());

            LogHelper.Info<GatewayResolutionApplicationEventHandler>("Completed Merchello GatewayProvider Resolution");
        }
    }
}