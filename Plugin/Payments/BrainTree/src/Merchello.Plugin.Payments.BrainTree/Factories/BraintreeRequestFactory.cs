namespace Merchello.Plugin.Payments.Braintree.Factories
{
    using System;
    using System.Linq;

    using global::Braintree;

    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree.Models;

    /// <summary>
    /// The <see cref="BraintreeRequestFactory"/>.
    /// </summary>
    internal class BraintreeRequestFactory
    {
        /// <summary>
        /// Creates a <see cref="ClientTokenRequest"/>.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The <see cref="ClientTokenRequest"/>.
        /// </returns>
        public ClientTokenRequest CreateClientTokenRequest(Guid customerKey)
        {
            return customerKey == Guid.Empty ? 
                new ClientTokenRequest() : 
                new ClientTokenRequest()
                    {
                        CustomerId = customerKey.ToString() 
                    };
        }

        public CustomerRequest CreateCustomerRequest(ICustomer customer)
        {
            if (customer == null) throw new ArgumentNullException("customer");

            return new CustomerRequest()
                       {
                       };
        }

        public DescriptorRequest CreateDescriptorRequest(BraintreeProviderSettings settings)
        {
            return new DescriptorRequest()
                       {
                           
                       };
        }

        //public CreditCardRequest CreateCreditCardRequest()

        public TransactionRequest CreateTransactionRequest(IInvoice invoice, string paymentMethodNonce, Func<Guid, ICustomer> getCustomer)
        {
            var customer = invoice.CustomerKey != null ? getCustomer(invoice.CustomerKey.Value) : null;

            return new TransactionRequest()
                       {
                           Amount = invoice.Total,
                           OrderId = invoice.PrefixedInvoiceNumber(),
                           PaymentMethodNonce = paymentMethodNonce,
                       };
        }
    }

    
}