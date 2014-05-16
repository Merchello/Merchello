using System.Configuration;
using System.Linq;
using System.Threading;
using Merchello.Plugin.Payments.AuthorizeNet;
using Merchello.Tests.AuthorizeNet.Integration.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.AuthorizeNet.Integration.GatewayProvider
{
    [TestFixture]
    public class ProviderTests : ProviderTestsBase
    {
        [SetUp]
        public void Init()
        {
            foreach (var method in Provider.PaymentMethods)
            {
                GatewayProviderService.Delete(method);    
            }
            
        }

        /// <summary>
        /// Test confirms that extended data processor args (which will be saved via the Angular dialog flyout) can be deserialized.
        /// </summary>
        [Test]
        public void Can_Retrieve_ProcessorSettings_From_ExtendedData()
        {
            //// Arrange
            // mainly handled in base class
            var loginId = ConfigurationManager.AppSettings["xlogin"];
            var xtrankey = ConfigurationManager.AppSettings["xtrankey"];

            //// Act
            var settings = Provider.GatewayProviderSettings.ExtendedData.GetProcessorSettings();

            //// Assert
            Assert.NotNull(settings);
            Assert.AreEqual("CC", settings.Method);
            Assert.AreEqual(loginId, settings.LoginId);
            Assert.AreEqual(xtrankey, settings.TransactionKey);
            Assert.AreEqual("|", settings.DelimitedChar);
        }

        /// <summary>
        /// Test confirms that a payment method can be created
        /// </summary>
        [Test]
        public void Can_Create_One_And_OnlyOne_PaymentMethod()
        {
            //// Arrange
            var resource = Provider.ListResourcesOffered().ToArray();
            Assert.AreEqual(1, resource.Count());

            //// Act
            var method = Provider.CreatePaymentMethod(resource.First(), "Credit Card", "Credit Card Payments");

            //// Assert
            Assert.NotNull(method);
        }
        


    }
}