using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core
{
    public class MerchelloResolverApplicationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationInitialized(umbracoApplication, applicationContext);

            LogHelper.Info<MerchelloResolverApplicationEventHandler>("Starting Merchello GatewayProvider Resolution");

            PaymentGatewayProviderResolver.Current = new PaymentGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<PaymentGatewayProviderBase>());
            NotificationGatewayProviderResolver.Current = new NotificationGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<NotificationGatewayProviderBase>());
            TaxationGatewayProviderResolver.Current = new TaxationGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<TaxationGatewayProviderBase>());
            ShippingGatewayProviderResolver.Current = new ShippingGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<ShippingGatewayProviderBase>());

            NotificationFormatterResolver.Current = new NotificationFormatterResolver();

            LogHelper.Info<MerchelloResolverApplicationEventHandler>("Completed Merchello GatewayProvider Resolution");
        }
    }
}