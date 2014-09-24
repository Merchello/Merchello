using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Tests.Base.TestHelpers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core;

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

            // deactivate all test providers
            var testProviders = GatewayProviderResolver.Current.GetActivatedProviders().Where(x => testKeys.Contains(x.GatewayProviderSettings.Key) && x.Activated);
            foreach (var provider in testProviders)
            {
                ((GatewayContext)MerchelloContext.Current.Gateways).DeactivateProvider(provider);
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
            var providers = GatewayProviderResolver.Current.GetAllProviders<PaymentGatewayProviderBase>();

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
            var providers = GatewayProviderResolver.Current.GetAllProviders<ShippingGatewayProviderBase>();

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
            var providers = GatewayProviderResolver.Current.GetAllProviders<TaxationGatewayProviderBase>();

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
            

            //// Act
            var providers = GatewayProviderResolver.Current.GetAllProviders<TaxationGatewayProviderBase>().ToArray();

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
            
            //// Act
            var providers = GatewayProviderResolver.Current.GetAllProviders<ShippingGatewayProviderBase>().ToArray();

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
            
            //// Act
            var providers = GatewayProviderResolver.Current.GetAllProviders<PaymentGatewayProviderBase>().ToArray();

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
            
            //// Act
            var provider = GatewayProviderResolver.Current.GetAllProviders<PaymentGatewayProviderBase>().FirstOrDefault(x => !x.Activated);
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
            
            var providers = GatewayProviderResolver.Current.GetAllProviders<PaymentGatewayProviderBase>();
            var provider = providers.FirstOrDefault(x => !x.Activated);
            Assert.NotNull(provider);
            MerchelloContext.Current.Gateways.Payment.ActivateProvider(provider);
            Assert.IsTrue(provider.Activated);

            //// Act
            var key = provider.Key;
            MerchelloContext.Current.Gateways.Payment.DeactivateProvider(provider);

            var retrieved = GatewayProviderResolver.Current.GetProviderByKey<PaymentGatewayProviderBase>(key,false);


            //// Assert
            Assert.IsFalse(retrieved.Activated);
        }

        [Test]
        public void Can_Deactivate_A_ShippingGatewayProvider()
        {
            //// Arrange

            var provider = GatewayProviderResolver.Current.GetAllProviders<ShippingGatewayProviderBase>().FirstOrDefault(x => !x.Activated);
            Assert.NotNull(provider);
            MerchelloContext.Current.Gateways.Shipping.ActivateProvider(provider);
            Assert.IsTrue(provider.Activated);

            //// Act
            var key = provider.Key;
            MerchelloContext.Current.Gateways.Shipping.DeactivateProvider(provider);

            var retrieved = GatewayProviderResolver.Current.GetProviderByKey<ShippingGatewayProviderBase>(key, false);


            //// Assert
            Assert.IsFalse(retrieved.Activated);
        }

        [Test]
        public void Can_Resolve_A_NotificationGatewayProvider()
        {
            //// Arrage
            var types =
                PluginManager.Current.ResolveTypesWithAttribute<GatewayProviderBase, GatewayProviderActivationAttribute>
                    ();
            //// Act 
            var provider = new List<GatewayProviderBase>();

        }
    }
}
