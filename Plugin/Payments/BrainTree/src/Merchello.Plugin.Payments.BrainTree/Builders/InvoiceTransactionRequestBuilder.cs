namespace Merchello.Plugin.Payments.Braintree.Builders
{
    using System;
    using global::Braintree;
    using Core;
    using Core.Models;
    using Models;

    /// <summary>
    /// The invoice <see cref="TransactionRequest"/> builder.
    /// </summary>
    public class InvoiceTransactionRequestBuilder : RequestBuilderBase<TransactionRequest>
    {
        /// <summary>
        /// The _invoice.
        /// </summary>
        private readonly IInvoice _invoice;

        /// <summary>
        /// The "nonce-from-the-client"
        /// </summary>
        private readonly string _paymentMethodNonce;

        /// <summary>
        /// The Customer.
        /// </summary>
        private ICustomer _customer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceTransactionRequestBuilder"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="paymentMethodNonce">The "nonce" from the client</param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the invoice is null
        /// </exception>
        protected InvoiceTransactionRequestBuilder(IMerchelloContext merchelloContext, BraintreeProviderSettings settings, IInvoice invoice, string paymentMethodNonce) 
            : base(merchelloContext, settings)
        {
            if (invoice == null) throw new ArgumentNullException("invoice");
            if (string.IsNullOrEmpty(paymentMethodNonce)) throw new ArgumentNullException("paymentMethodNonce");

            _invoice = invoice;
            _paymentMethodNonce = paymentMethodNonce;

            Initialize();
        }


        /// <summary>
        /// Gets the api customer id.
        /// </summary>
        private string ApiCustomerId
        {
            get { return _customer == null ? string.Empty : _customer.Key.ToString(); }
        }

        /// <summary>
        /// Builds the <see cref="TransactionRequest"/> for the <see cref="IInvoice"/>
        /// </summary>
        /// <returns>
        /// The <see cref="TransactionRequest"/>.
        /// </returns>
        public override TransactionRequest Build()
        {
            BraintreeRequest.Amount = _invoice.Total;
            BraintreeRequest.OrderId = _invoice.PrefixedInvoiceNumber();
            BraintreeRequest.PaymentMethodNonce = _paymentMethodNonce;

            return BraintreeRequest;
        }



        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            if (_invoice.CustomerKey == null || Guid.Empty.Equals(_invoice.CustomerKey.Value)) return;

            _customer = MerchelloContext.Services.CustomerService.GetByKey(_invoice.CustomerKey.Value);
        }
    }
}