namespace Merchello.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

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

        #region Constructors

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

            this.BillingAddressFactory = billingAddressFactory;
            this.ShippingAddressFactory = shippingAddressFactory;
            this._useCustomerAddress = initializeFromCustomerAddress;
        }

        #endregion

        /// <summary>
        /// Gets the billing address factory.
        /// </summary>
        protected CheckoutAddressModelFactory<TBillingAddress> BillingAddressFactory { get; private set; }

        /// <summary>
        /// Gets the shipping address factory.
        /// </summary>
        protected CheckoutAddressModelFactory<TShippingAddress> ShippingAddressFactory { get; private set; }

        /// <summary>
        /// Saves the <see cref="ICheckoutAddressModel"/> for use in the checkout.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <returns>
        /// Redirects or JSON response depending if called Async.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SaveBillingAddress(TBillingAddress model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            try
            {
                // Ensure billing address type is billing
                if (model.AddressType != AddressType.Billing) model.AddressType = AddressType.Billing;

                var address = BillingAddressFactory.Create(model);

                // Temporarily save the address in the checkout manager.
                this.CheckoutManager.Customer.SaveBillToAddress(address);

                if (!this.CurrentCustomer.IsAnonymous) this.SaveCustomerBillingAddress(model);

                model.WorkflowMarker = GetNextCheckoutWorkflowMarker(CheckoutStage.BillingAddress);

                return this.HandleBillingAddressSaveSuccess(model);
            }
            catch (Exception ex)
            {
                return this.HandleBillingAddressSaveException(model, ex);
            }
        }

        /// <summary>
        /// Saves the <see cref="ICheckoutAddressModel"/> for use in the checkout
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SaveShippingAddress(TShippingAddress model)
        {
            if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

            try
            {
                // Ensure billing address type is billing
                if (model.AddressType != AddressType.Shipping) model.AddressType = AddressType.Shipping;

                var address = ShippingAddressFactory.Create(model);

                // Temporarily save the address in the checkout manager.
                this.CheckoutManager.Customer.SaveShipToAddress(address);

                if (!this.CurrentCustomer.IsAnonymous) this.SaveCustomerShippingAddress(model);

                model.WorkflowMarker = GetNextCheckoutWorkflowMarker(CheckoutStage.ShippingAddress);

                return this.HandleShippingAddressSaveSuccess(model);
            }
            catch (Exception ex)
            {
                return this.HandleShippingAddressSaveException(model, ex);
            }
        }

        #region ChildActions

        /// <summary>
        /// Renders the billing address form.
        /// </summary>
        /// <param name="view">
        /// The view to display.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult BillingAddressForm(string view = "")
        {
            TBillingAddress model = null;

            // Determine if we already have an address saved in the checkout manager
            var address = CheckoutManager.Customer.GetBillToAddress();
            if (address != null)
            {
                model = BillingAddressFactory.Create(address);
            }
            else
            {
                // If not and the we have the configuration set to use the customer's default customer billing address
                // This can only be done if the customer is logged in.  e.g. Not an anonymous customer
                if (!this.CurrentCustomer.IsAnonymous && this._useCustomerAddress)
                {
                    var defaultBilling = ((ICustomer)this.CurrentCustomer).DefaultCustomerAddress(AddressType.Billing);
                    if (defaultBilling != null) model = BillingAddressFactory.Create((ICustomer)CurrentCustomer, defaultBilling);
                }
            }

            // If the model is still null at this point, we need to generate a default model
            // for the country drop down list
            if (model == null) model = BillingAddressFactory.Create(new Address());

            return view.IsNullOrWhiteSpace() 
                       ? this.PartialView(model)
                       : this.PartialView(view, model);
        }

        /// <summary>
        /// Renders the shipping address form.
        /// </summary>
        /// <param name="view">
        /// The view to display.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public ActionResult ShippingAddressForm(string view = "")
        {
            var billingAddress = CheckoutManager.Customer.GetBillToAddress();
            if (billingAddress == null) return InvalidCheckoutStagePartial();

            TShippingAddress model = null;

            // Determine if we already have an address saved in the checkout manager
            var address = CheckoutManager.Customer.GetShipToAddress();
            if (address != null)
            {
                model = ShippingAddressFactory.Create(address);
            }
            else
            {
                // If not and the we have the configuration set to use the customer's default customer shipping address
                // This can only be done if the customer is logged in.  e.g. Not an anonymous customer
                if (!this.CurrentCustomer.IsAnonymous && this._useCustomerAddress)
                {
                    var defaultShipping = ((ICustomer)this.CurrentCustomer).DefaultCustomerAddress(AddressType.Shipping);
                    if (defaultShipping != null) model = ShippingAddressFactory.Create((ICustomer)CurrentCustomer, defaultShipping);
                }
            }

            // If the model is still null at this point, we need to generate a default model
            // for the country drop down list
            if (model == null) model = ShippingAddressFactory.Create(new Address());

            return view.IsNullOrWhiteSpace()
                       ? this.PartialView(model)
                       : this.PartialView(view, model);
        }

        #endregion

        /// <summary>
        /// Allows for saving the billing address to the customer.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        protected virtual void SaveCustomerBillingAddress(TBillingAddress model)
        {
        }

        /// <summary>
        /// Allows for saving the shipping address to the customer.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        protected virtual void SaveCustomerShippingAddress(TShippingAddress model)
        {
        }

        /// <summary>
        /// Allows for overriding the action of a successful billing address save.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleBillingAddressSaveSuccess(TBillingAddress model)
        {
            return this.RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// Handles a billing address save exception.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// The <see cref="Exception"/> to be handled
        /// </exception>
        protected virtual ActionResult HandleBillingAddressSaveException(TBillingAddress model, Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// Allows for overriding the action of a successful shipping address save.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleShippingAddressSaveSuccess(TShippingAddress model)
        {
            return this.RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// Handles a shipping address save exception.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutAddressModel"/>.
        /// </param>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// The <see cref="Exception"/> to be handled
        /// </exception>
        protected virtual ActionResult HandleShippingAddressSaveException(TShippingAddress model, Exception ex)
        {
            throw ex;
        }
    }
}