namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Builders;
    using Merchello.Core.Events;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// A base class for CheckoutPaymentManagers.
    /// </summary>
    public abstract class CheckoutPaymentManagerBase : CheckoutCustomerDataManagerBase, ICheckoutPaymentManager
    {
        /// <summary>
        /// A function to instantiate an invoice BuilderChain.
        /// </summary>
        private IBuilderChain<IInvoice> _invoiceBuilder; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="invoiceBuilder">
        /// A lazy instantiate to get an invoice BuilderChain.
        /// </param>
        protected CheckoutPaymentManagerBase(ICheckoutContext context, IBuilderChain<IInvoice> invoiceBuilder)
            : base(context)
        {
            Mandate.ParameterNotNull(invoiceBuilder, "invoiceBuilder");
            this._invoiceBuilder = invoiceBuilder;

            this.Initialize();
        }

        /// <summary>
        /// Occurs after an invoice has been prepared.
        /// </summary>
        public static event TypedEventHandler<CheckoutPaymentManagerBase, CheckoutEventArgs<IInvoice>> InvoicePrepared;

        /// <summary>
        /// Occurs after a sale has been finalized.
        /// </summary>
        public static event TypedEventHandler<CheckoutPaymentManagerBase, CheckoutEventArgs<IPaymentResult>> Finalizing;

        /// <summary>
        /// Gets the <see cref="IBuilderChain{IInoice}"/>.
        /// </summary>
        protected IBuilderChain<IInvoice> InvoiceBuilder
        {
            get
            {
                return this._invoiceBuilder;
            }
        } 

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="ICheckoutPaymentManager"/> is ready to prepare an <see cref="IInvoice"/>
        /// </summary>
        /// <returns>
        /// True or false
        /// </returns>
        public virtual bool IsReadyToInvoice()
        {
            return Context.Customer.ExtendedData.GetAddress(AddressType.Billing) != null;
        }

        /// <summary>
        /// Generates an <see cref="IInvoice"/>
        /// </summary>
        /// <returns>An <see cref="IInvoice"/></returns>
        public virtual IInvoice PrepareInvoice()
        {
            return !IsReadyToInvoice() ? null : PrepareInvoice(InvoiceBuilder);
        }

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <param name="invoiceBuilder">The invoice builder class</param>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        public virtual IInvoice PrepareInvoice(IBuilderChain<IInvoice> invoiceBuilder)
        {
            if (!IsReadyToInvoice()) return null;

            ////var requestCache = _merchelloContext.Cache.RequestCache;
            ////var cacheKey = string.Format("merchello.salespreparationbase.prepareinvoice.{0}", ItemCache.VersionKey);
            var attempt = invoiceBuilder.Build();

            if (attempt.Success)
            {
                InvoicePrepared.RaiseEvent(new CheckoutEventArgs<IInvoice>(Context.Customer, attempt.Result), this);

                return attempt.Result;
            }

            LogHelper.Error<SalePreparationBase>("The invoice builder failed to generate an invoice.", attempt.Exception);

            throw attempt.Exception;
        }

        /// <summary>
        /// Removes a previously saved payment method..
        /// </summary>
        public abstract void ClearPaymentMethod();

        /// <summary>
        /// Saves a <see cref="IPaymentMethod"/> to <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <param name="paymentMethod">
        /// The payment Method.
        /// </param>
        public abstract void SavePaymentMethod(IPaymentMethod paymentMethod);

        /// <summary>
        /// Gets a list of all possible Payment Methods
        /// </summary>
        /// <returns>A collection of <see cref="IPaymentGatewayMethod"/>s</returns>
        public virtual IEnumerable<IPaymentGatewayMethod> GetPaymentGatewayMethods()
        {
            return Context.Gateways.Payment.GetPaymentGatewayMethods();
        }

        /// <summary>
        /// Gets a <see cref="IPaymentMethod"/> from <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <returns>
        /// The previously saved <see cref="IPaymentMethod"/>.
        /// </returns>
        public abstract IPaymentMethod GetPaymentMethod();

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public abstract IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentGatewayMethod"/> to use in processing the payment</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public abstract IPaymentResult AuthorizePayment(IPaymentGatewayMethod paymentGatewayMethod);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public abstract IPaymentResult AuthorizePayment(Guid paymentMethodKey, ProcessorArgumentCollection args);

        /// <summary>
        /// Attempts to process a payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>The <see cref="IPaymentResult"/></returns>
        public abstract IPaymentResult AuthorizePayment(Guid paymentMethodKey);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public abstract IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod, ProcessorArgumentCollection args);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentGatewayMethod">The <see cref="IPaymentMethod"/></param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public abstract IPaymentResult AuthorizeCapturePayment(IPaymentGatewayMethod paymentGatewayMethod);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <param name="args">Additional arguments required by the payment processor</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public abstract IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey, ProcessorArgumentCollection args);

        /// <summary>
        /// Authorizes and Captures a Payment
        /// </summary>
        /// <param name="paymentMethodKey">The <see cref="IPaymentMethod"/> key</param>
        /// <returns>A <see cref="IPaymentResult"/></returns>
        public abstract IPaymentResult AuthorizeCapturePayment(Guid paymentMethodKey);

        /// <summary>
        /// Raises the Finalizing event.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        protected void OnFinalizing(IPaymentResult result)
        {
            if (Finalizing != null)
            {
                Finalizing.RaiseEvent(new CheckoutEventArgs<IPaymentResult>(Context.Customer, result), this);
            }
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {
            if (Context.IsNewVersion && Context.ChangeSettings.ResetPaymentManagerDataOnVersionChange)
            {
                this.ClearPaymentMethod();
            }
        }
    }
}