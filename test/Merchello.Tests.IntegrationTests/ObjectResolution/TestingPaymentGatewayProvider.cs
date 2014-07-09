using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [GatewayProviderActivation("5A5B38F4-0E74-4057-BCFF-F903CF449AD8", "Payment Provider For Testing", "Payment Provider for Object Resolution Testing")]
    [GatewayProviderEditor("Payment Provider Configuration", "~/App_Plugins/Merchello/Testing/test.paymentprovider.view.html")]
    public class TestingPaymentGatewayProvider : PaymentGatewayProviderBase
    {

        private static IEnumerable<IGatewayResource> AvailableResources
        {
            get
            {
                return new List<IGatewayResource>()
                {
                    new GatewayResource("Test1", "Test 1"),
                    new GatewayResource("Test2", "Test 2"),
                    new GatewayResource("Test3", "Test 3"),
                    new GatewayResource("Test4", "Test 4")
                };
            }
        }

        public TestingPaymentGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return AvailableResources.Where(x => PaymentMethods.All(y => y.PaymentCode != x.ServiceCode));
        }

        public override IPaymentGatewayMethod CreatePaymentMethod(IGatewayResource gatewayResource, string name, string description)
        {
            // assert gateway resource is still available
            var available = ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == gatewayResource.ServiceCode);
            if (available == null) throw new InvalidOperationException("GatewayResource has already been assigned");

            var attempt = GatewayProviderService.CreatePaymentMethodWithKey(GatewayProviderSettings.Key, name, description, available.ServiceCode);


            if (attempt.Success)
            {
                PaymentMethods = null;

                return new TestingPaymentGatewayMethod(GatewayProviderService, attempt.Result);
            }

            LogHelper.Error<TestingPaymentGatewayProvider>(string.Format("Failed to create a payment method name: {0}, description {1}, paymentCode {2}", name, description, available.ServiceCode), attempt.Exception);

            throw attempt.Exception;
        }

        public override IPaymentGatewayMethod GetPaymentGatewayMethodByKey(Guid paymentMethodKey)
        {
            var paymentMethod = PaymentMethods.FirstOrDefault(x => x.Key == paymentMethodKey);

            if (paymentMethod == null) throw new NullReferenceException("PaymentMethod not found");

            return new TestingPaymentGatewayMethod(GatewayProviderService, paymentMethod);
        }

        public override IPaymentGatewayMethod GetPaymentGatewayMethodByPaymentCode(string paymentCode)
        {
            var paymentMethod = PaymentMethods.FirstOrDefault(x => x.PaymentCode == paymentCode);

            if (paymentMethod == null) throw new NullReferenceException("PaymentMethod not found");

            return new TestingPaymentGatewayMethod(GatewayProviderService, paymentMethod);
        }
    }
}