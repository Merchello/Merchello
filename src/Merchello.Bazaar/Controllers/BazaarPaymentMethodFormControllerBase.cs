namespace Merchello.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Models;
    using Core;
    using Core.Gateways.Payment;
    using Core.Models;
    using Web;
    using Web.Mvc;
    using Core.Checkout;
    using Web.Workflow;

    /// <summary>
    /// The bazaar payment method form controller base.
    /// </summary>
    public abstract class BazaarPaymentMethodFormControllerBase : PaymentMethodUiController<CheckoutConfirmationForm>
    {
        /// <summary>
        /// The confirm sale.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmSale(CheckoutConfirmationForm model)
        {
            if (!ModelState.IsValid) return this.CurrentUmbracoPage();

            // Get all the objects we need
            var settings = new CheckoutManagerSettings() { InvoiceNumberPrefix = "BZR" };
            var checkoutManager = Basket.GetCheckoutManager();
            var customerManager = checkoutManager.Customer;
            var shippingManager = checkoutManager.Shipping;
            var paymentManager = checkoutManager.Payment;
        
            // Don't raise Customer events
            checkoutManager.Context.RaiseCustomerEvents = false;

            // Clear Shipment Rate Quotes
            shippingManager.ClearShipmentRateQuotes();

            // Get the shipping address
            var shippingAddress = customerManager.GetShipToAddress();

            // Get the shipment again
            var shipment = Basket.PackageBasket(shippingAddress).FirstOrDefault();

            // get the quote using the "approved shipping method"
            var quote = shipment.ShipmentRateQuoteByShipMethod(model.ShipMethodKey);

            // save the quote
            shippingManager.SaveShipmentRateQuote(quote);

            var paymentMethod = GatewayContext.Payment.GetPaymentGatewayMethodByKey(model.PaymentMethodKey).PaymentMethod;
            paymentManager.SavePaymentMethod(paymentMethod);

            // AuthorizePayment will save the invoice with an Invoice Number.
            var attempt = this.PerformProcessPayment(checkoutManager, paymentMethod);

            if (!attempt.Payment.Success)
            {
                return this.CurrentUmbracoPage();   
            }

            // Trigger the order confirmation notification
            var billingAddress = attempt.Invoice.GetBillingAddress();
            string contactEmail;
            if (string.IsNullOrEmpty(billingAddress.Email) && !CurrentCustomer.IsAnonymous)
            {
                contactEmail = ((ICustomer)CurrentCustomer).Email;
            }
            else
            {
                contactEmail = billingAddress.Email;
            }

            if (!string.IsNullOrEmpty(contactEmail))
            {
                Notification.Trigger("OrderConfirmation", attempt, new[] { contactEmail });
            }
            
            // store the invoice key in the CustomerContext for use on the receipt page.
            CustomerContext.SetValue("invoiceKey", attempt.Invoice.Key.ToString());

            return RedirectToUmbracoPage(model.ReceiptPageId);
        }

        /// <summary>
        /// Responsible for actually processing the payment with the PaymentProvider
        /// </summary>
        /// <param name="checkoutManager">
        /// The preparation.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected abstract IPaymentResult PerformProcessPayment(ICheckoutManagerBase checkoutManager, IPaymentMethod paymentMethod);
    }
}