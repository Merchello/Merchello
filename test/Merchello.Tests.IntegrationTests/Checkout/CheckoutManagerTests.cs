namespace Merchello.Tests.IntegrationTests.Checkout
{
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Web;
    using Merchello.Web.Workflow;

    using NUnit.Framework;

    [TestFixture, Category("LongRunning")]
    public class CheckoutManagerTests : CheckoutManagersTestBase
    {
        [SetUp]
        public void TestSetup()
        {
            this.CurrentCustomer.Basket().Empty();
        }

        /// <summary>
        /// Test shows a CheckoutContext can be generated from a basket
        /// </summary>
        /// <remarks>
        /// The extension method CreateCheckoutContext is internal for testing purposes.  Generally, the dev will use the GetCheckoutManager() extension
        /// </remarks>
        [Test]
        public void Can_Create_A_CheckoutContext()
        {
            this.WriteBasketInfoToConsole();

            //// Arrange
            CurrentCustomer.Basket().AddItem(_product1, 10);
            this.WriteBasketInfoToConsole();

            //// Act
            var context = this.CurrentCustomer.Basket().CreateCheckoutContext(MerchelloContext.Current, new CheckoutContextChangeSettings());

            //// Assert
            Assert.NotNull(context);
            Assert.NotNull(context.Services);
            Assert.NotNull(context.Gateways);
            Assert.IsTrue(context.ApplyTaxesToInvoice);
            Assert.AreEqual(1, context.ItemCache.Items.Count);
        }

        /// <summary>
        /// Test shows 
        /// </summary>
        [Test]
        public void Can_Get_CustomerCheckoutManager()
        {
            this.WriteBasketInfoToConsole();

            //// Arrange
            CurrentCustomer.Basket().AddItem(_product1, 10);
            CurrentCustomer.Basket().AddItem(_product2, 5);
            CurrentCustomer.Basket().AddItem(_product2, 1);
            this.CurrentCustomer.Basket().Save();
            this.WriteBasketInfoToConsole();

            //// Act
            var checkoutManager = this.CurrentCustomer.Basket().GetCheckoutManager();

            //// Assert
            Assert.NotNull(checkoutManager);
            Assert.NotNull(checkoutManager.Customer, "Customer manager was null");
            Assert.NotNull(checkoutManager.Extended, "Extended manager was null");
            Assert.NotNull(checkoutManager.Offer, "Offer manager was null");
            Assert.NotNull(checkoutManager.Payment, "Payment manager was null");
            Assert.NotNull(checkoutManager.Shipping, "Shipping manager was null");
        }


        /// <summary>
        /// Test proves the proper triggering of IsNewVersion (this flag is used to clear temporary information internally)
        /// </summary>
        [Test]
        public void Can_Show_Adding_An_Additional_Item_To_Basket_Triggers_IsNewVersion()
        {
            this.WriteBasketInfoToConsole();

            //// Arrange
            CurrentCustomer.Basket().AddItem(_product1, 10);
            this.CurrentCustomer.Basket().Save();

            //// Act/Assert
            Assert.IsTrue(this.CurrentCustomer.Basket().GetCheckoutManager().Context.IsNewVersion, "Initialized as new version");
            Assert.IsFalse(this.CurrentCustomer.Basket().GetCheckoutManager().Context.IsNewVersion, "Second call should not be a new version");
            CurrentCustomer.Basket().AddItem(_product2, 5);
            this.CurrentCustomer.Basket().Save();
            Assert.IsTrue(this.CurrentCustomer.Basket().GetCheckoutManager().Context.IsNewVersion, "Basket version changed so new version should be true");
        }

        /// <summary>
        /// Test shows that customer addresses can be saved
        /// </summary>
        [Test]
        public void Can_Save_And_Get_Customer_Addresses()
        {
            this.WriteBasketInfoToConsole();

            //// Arrange
            CurrentCustomer.Basket().AddItem(_product1, 10);
            this.CurrentCustomer.Basket().Save();
            var shipping = MockAddressMaker.GetAddress();
            var billing = MockAddressMaker.GetAddress();

            var checkoutManager = this.CurrentCustomer.Basket().GetCheckoutManager();

            //// Act
            checkoutManager.Customer.SaveShipToAddress(shipping);
            checkoutManager.Customer.SaveBillToAddress(billing);
            var savedShipping = checkoutManager.Customer.GetShipToAddress();
            var savedBilling = checkoutManager.Customer.GetBillToAddress();

            //// Assert
            Assert.NotNull(savedShipping);
            Assert.NotNull(savedBilling);
            Assert.AreEqual(savedShipping.Address1, shipping.Address1);
            Assert.AreEqual(savedBilling.Address1, billing.Address1);
        }

        /// <summary>
        /// Shows the customer address data are deleted (by default) when the basket version changes
        /// </summary>
        [Test]
        public void Can_Show_Customer_Addresses_Are_Cleared_After_VersionReset()
        {
            //// Arrange
            var shipping = MockAddressMaker.GetAddress();
            var billing = MockAddressMaker.GetAddress();

            var checkoutManager = this.CurrentCustomer.Basket().GetCheckoutManager();
            checkoutManager.Customer.SaveShipToAddress(shipping);
            checkoutManager.Customer.SaveBillToAddress(billing);

            //// Act
            CurrentCustomer.Basket().AddItem(_product1, 10);
            this.CurrentCustomer.Basket().Save();

            Assert.IsNull(this.CurrentCustomer.Basket().GetCheckoutManager().Customer.GetShipToAddress());
            Assert.IsNull(this.CurrentCustomer.Basket().GetCheckoutManager().Customer.GetBillToAddress());
        }

        /// <summary>
        /// Shows the customer address data are deleted (by default) when the basket version changes
        /// </summary>
        [Test]
        public void Can_Show_Customer_Addresses_Are_Not_Cleared_After_VersionReset_WithCustomSettings()
        {
            //// Arrange
            var shipping = MockAddressMaker.GetAddress();
            var billing = MockAddressMaker.GetAddress();
            var settings = new CheckoutContextChangeSettings() { ResetCustomerManagerDataOnVersionChange = false };

            var checkoutManager = this.CurrentCustomer.Basket().GetCheckoutManager();
            checkoutManager.Customer.SaveShipToAddress(shipping);
            checkoutManager.Customer.SaveBillToAddress(billing);

            //// Act
            CurrentCustomer.Basket().AddItem(_product1, 10);
            this.CurrentCustomer.Basket().Save();

            Assert.NotNull(this.CurrentCustomer.Basket().GetCheckoutManager(settings).Customer.GetShipToAddress());
            Assert.NotNull(this.CurrentCustomer.Basket().GetCheckoutManager(settings).Customer.GetBillToAddress());
        }

        /// <summary>
        /// Test asserts that a shipment rate quote can be saved and it generates a shipping line item in the ItemCache when saved
        /// </summary>
        [Test]
        public void Can_Save_A_Shipment_Rate_Quote()
        {
            //// Arrange
            CurrentCustomer.Basket().AddItem(_product1, 10);
            this.CurrentCustomer.Basket().Save();
            var shipping = MockAddressMaker.GetAddress("US");

            var checkoutManager = this.CurrentCustomer.Basket().GetCheckoutManager();
            checkoutManager.Customer.SaveShipToAddress(shipping);

            //// Act
            var shipment = this.CurrentCustomer.Basket().PackageBasket(shipping).FirstOrDefault();
            Assert.NotNull(shipment);

            var quotes = shipment.ShipmentRateQuotes().ToArray();
            Assert.NotNull(quotes);
            Assert.IsTrue(quotes.Any(), "The collection of quotes was empty");
            
            checkoutManager.Shipping.SaveShipmentRateQuote(quotes.First());

            //// Assert
            var shippingLineItems =
                checkoutManager.Context.ItemCache.Items.Where(x => x.LineItemType == LineItemType.Shipping);

            Assert.IsTrue(shippingLineItems.Any());
        }

        /// <summary>
        /// Shows that an invoice can be generated by the PaymentManager
        /// </summary>
        [Test]
        public void Can_Generate_An_Invoice_For_Preview()
        {
            //// Arrange
            CurrentCustomer.Basket().AddItem(_product1, 10);
            CurrentCustomer.Basket().AddItem(_product2, 5);
            CurrentCustomer.Basket().AddItem(_product3, 1);

            this.CurrentCustomer.Basket().Save();
            var shipping = MockAddressMaker.GetAddress("US");
            var billing = MockAddressMaker.GetAddress("US");

            var checkoutManager = this.CurrentCustomer.Basket().GetCheckoutManager();

            checkoutManager.Customer.SaveShipToAddress(shipping);
            checkoutManager.Customer.SaveBillToAddress(billing);

            var shipment = this.CurrentCustomer.Basket().PackageBasket(shipping).FirstOrDefault();
            var quotes = shipment.ShipmentRateQuotes().ToArray();
            Assert.NotNull(quotes);
            Assert.IsTrue(quotes.Any(), "The collection of quotes was empty");

            checkoutManager.Shipping.SaveShipmentRateQuote(quotes.First());

            //// Act
            checkoutManager.Payment.InvoiceNumberPrefix = "rss";
            var invoice = checkoutManager.Payment.PrepareInvoice();

            //// Assert
            Assert.NotNull(invoice);
            Assert.IsTrue(invoice.PrefixedInvoiceNumber().StartsWith("rss"));

        }
    }
}