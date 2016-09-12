namespace Merchello.Tests.Unit.Configurations.MerchelloSettings
{
    using NUnit.Framework;

    [TestFixture]
    public class CheckoutContextElementTests : MerchelloSettingsTests
    {
        [Test]
        public void InvoiceNumberPrefix()
        {
            //// Arrange
            const string expected = "FT";

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.InvoiceNumberPrefix;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void ApplyTaxesToInvoice()
        {
            //// Arrange
            const bool expected = true;

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.ApplyTaxesToInvoice;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void RaiseCustomerEvents()
        {
            //// Arrange
            const bool expected = false;

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.RaiseCustomerEvents;

            //// Assert
            Assert.AreEqual(expected, value);
        }


        [Test]
        public void ResetCustomerManagerDataOnVersionChange()
        {
            //// Arrange
            const bool expected = false;

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.ResetCustomerManagerDataOnVersionChange;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void ResetPaymentManagerDataOnVersionChange()
        {
            //// Arrange
            const bool expected = true;

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.ResetPaymentManagerDataOnVersionChange;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void ResetExtendedManagerDataOnVersionChange()
        {
            //// Arrange
            const bool expected = true;

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.ResetExtendedManagerDataOnVersionChange;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void ResetShippingManagerDataOnVersionChange()
        {
            //// Arrange
            const bool expected = true;

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.ResetShippingManagerDataOnVersionChange;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void ResetOfferManagerDataOnVersionChange()
        {
            //// Arrange
            const bool expected = true;

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.ResetOfferManagerDataOnVersionChange;

            //// Assert
            Assert.AreEqual(expected, value);
        }

        [Test]
        public void EmptyBasketOnPaymentSuccess()
        {
            //// Arrange
            const bool expected = true;

            //// Act
            var value = SettingsSection.Checkout.CheckoutContext.EmptyBasketOnPaymentSuccess;

            //// Assert
            Assert.AreEqual(expected, value);
        }
    }
}