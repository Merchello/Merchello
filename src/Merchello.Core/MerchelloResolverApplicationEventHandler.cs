using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Triggers;
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

            PaymentGatewayProviderResolver.Current = new PaymentGatewayProviderResolver(() => PluginManager.Current.ResolveTypesWithAttribute<PaymentGatewayProviderBase, GatewayProviderActivationAttribute>());
            NotificationGatewayProviderResolver.Current = new NotificationGatewayProviderResolver(() => PluginManager.Current.ResolveTypesWithAttribute<NotificationGatewayProviderBase, GatewayProviderActivationAttribute>());
            TaxationGatewayProviderResolver.Current = new TaxationGatewayProviderResolver(() => PluginManager.Current.ResolveTypesWithAttribute<TaxationGatewayProviderBase, GatewayProviderActivationAttribute>());
            ShippingGatewayProviderResolver.Current = new ShippingGatewayProviderResolver(() => PluginManager.Current.ResolveTypesWithAttribute<ShippingGatewayProviderBase, GatewayProviderActivationAttribute>());

            EventTriggerRegistry.Current = 
                new EventTriggerRegistry(() => PluginManager.Current.ResolveTypesWithAttribute<IEventTrigger, EventTriggerAttribute>());



            NotificationFormatterResolver.Current = new NotificationFormatterResolver();

            LogHelper.Info<MerchelloResolverApplicationEventHandler>("Completed Merchello GatewayProvider Resolution");
        }
    }
}