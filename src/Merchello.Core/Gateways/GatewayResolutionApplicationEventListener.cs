﻿using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Umbraco.Core;

namespace Merchello.Core.Gateways
{
    public class GatewayResolutionApplicationEventListener : ApplicationEventHandler
    {
        protected override void ApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationInitialized(umbracoApplication, applicationContext);

            PaymentGatewayProviderResolver.Current = new PaymentGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<PaymentGatewayProviderBase>());
            TaxationGatewayProviderResolver.Current = new TaxationGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<TaxationGatewayProviderBase>());
            ShippingGatewayProviderResolver.Current = new ShippingGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<ShippingGatewayProviderBase>());
        }
    }
}