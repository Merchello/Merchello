namespace Merchello.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A <see cref="SurfaceController"/> responsible for checkout operations.
    /// </summary>
    [PluginController("Bazaar")]
    public class SalePreparationOperationsController : SurfaceControllerBase
    {
        /// <summary>
        /// Saves addresses during the checkout process.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult SaveAddresses(CheckoutAddressForm model)
        {
            if (!ModelState.IsValid) return this.CurrentUmbracoPage();

            var preparation = Basket.SalePreparation();
            preparation.RaiseCustomerEvents = false;

            preparation.SaveBillToAddress(model.GetBillingAddress());
            preparation.SaveShipToAddress(model.GetShippingAddress());
            
            return RedirectToUmbracoPage(model.ConfirmSalePageId);
        }

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

            // TODO This only works for cash payments - but is only for today's demo.
            var paymentMethod = GatewayContext.Payment.GetPaymentGatewayMethodByKey(model.PaymentMethodKey).PaymentMethod;
            preparation.SavePaymentMethod(paymentMethod);

            // AuthorizePayment will save the invoice with an Invoice Number.
            var attempt = preparation.AuthorizePayment(paymentMethod.Key);

            if (!attempt.Payment.Success)
            {
                return this.CurrentUmbracoPage();   
            }

            // Trigger the order confirmation notification
            var billingAddress = attempt.Invoice.GetBillingAddress();
            if (!string.IsNullOrEmpty(billingAddress.Email))
            {
                Notification.Trigger("OrderConfirmation", attempt.Payment, new[] { billingAddress.Email });
            }
            
            // store the invoice key in the CustomerContext for use on the receipt page.
            CustomerContext.SetValue("invoiceKey", attempt.Invoice.Key.ToString());

            return RedirectToUmbracoPage(model.ReceiptPageId);
            
        }
    }
}