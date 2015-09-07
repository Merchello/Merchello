namespace Merchello.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Web;
    using Merchello.Web.Mvc;

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

            var preparation = Basket.SalePreparation();
            preparation.RaiseCustomerEvents = false;
           

            preparation.ClearShipmentRateQuotes();
            var shippingAddress = Basket.SalePreparation().GetShipToAddress();

            // Get the shipment again
            var shipment = Basket.PackageBasket(shippingAddress).FirstOrDefault();

            // get the quote using the "approved shipping method"
            var quote = shipment.ShipmentRateQuoteByShipMethod(model.ShipMethodKey);

            // save the quote
            Basket.SalePreparation().SaveShipmentRateQuote(quote);

            var paymentMethod = GatewayContext.Payment.GetPaymentGatewayMethodByKey(model.PaymentMethodKey).PaymentMethod;
            preparation.SavePaymentMethod(paymentMethod);

            // AuthorizePayment will save the invoice with an Invoice Number.
            var attempt = this.PerformProcessPayment(preparation, paymentMethod);

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
        /// <param name="preparation">
        /// The preparation.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment method.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected abstract IPaymentResult PerformProcessPayment(SalePreparationBase preparation, IPaymentMethod paymentMethod);
    }
}