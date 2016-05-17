namespace Merchello.Web.Store.Controllers.Payment
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Providers.Payment.Braintree;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Store.Factories;
    using Merchello.Web.Store.Models;
    using Merchello.Web.Store.Models.Async;

    /// <summary>
    /// A base controller for rendering and processing Braintree payments.
    /// </summary>
    /// <typeparam name="TPaymentModel">
    /// The type of <see cref="BraintreePaymentModel"/>
    /// </typeparam>
    public abstract class BraintreePaymentControllerBase<TPaymentModel> : CheckoutPaymentControllerBase<TPaymentModel>
        where TPaymentModel : BraintreePaymentModel, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        protected BraintreePaymentControllerBase()
            : this(
                  new BraintreePaymentModelFactory<TPaymentModel>(),
                  new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        /// <param name="checkoutPaymentModelFactory">
        /// The <see cref="BraintreePaymentModelFactory{TPaymentModel}"/>.
        /// </param>
        protected BraintreePaymentControllerBase(
            BraintreePaymentModelFactory<TPaymentModel> checkoutPaymentModelFactory)
            : this(checkoutPaymentModelFactory, new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        /// <param name="checkoutPaymentModelFactory">
        /// The <see cref="BraintreePaymentModelFactory{TPaymentModel}"/>.
        /// </param>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected BraintreePaymentControllerBase(
            BraintreePaymentModelFactory<TPaymentModel> checkoutPaymentModelFactory,
            CheckoutContextSettingsFactory contextSettingsFactory)
            : base(checkoutPaymentModelFactory, contextSettingsFactory)
        {
        }

        #endregion


        /// <summary>
        /// Processes a Braintree payment.
        /// </summary>
        /// <param name="nonce">
        /// The payment method nonce
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult Process(string nonce)
        {
            // We have to get the paymentMethod from the CheckoutManager BEFORE processing the payment
            // so we have it for the factory since the checkout manager may be cleared on payment success
            // depending on CheckoutContextSettings.ResetCheckoutManagerOnPaymentSuccess configuration
            var paymentMethod = CheckoutManager.Payment.GetPaymentMethod();

            try
            {
                var attempt = ProcessPayment(nonce);

                var model = CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod, attempt);

                // Send the notification
                HandleNotificiation(model, attempt);

                return HandlePaymentSuccess(model);
            }
            catch (Exception ex)
            {
                var model = CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod);
                model.Token = nonce;

                return HandlePaymentException(model, ex);
            }
        }

        /// <summary>
        /// Retries a payment.
        /// </summary>
        /// <param name="nonce">
        /// The nonce.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice Key.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public ActionResult Retry(string nonce, Guid invoiceKey)
        {
            // We have to get the paymentMethod from the CheckoutManager BEFORE processing the payment
            // so we have it for the factory since the checkout manager may be cleared on payment success
            // depending on CheckoutContextSettings.ResetCheckoutManagerOnPaymentSuccess configuration
            var paymentMethod = CheckoutManager.Payment.GetPaymentMethod();

            try
            {
                var invoice = MerchelloServices.InvoiceService.GetByKey(invoiceKey);
                var attempt = ProcessPayment(nonce, invoice);
                var model = CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod, attempt);

                return HandlePaymentSuccess(model);
            }
            catch (Exception ex)
            {
                var model = CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod);
                model.Token = nonce;

                return HandlePaymentException(model, ex);
            }
        }

        /// <summary>
        /// Performs the work of processing the payment with Braintree.
        /// </summary>
        /// <param name="nonce">
        /// The 'nonce' generated by Braintree that we use to bill against.
        /// </param>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected virtual IPaymentResult ProcessPayment(string nonce, IInvoice invoice = null)
        {
            // gets the payment method
            var paymentMethod = CheckoutManager.Payment.GetPaymentMethod();

            // You need a ProcessorArgumentCollection for this transaction to store the payment method nonce
            // The braintree package includes an extension method off of the ProcessorArgumentCollection - SetPaymentMethodNonce([nonce]);
            var args = new ProcessorArgumentCollection();
            args.SetPaymentMethodNonce(nonce);

            // We want this to be an AuthorizeCapture(paymentMethod.Key, args);
            return invoice == null
                       ? CheckoutManager.Payment.AuthorizeCapturePayment(paymentMethod.Key, args)
                       : invoice.AuthorizeCapturePayment(paymentMethod.Key, args);
        }

        /// <summary>
        /// Gets the total basket count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <remarks>
        /// This is generally used in navigations and labels.  Some implementations show the total number of line items while
        /// others show the total number of items (total sum of product quantities - default).
        /// 
        /// Method is used in Async responses to allow for easier HTML label updates 
        /// </remarks>
        protected virtual int GetBasketItemCountForDisplay()
        {
            return this.Basket.TotalQuantityCount;
        }
    }
}