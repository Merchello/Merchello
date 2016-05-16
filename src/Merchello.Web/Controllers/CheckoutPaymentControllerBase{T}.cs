namespace Merchello.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Mvc;

    using Umbraco.Core;

    /// <summary>
    /// A base controller responsible for handling payment method selection operations..
    /// </summary>
    /// <typeparam name="TPaymentModel">
    /// The type of the <see cref="ICheckoutPaymentModel"/>
    /// </typeparam>
    public abstract class CheckoutPaymentControllerBase<TPaymentModel> : CheckoutControllerBase, IPaymentMethodUiController
        where TPaymentModel : class, ICheckoutPaymentModel, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        protected CheckoutPaymentControllerBase()
            : this(new CheckoutPaymentModelFactory<TPaymentModel>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        /// <param name="checkoutPaymentModelFactory">
        /// The <see cref="CheckoutPaymentModelFactory{TPaymentModel}"/>.
        /// </param>
        protected CheckoutPaymentControllerBase(CheckoutPaymentModelFactory<TPaymentModel> checkoutPaymentModelFactory)
            : this(checkoutPaymentModelFactory, new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        /// <param name="checkoutPaymentModelFactory">
        /// The <see cref="CheckoutPaymentModelFactory{TPaymentModel}"/>.
        /// </param>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected CheckoutPaymentControllerBase(
            CheckoutPaymentModelFactory<TPaymentModel> checkoutPaymentModelFactory,
            CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
            Mandate.ParameterNotNull(checkoutPaymentModelFactory, "checkoutPaymentModelFactory");
            this.CheckoutPaymentModelFactory = checkoutPaymentModelFactory;

            // ensures the sub class has a GatewayMethodUiAttribute
            EnsureAttribute();
        }

        #endregion

        /// <summary>
        /// Gets the checkout payment model factory.
        /// </summary>
        protected CheckoutPaymentModelFactory<TPaymentModel> CheckoutPaymentModelFactory { get; private set; }

        /// <summary>
        /// Responsible for rendering the payment form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public abstract ActionResult PaymentForm(string view = "");

        /// <summary>
        /// Sets the invoice number prefix.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        protected virtual void SetInvoiceNumberPrefix(string prefix)
        {
            CheckoutManager.Context.Settings.InvoiceNumberPrefix = prefix;
        }

        /// <summary>
        /// Clears invoice number prefix.
        /// </summary>
        protected virtual void ClearInvoiceNumberPrefix()
        {
            SetInvoiceNumberPrefix(string.Empty);
        }

        /// <summary>
        /// Handles the successful payment.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandlePaymentSuccess(TPaymentModel model)
        {
            if (!model.ViewData.Success)
            {
                ViewData["MerchelloViewData"] = model.ViewData; 
                return CurrentUmbracoPage();
            }

            return RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// Handles a payment exception.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentModel"/>.
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
        protected virtual ActionResult HandlePaymentException(TPaymentModel model, Exception ex)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");
            MultiLogHelper.Error<CheckoutPaymentControllerBase<TPaymentModel>>("Failed payment operation.", ex, logData);
            throw ex;
        }

        /// <summary>
        /// Gets the <see cref="IInvoice"/>.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="IInvoice"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the invoice was not found
        /// </exception>
        protected virtual IInvoice GetInvoice(Guid invoiceKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
            var invoice = MerchelloServices.InvoiceService.GetByKey(invoiceKey);
            if (invoice == null)
            {
                throw new NullReferenceException("Invoice was not found.");
            }

            return invoice;
        }

        /// <summary>
        /// Gets the <see cref="IPayment"/>.
        /// </summary>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <returns>
        /// The <see cref="IPayment"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the payment was not found
        /// </exception>
        protected virtual IPayment GetPayment(Guid paymentKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(paymentKey), "paymentKey");
            var payment = MerchelloServices.PaymentService.GetByKey(paymentKey);
            if (payment == null)
            {
                throw new NullReferenceException("Payment was not found.");
            }

            return payment;
        }

        /// <summary>
        /// Ensures the class is decorated with a <see cref="GatewayMethodUiAttribute"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the <see cref="GatewayMethodUiAttribute"/> is not found
        /// </exception>
        private void EnsureAttribute()
        {
            // Ensure GatewayMethodUiAttribute
            var att = GetType().GetCustomAttribute<GatewayMethodUiAttribute>(true);
            if (att == null)
            {
                var logData = MultiLogger.GetBaseLoggingData();
                logData.AddCategory("Controllers");
                var nullRef = new NullReferenceException("Implementation of CheckoutPaymentControllerBase must be decorated with a GatewayMethodUiAttribute");
                MultiLogHelper.Error<CheckoutPaymentControllerBase<TPaymentModel>>("GatewayMethodUiAttribute was not found.", nullRef, logData);
                throw nullRef;
            }
        }
    }
}