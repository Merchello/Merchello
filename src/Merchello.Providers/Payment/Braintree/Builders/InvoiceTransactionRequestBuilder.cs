namespace Merchello.Providers.Payment.Braintree.Builders
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Models;

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

            this._invoice = invoice;
            this._paymentMethodNonce = paymentMethodNonce;

            this.Initialize();
        }


        /// <summary>
        /// Gets the api customer id.
        /// </summary>
        private string ApiCustomerId
        {
            get { return this._customer == null ? string.Empty : this._customer.Key.ToString(); }
        }

        /// <summary>
        /// Builds the <see cref="TransactionRequest"/> for the <see cref="IInvoice"/>
        /// </summary>
        /// <returns>
        /// The <see cref="TransactionRequest"/>.
        /// </returns>
        public override TransactionRequest Build()
        {
            this.BraintreeRequest.Amount = this._invoice.Total;
            this.BraintreeRequest.OrderId = this._invoice.PrefixedInvoiceNumber();
            this.BraintreeRequest.PaymentMethodNonce = this._paymentMethodNonce;

            return this.BraintreeRequest;
        }



        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            if (this._invoice.CustomerKey == null || Guid.Empty.Equals(this._invoice.CustomerKey.Value)) return;

            this._customer = this.MerchelloContext.Services.CustomerService.GetByKey(this._invoice.CustomerKey.Value);
        }
    }
}