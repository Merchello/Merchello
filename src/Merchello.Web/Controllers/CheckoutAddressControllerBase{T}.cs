﻿namespace Merchello.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;

    /// <summary>
    /// A base controller to handle checkout address operations.
    /// </summary>
    /// <typeparam name="TBillingAddress">
    /// The type of the billing address
    /// </typeparam>
    /// <typeparam name="TShippingAddress">
    /// The type of the shipping address
    /// </typeparam>
    public abstract class CheckoutAddressControllerBase<TBillingAddress, TShippingAddress> : CheckoutControllerBase
        where TBillingAddress : class, ICheckoutAddressModel, new()
        where TShippingAddress : class, ICheckoutAddressModel, new()
    {
        /// <summary>
        /// A value indicating whether or not to use the default customer address (of type) to pre load form values.
        /// </summary>
        private readonly bool _useCustomerAddress;

        /// <summary>
        /// The <see cref="CheckoutAddressModelFactory{TBillingAddress}"/>.
        /// </summary>
        private readonly CheckoutAddressModelFactory<TBillingAddress> _billingAddressFactory;

        /// <summary>
        /// The <see cref="CheckoutAddressModelFactory{TShippingAddress}"/>.
        /// </summary>
        private readonly CheckoutAddressModelFactory<TShippingAddress> _shippingAddressFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutAddressControllerBase{TBillingAddress,TShippingAddress}"/> class.
        /// </summary>
        /// <param name="initializeFromCustomerAddress">
        /// A value indicating whether or not to attempt to initialize address forms with default customer address
        /// (if available)
        /// </param>
        protected CheckoutAddressControllerBase(bool initializeFromCustomerAddress = true)
            : this(
                  new CheckoutAddressModelFactory<TBillingAddress>(), 
                  new CheckoutAddressModelFactory<TShippingAddress>(), 
                  new CheckoutContextSettingsFactory(),
                  initializeFromCustomerAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutAddressControllerBase{TBillingAddress,TShippingAddress}"/> class.
        /// </summary>
        /// <param name="billingAddressFactory">
        /// The billing address factory.
        /// </param>
        /// <param name="shippingAddressFactory">
        /// The shipping address factory.
        /// </param>
        /// <param name="initializeFromCustomerAddress">
        /// A value indicating whether or not to attempt to initialize address forms with default customer address
        /// (if available)
        /// </param>
        protected CheckoutAddressControllerBase(
            CheckoutAddressModelFactory<TBillingAddress> billingAddressFactory,
            CheckoutAddressModelFactory<TShippingAddress> shippingAddressFactory,
            bool initializeFromCustomerAddress = true)
            : this(billingAddressFactory, shippingAddressFactory, new CheckoutContextSettingsFactory(), initializeFromCustomerAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutAddressControllerBase{TBillingAddress,TShippingAddress}"/> class.
        /// </summary>
        /// <param name="billingAddressFactory">
        /// The <see cref="CheckoutAddressModelFactory{TBillingAddress}"/>
        /// </param>
        /// <param name="shippingAddressFactory">
        /// The <see cref="CheckoutAddressModelFactory{TShippingAddress}"/>
        /// </param>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        /// <param name="initializeFromCustomerAddress">
        /// A value indicating whether or not to attempt to initialize address forms with default customer address
        /// (if available)
        /// </param>
        protected CheckoutAddressControllerBase(
            CheckoutAddressModelFactory<TBillingAddress> billingAddressFactory,
            CheckoutAddressModelFactory<TShippingAddress> shippingAddressFactory,
            CheckoutContextSettingsFactory contextSettingsFactory,
            bool initializeFromCustomerAddress = true)
            : base(contextSettingsFactory)
        {
            Mandate.ParameterNotNull(billingAddressFactory, "billingAddressFactory");
            Mandate.ParameterNotNull(shippingAddressFactory, "shippingAddressFactory");

            this._billingAddressFactory = billingAddressFactory;
            this._shippingAddressFactory = shippingAddressFactory;
            this._useCustomerAddress = initializeFromCustomerAddress;
        }

        /// <summary>
        /// Saves the <see cref="TBillingAddress"/> for use in the checkout.
        /// </summary>
        /// <param name="model">
        /// The <see cref="TBillingAddress"/>.
        /// </param>
        /// <returns>
        /// Redirects or JSON response depending if called Async.
        /// </returns>
        [HttpPost]
        public virtual ActionResult SaveBillingAddress(TBillingAddress model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            this.CheckoutManager.Customer.SaveBillToAddress(this._billingAddressFactory.Create(model));

            if (!this.CurrentCustomer.IsAnonymous) this.SaveCustomerAddress(model);

            model.WorkflowMarker = GetNextCheckoutWorkflowMarker(CheckoutStage.BillingAddress);

            return this.RedirectAddressSaveSuccess(model);
        }

        /// <summary>
        /// Saves the <see cref="TShippingAddress"/> for use in the checkout
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        [HttpPost]
        public virtual ActionResult SaveShippingAddress(TShippingAddress model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            throw new NotImplementedException();
        }

        #region ChildActions

        /// <summary>
        /// Renders the billing address form.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult BillingAddressForm()
        {
            // TODO Country Code and Region

            ICustomerAddress defaultBilling = null;
            if (!this.CurrentCustomer.IsAnonymous && this._useCustomerAddress)
            {
                defaultBilling = ((ICustomer)this.CurrentCustomer).DefaultCustomerAddress(AddressType.Billing);
            }

            return defaultBilling == null
                       ? this.PartialView()
                       : this.PartialView(this._billingAddressFactory.Create((ICustomer)this.CurrentCustomer, defaultBilling));
        }

        /// <summary>
        /// Renders the shipping address form.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult ShippingAddressForm()
        {
            // TODO Country Code and Region

            ICustomerAddress defaultShipping = null;
            if (!this.CurrentCustomer.IsAnonymous && this._useCustomerAddress)
            {
                defaultShipping = ((ICustomer)this.CurrentCustomer).DefaultCustomerAddress(AddressType.Shipping);
            }

            return defaultShipping == null
                   ? this.PartialView()
                   : this.PartialView(this._shippingAddressFactory.Create((ICustomer)this.CurrentCustomer, defaultShipping));
        }

        #endregion

        /// <summary>
        /// Allows for saving the address to the customer.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <typeparam name="TAddress">
        /// The type of address to be saved
        /// </typeparam>
        protected virtual void SaveCustomerAddress<TAddress>(TAddress model) where TAddress : ICheckoutAddressModel
        {
        }

        /// <summary>
        /// Allows for overriding the redirection of a successful address save.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <typeparam name="TAddress">
        /// The type of address saved
        /// </typeparam>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult RedirectAddressSaveSuccess<TAddress>(TAddress model)
            where TAddress : ICheckoutAddressModel
        {
            return this.RedirectToCurrentUmbracoPage();
        }

    }
}