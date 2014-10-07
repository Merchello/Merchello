using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Tests.Base.TestHelpers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.DisplayClasses
{
    [TestFixture]
    public class GatewayProviderDisplayTests : MerchelloAllInTestBase
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
        /// Test confirms 
        /// </summary>
        [Test]
        public void Can_Map_Test_PaymentProvider_To_GatewayProviderDisplay_With_Editor()
        {
            //// Arrange
            var key = new Guid("5A5B38F4-0E74-4057-BCFF-F903CF449AD8");
            const string editorView = "/App_Plugins/Merchello/Testing/test.paymentprovider.view.html";
            var provider = MerchelloContext.Current.Gateways.Payment.GetAllProviders().FirstOrDefault(x => x.Key == key);
            Assert.NotNull(provider);

            //// Act
            var mapped = provider.GatewayProviderSettings.ToGatewayProviderDisplay();

            //// Assert
            Assert.NotNull(mapped, "mapped was null");
            Assert.NotNull(mapped.DialogEditorView, "mapped did not have a DialogEditorView");
            Assert.AreEqual(editorView, mapped.DialogEditorView.EditorView);
        }

        /// <summary>
        /// Test confirms a PaymentProvider can be mapped to a GatewayProviderDisplay with ExtendedData
        /// </summary>
        [Test]
        public void Can_Map_Test_PaymentProvider_To_GatewayProviderDisplay_With_ExtendedData()
        {
            //// Arrange
            var key = new Guid("5A5B38F4-0E74-4057-BCFF-F903CF449AD8");
            var provider = MerchelloContext.Current.Gateways.Payment.GetProviderByKey(key, false);
            Assert.NotNull(provider);

            // add stuff to extendedData
            provider.ExtendedData.SetValue("test1", "test1");
            provider.ExtendedData.SetValue("test2", "test2");
            provider.ExtendedData.SetValue("test3", "test3");

            //// Act
            var mapped = provider.GatewayProviderSettings.ToGatewayProviderDisplay();

            //// Assert
            Assert.NotNull(mapped);
            Assert.IsTrue(mapped.ExtendedData.Any());
        }

        /// <summary>
        /// Test confirms that a CashPaymentGatewayMethod can be mapped directly to a PaymentMethodDisplay
        /// </summary>
        [Test]
        public void Can_Map_CashPaymentGatewayMethod_To_PaymentMethodDisplay()
        {
            //// Arrange
            const string editor = "/App_Plugins/Merchello/Modules/Settings/Payment/Dialogs/paymentmethod.html";
            var provider =
                MerchelloContext.Current.Gateways.Payment.CreateInstance(
                    Constants.ProviderKeys.Payment.CashPaymentProviderKey);
            Assert.NotNull(provider, "Cash payment provider was null");

            var method = provider.GetPaymentGatewayMethodByKey(provider.PaymentMethods.First().Key);
            Assert.NotNull(method, "Payment method was null");

            //// Act
            var display = method.ToPaymentMethodDisplay();

            //// Assert
            Assert.NotNull(display);
            Assert.AreEqual(method.PaymentMethod.Name, display.Name);
            Assert.AreEqual(method.PaymentMethod.Key, display.Key);
            Assert.AreEqual(method.PaymentMethod.Description, display.Description);
            Assert.AreEqual(editor, display.DialogEditorView.EditorView);

        }
    }
}