namespace Merchello.Core.Checkout
{
    using System;

    using Merchello.Core.Builders;
    using Merchello.Core.Events;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core.Events;

    /// <summary>
    /// A base class for CheckoutPaymentManagers.
    /// </summary>
    public abstract class CheckoutPaymentManagerBase : CheckoutCustomerDataManagerBase, ICheckoutPaymentManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutPaymentManagerBase(ICheckoutContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Occurs after an invoice has been prepared.
        /// </summary>
        public static event TypedEventHandler<CheckoutPaymentManagerBase, SalesPreparationEventArgs<IInvoice>> InvoicePrepared;

        /// <summary>
        /// Occurs after a sale has been finalized.
        /// </summary>
        public static event TypedEventHandler<CheckoutPaymentManagerBase, SalesPreparationEventArgs<IPaymentResult>> Finalizing;

        /// <summary>
        /// Gets or sets the invoice number prefix.
        /// </summary>
        public string InvoiceNumberPrefix { get; set; }

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates an <see cref="IInvoice"/> representing the bill for the current "checkout order"
        /// </summary>
        /// <param name="invoiceBuilder">The invoice builder class</param>
        /// <returns>An <see cref="IInvoice"/> that is not persisted to the database.</returns>
        public virtual IInvoice PrepareInvoice(IBuilderChain<IInvoice> invoiceBuilder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a <see cref="IPaymentMethod"/> to <see cref="ICustomerBase"/> extended data
        /// </summary>
        /// <param name="paymentMethod">
        /// The payment Method.
        /// </param>
        public abstract void SavePaymentMethod(IPaymentMethod paymentMethod);

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
    }
}