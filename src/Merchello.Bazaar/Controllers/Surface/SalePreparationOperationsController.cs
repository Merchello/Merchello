namespace Merchello.Bazaar.Controllers.Surface
{
    using System;
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
            // we have to do custom validation here since there are so many 
            // different variations of the submitted model
            var isValid = true;
            if (model.BillingAddressKey.Equals(Guid.Empty))
            {
                isValid = this.ModelState.IsValidField("BillingName") && this.ModelState.IsValidField("BillingEmail")
                          && this.ModelState.IsValidField("BillingAddress1") && this.ModelState.IsValidField("BillingLocality")
                          && this.ModelState.IsValidField("BillingPostalCode");
            }
            if (!isValid) return this.CurrentUmbracoPage();

            if (model.ShippingAddressKey.Equals(Guid.Empty))
            {
                isValid = this.ModelState.IsValidField("ShippingName") && this.ModelState.IsValidField("ShippingEmail")
                          && this.ModelState.IsValidField("ShippingAddress1") && this.ModelState.IsValidField("ShippingLocality")
                          && this.ModelState.IsValidField("ShippingPostalCode");
            }

            if (!isValid) return this.CurrentUmbracoPage();

            var preparation = this.Basket.SalePreparation();
            preparation.RaiseCustomerEvents = false;

            var saveBilling = false;
            var saveShipping = false;

            IAddress billingAddress;
            if (!model.BillingAddressKey.Equals(Guid.Empty))
            {
                var billing = this.MerchelloServices.CustomerService.GetAddressByKey(model.BillingAddressKey);
                billingAddress = billing.AsAddress(billing.FullName);
            }
            else
            {
                billingAddress = model.GetBillingAddress();
                saveBilling = true;
            }

            IAddress shippingAddress;
            if (!model.ShippingAddressKey.Equals(Guid.Empty))
            {
                var shipping = this.MerchelloServices.CustomerService.GetAddressByKey(model.ShippingAddressKey);
                shippingAddress = shipping.AsAddress(shipping.FullName);
            }
            else
            {
                shippingAddress = model.GetShippingAddress();
                saveShipping = true;
            }

            if (model.SaveCustomerAddress)
            {
                var redirect = (saveBilling && !this.ModelState.IsValidField("BillingAddressLabel")) || 
                    (saveShipping && (!this.ModelState.IsValidField("ShippingAddressLabel") && !model.BillingIsShipping));
                if (redirect) return this.CurrentUmbracoPage();
                
                //// at this point we know the customer is an ICustomer 
                var customer = (ICustomer)this.CurrentCustomer;
                
                if (saveBilling)
                {
                    customer.CreateCustomerAddress(billingAddress, model.BillingAddressLabel, AddressType.Billing);
                }

                if (saveShipping)
                {
                    if (model.BillingIsShipping) model.ShippingAddressLabel = model.BillingAddressLabel;
                    customer.CreateCustomerAddress(shippingAddress, model.ShippingAddressLabel, AddressType.Shipping);
                }
            }

            preparation.SaveBillToAddress(billingAddress);
            preparation.SaveShipToAddress(shippingAddress);
            
            return this.RedirectToUmbracoPage(model.ConfirmSalePageId);
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
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            var preparation = this.Basket.SalePreparation();
            preparation.RaiseCustomerEvents = false;
           

            preparation.ClearShipmentRateQuotes();
            var shippingAddress = this.Basket.SalePreparation().GetShipToAddress();

            // Get the shipment again
            var shipment = this.Basket.PackageBasket(shippingAddress).FirstOrDefault();

            // get the quote using the "approved shipping method"
            var quote = shipment.ShipmentRateQuoteByShipMethod(model.ShipMethodKey);

            // save the quote
            this.Basket.SalePreparation().SaveShipmentRateQuote(quote);

            var paymentMethod = this.GatewayContext.Payment.GetPaymentGatewayMethodByKey(model.PaymentMethodKey).PaymentMethod;
            preparation.SavePaymentMethod(paymentMethod);

            // AuthorizePayment will save the invoice with an Invoice Number.
            var attempt = preparation.AuthorizePayment(paymentMethod.Key);

            if (!attempt.Payment.Success)
            {
                return this.CurrentUmbracoPage();   
            }

            // Trigger the order confirmation notification
            var billingAddress = attempt.Invoice.GetBillingAddress();
            string contactEmail;
            if (string.IsNullOrEmpty(billingAddress.Email) && !this.CurrentCustomer.IsAnonymous)
            {
                contactEmail = ((ICustomer)this.CurrentCustomer).Email;
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
            this.CustomerContext.SetValue("invoiceKey", attempt.Invoice.Key.ToString());

            return this.RedirectToUmbracoPage(model.ReceiptPageId);
            
        }
    }
}