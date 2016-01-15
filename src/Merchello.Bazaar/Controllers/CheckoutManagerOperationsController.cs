namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Mvc;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// A <see cref="SurfaceController"/> responsible for checkout operations.
    /// </summary>
    [PluginController("Bazaar")]
    public partial class CheckoutManagerOperationsController : MerchelloSurfaceController
    {
        /// <summary>
        /// The checkout manager.
        /// </summary>
        private ICheckoutManagerBase _checkoutManager;

        /// <summary>
        /// Gets the checkout manager.
        /// </summary>
        private ICheckoutManagerBase CheckoutManager
        {
            get
            {
                if (this._checkoutManager == null)
                {                    
                    this._checkoutManager = Basket.GetCheckoutManager();
                    this.CheckoutManager.Context.RaiseCustomerEvents = false;
                }

                return this._checkoutManager;
            }
        }

        /// <summary>
        /// Tries to redeem a coupon offer.
        /// </summary>
        /// <param name="model">
        /// The <see cref="RedeemCouponOfferForm"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RedeemCouponOffer(RedeemCouponOfferForm model)
        {
            if (string.IsNullOrEmpty(model.OfferCode)) return this.CurrentUmbracoPage();

            var result = CheckoutManager.Offer.RedeemCouponOffer(model.OfferCode);
            ViewBag.CouponRedemptionResult = result;
            return this.CurrentUmbracoPage();
        }

        /// <summary>
        /// Removes the coupon.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="redirectId">
        /// The content id of the page to redirect to
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult RemoveCoupon(string offerCode, int redirectId)
        {
            CheckoutManager.Offer.RemoveOfferCode(offerCode);
            return this.RedirectToUmbracoPage(redirectId);
        }

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
                isValid = ModelState.IsValidField("BillingName") && ModelState.IsValidField("BillingEmail")
                          && ModelState.IsValidField("BillingAddress1") && ModelState.IsValidField("BillingLocality")
                          && ModelState.IsValidField("BillingPostalCode");
            }
            if (!isValid) return this.CurrentUmbracoPage();

            if (model.ShippingAddressKey.Equals(Guid.Empty))
            {
                isValid = ModelState.IsValidField("ShippingName") && ModelState.IsValidField("ShippingEmail")
                          && ModelState.IsValidField("ShippingAddress1") && ModelState.IsValidField("ShippingLocality")
                          && ModelState.IsValidField("ShippingPostalCode");
            }

            if (!isValid) return this.CurrentUmbracoPage();

            var saveBilling = false;
            var saveShipping = false;

            IAddress billingAddress;
            if (!model.BillingAddressKey.Equals(Guid.Empty))
            {
                var billing = MerchelloServices.CustomerService.GetAddressByKey(model.BillingAddressKey);
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
                var shipping = MerchelloServices.CustomerService.GetAddressByKey(model.ShippingAddressKey);
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
                var customer = (ICustomer)CurrentCustomer;

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

            CheckoutManager.Customer.SaveBillToAddress(billingAddress);
            CheckoutManager.Customer.SaveShipToAddress(shippingAddress);

            return RedirectToUmbracoPage(model.ConfirmSalePageId);
        }
    }
}