namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;

    using global::Braintree;

    using Merchello.Core.Models;

    /// <summary>
    /// The <see cref="CustomerRequestFactory"/>.
    /// </summary>
    internal class CustomerRequestFactory
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

        /// <summary>
        /// Creates a simple <see cref="CustomerRequest"/>.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerRequest"/>.
        /// </returns>
        public CustomerRequest CreateCustomerRequest(ICustomer customer)
        {
            Mandate.ParameterNotNull(customer, "customer");

            return new CustomerRequest()
                       {
                           CustomerId = customer.Key.ToString(),
                           FirstName = customer.FirstName,
                           LastName = customer.LastName,
                           Email = customer.Email
                       };
        }

        /// <summary>
        /// Creates a <see cref="CustomerRequest"/> with a payment method nonce.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerRequest"/>.
        /// </returns>
        public CustomerRequest CreateCustomerRequest(ICustomer customer, string paymentMethodNonce)
        {
            var request = CreateCustomerRequest(customer);
            if (!string.IsNullOrEmpty(paymentMethodNonce)) request.PaymentMethodNonce = paymentMethodNonce;
            return request;
        }

        /// <summary>
        /// The create customer request.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">The "nonce-from-the-client"</param>
        /// <param name="billingAddress">The customer's billing address</param>
        /// <param name="isUpdate">A value indicating whether or not this is an Update request</param>
        /// <returns>
        /// The <see cref="CustomerRequest"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception is the customer is null
        /// </exception>
        public CustomerRequest CreateCustomerRequest(ICustomer customer, string paymentMethodNonce, IAddress billingAddress, bool isUpdate = false)
        {
            var request = CreateCustomerRequest(customer);

            if (!string.IsNullOrEmpty(paymentMethodNonce)) request.CreditCard = CreateCreditCardRequest(paymentMethodNonce, billingAddress, isUpdate);

            return request;
        }

        /// <summary>
        /// Creates a new <see cref="CreditCardRequest"/>
        /// </summary>
        /// <param name="paymentMethodNonce">
        /// The "nonce-from-the-client"
        /// </param>
        /// <param name="billingAddress">The billing address associated with the credit card</param>
        /// <param name="isUpdate">A value indicating whether or not this is an Update request</param>
        /// <returns>
        /// The <see cref="CreditCardRequest"/>.
        /// </returns>
        /// <remarks>
        /// Uses VerifyCard = true as a default option
        /// </remarks>
        public CreditCardRequest CreateCreditCardRequest(string paymentMethodNonce, IAddress billingAddress = null, bool isUpdate = false)
        {
            return CreateCreditCardRequest(paymentMethodNonce, new CreditCardOptionsRequest() { VerifyCard = true }, billingAddress, isUpdate);
        }

        /// <summary>
        /// Creates a new <see cref="CreditCardRequest"/>
        /// </summary>
        /// <param name="paymentMethodNonce">
        /// The "nonce-from-the-client"
        /// </param>
        /// <param name="optionsRequest">
        /// The options request.
        /// </param>
        /// <param name="billingAddress">The billing address associated with the credit card</param>
        /// <param name="isUpdate">A value indicating whether or not this is an update request</param>
        /// <returns>
        /// The <see cref="CreditCardRequest"/>.
        /// </returns>
        public CreditCardRequest CreateCreditCardRequest(string paymentMethodNonce, CreditCardOptionsRequest optionsRequest, IAddress billingAddress = null, bool isUpdate = false)
        {
            Mandate.ParameterNotNullOrEmpty(paymentMethodNonce, "paymentMethodNonce");

            var request = new CreditCardRequest()
                              {
                                  PaymentMethodNonce = paymentMethodNonce
                              };
            if (optionsRequest != null) request.Options = optionsRequest;
            if (billingAddress != null) request.BillingAddress = this.CreateCreditCardAddressRequest(billingAddress, isUpdate);

            return request;
        }

        /// <summary>
        /// Creates a <see cref="CreditCardRequest"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="isUpdate">
        /// The is update.
        /// </param>
        /// <returns>
        /// The <see cref="CreditCardAddressRequest"/>.
        /// </returns>
        public CreditCardAddressRequest CreateCreditCardAddressRequest(IAddress address, bool isUpdate = false)
        {
            return new CreditCardAddressRequest()
                       {
                           FirstName = address.TrySplitFirstName(),
                           LastName = address.TrySplitLastName(),
                           Company = address.Organization,
                           StreetAddress = address.Address1,
                           ExtendedAddress = address.Address2,
                           Locality = address.Locality,
                           Region = address.Region,
                           PostalCode = address.PostalCode,
                           CountryCodeAlpha2 = address.CountryCode,
                           Options = new CreditCardAddressOptionsRequest() { UpdateExisting = isUpdate }
                       };
        }
    }    
}