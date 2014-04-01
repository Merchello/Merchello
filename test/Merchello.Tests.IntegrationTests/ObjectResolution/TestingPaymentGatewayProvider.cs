using System;
using System.Collections.Generic;
using ClientDependency.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [GatewayProviderActivation("5A5B38F4-0E74-4057-BCFF-F903CF449AD8", "Payment Provider For Testing", "Payment Provider for Object Resolution Testing")]
    [GatewayProviderEditor("Payment Provider Configuration", "~/App_Plugins/Merchello/Testing/test.paymentprovider.view.html")]
    public class TestingPaymentGatewayProvider : PaymentGatewayProviderBase
    {
        public TestingPaymentGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
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