namespace Merchello.Plugin.Payments.Braintree.Provider
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    /// <summary>
    /// The BrainTree Payment Gateway Provider.
    /// </summary>
    [GatewayProviderActivation("D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969", "BrainTree Payment Provider", "BrainTree Payment Provider")]
    [GatewayProviderEditor("BrainTree Configuration", "~/App_Plugins/Merchello.BrainTree/providerSettingsDialog.html")]
    public class BraintreePaymentGatewayProvider : PaymentGatewayProviderBase, IBraintreePaymentGatewayProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentGatewayProvider"/> class.
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
        public BraintreePaymentGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, Umbraco.Core.Cache.IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            throw new NotImplementedException();
        }

        public override IPaymentGatewayMethod CreatePaymentMethod(IGatewayResource gatewayResource, string name, string description)
        {
            throw new NotImplementedException();
        }

        public override IPaymentGatewayMethod GetPaymentGatewayMethodByKey(Guid paymentMethodKey)
        {
            throw new NotImplementedException();
        }

        public override IPaymentGatewayMethod GetPaymentGatewayMethodByPaymentCode(string paymentCode)
        {
            throw new NotImplementedException();
        }
    }
}