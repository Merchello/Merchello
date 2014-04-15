﻿using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [TestFixture]
    public class GatewayProviderResolution : MerchelloAllInTestBase
    {
        [SetUp]
        public void Init()
        {
            var testKeys = new[]
            {
                new Guid("5A5B38F4-0E74-4057-BCFF-F903CF449AD8"),
                new Guid("61D8BC55-5D72-4244-A63B-E942C1D4AB47"),
                new Guid("518B5FDF-C414-4309-99D5-E61028311A2F")
            };

            // delete the test providers if they exist
            var providers = DbPreTestDataWorker.GatewayProviderService.GetAllGatewayProviders().Where(x => testKeys.Contains(x.Key));

            foreach (var provider in providers)
            {
                DbPreTestDataWorker.GatewayProviderService.Delete(provider);
            }


        }

        /// <summary>
        /// Test verifies that PaymentGatewayProviders can be resolved
        /// </summary>
        [Test]
        public void Can_Retreive_PaymentGatewayProviders_From_Resolver()
        {
            //// Arrange
            // Handled in base class instantiation

            //// Act
            var providers = PaymentGatewayProviderResolver.Current.ProviderTypes;

            //// Assert
            Assert.IsTrue(providers.Any());
        }

        /// <summary>
        /// Test verifies that ShippingGatewayProviders can be resolved
        /// </summary>
        [Test]
        public void Can_Retreive_ShippingGatewayProviders_From_Resolver()
        {
            //// Arrange
            // Handled in base class instantiation

            //// Act
            var providers = ShippingGatewayProviderResolver.Current.ProviderTypes;

            //// Assert
            Assert.IsTrue(providers.Any());
        }

        /// <summary>
        /// Test verifies that TaxationGatewayProviders can be resolved
        /// </summary>
        [Test]
        public void Can_Retreive_TaxationGatewayProviders_From_Resolver()
        {
            //// Arrange
            // Handled in base class instantiation

            //// Act
            var providers = TaxationGatewayProviderResolver.Current.ProviderTypes;

            //// Assert
            Assert.IsTrue(providers.Any());
        }

        /// <summary>
        /// Test verifies that both "Actived" and "Inactive"  Taxation GatewayProviders can be resolved and returned
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_AllTaxationProviders()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache);

            //// Act
            var providers = resolver.GetAllProviders<TaxationGatewayProviderBase>().ToArray();

            //// Assert
            Assert.IsTrue(providers.Any());
            Assert.IsTrue(providers.Any(x => x.Activated));
            Assert.IsTrue(providers.Any(x => !x.Activated));
        }

        /// <summary>
        /// Test verifies that both "Actived" and "Inactive" Shipping GatewayProviders can be resolved and returned
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_AllShippingProviders()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache);

            //// Act
            var providers = resolver.GetAllProviders<ShippingGatewayProviderBase>().ToArray();

            //// Assert
            Assert.IsTrue(providers.Any());
            Assert.IsTrue(providers.Any(x => x.Activated));
            Assert.IsTrue(providers.Any(x => !x.Activated));
        }

        /// <summary>
        /// Test verifies that both "Actived" and "Inactive" Payment GatewayProviders can be resolved and returned
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_AllPaymentProviders()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache);

            //// Act
            var providers = resolver.GetAllProviders<PaymentGatewayProviderBase>().ToArray();

            //// Assert
            Assert.IsTrue(providers.Any());
            Assert.IsTrue(providers.Any(x => x.Activated));
            Assert.IsTrue(providers.Any(x => !x.Activated));
        }

        /// <summary>
        /// Test verifies that a PaymentGatewayProvider can be activated
        /// </summary>
        [Test]
        public void Can_Activate_A_PaymentGatewayProvider()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache);

            //// Act
            var provider = resolver.GetAllProviders<PaymentGatewayProviderBase>().FirstOrDefault(x => !x.Activated);
            Assert.NotNull(provider);

            MerchelloContext.Current.Gateways.Payment.ActivateProvider(provider);

            //// Assert
            Assert.IsTrue(provider.Activated);
        }

        /// <summary>
        /// Test verifies that a PaymentGatewayProvider can be deactivated
        /// </summary>
        [Test]
        public void Can_Deactivate_A_PaymentGatewayProvider()
        {
            //// Arrange
            var resolver = new GatewayProviderResolver(MerchelloContext.Current.Services.GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache);
            var provider = resolver.GetAllProviders<PaymentGatewayProviderBase>().FirstOrDefault(x => !x.Activated);
            Assert.NotNull(provider);
            MerchelloContext.Current.Gateways.Payment.ActivateProvider(provider);
            Assert.IsTrue(provider.Activated);

            //// Act
            var key = provider.Key;
            MerchelloContext.Current.Gateways.Payment.DeactivateProvider(provider);

            var retrieved = resolver.GetAllProviders<PaymentGatewayProviderBase>().FirstOrDefault(x => x.Key == key);


            //// Assert
            Assert.IsFalse(retrieved.Activated);
        }

    }
}
