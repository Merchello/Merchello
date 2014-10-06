namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using global::Braintree;
    using Core.Models;
    using Models;

    /// <summary>
    /// The <see cref="BraintreeApiRequestFactory"/>.
    /// </summary>
    internal class BraintreeApiRequestFactory
    {
        /// <summary>
        /// The <see cref="BraintreeProviderSettings"/>.
        /// </summary>
        private readonly BraintreeProviderSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeApiRequestFactory"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeApiRequestFactory(BraintreeProviderSettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");

            _settings = settings;
        }

        #region Address

        /// <summary>
        /// Creates an <see cref="AddressRequest"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="AddressRequest"/>.
        /// </returns>
        public AddressRequest CreateAddressRequest(IAddress address)
        {
            return new AddressRequest()
            {
                FirstName = address.TrySplitFirstName(),
                LastName = address.TrySplitLastName(),
                Company = address.Organization,
                StreetAddress = address.Address1,
                ExtendedAddress = address.Address2,
                Locality = address.Locality,
                Region = address.Region,
                PostalCode = address.PostalCode,
                CountryCodeAlpha2 = address.CountryCode
            };
        }

        #endregion

        #region Client Token

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

        #endregion

        #region Credit Card

        /// <summary>
        /// Creates a new <see cref="CreditCardRequest"/>
        /// </summary>
        /// <param name="paymentMethodNonce">
        /// The "nonce-from-the-client"
        /// </param>
        /// <param name="billingAddress">
        /// The billing address associated with the credit card
        /// </param>
        /// <param name="shippingAddress">
        /// The shipping Address.
        /// </param>
        /// <param name="isUpdate">
        /// A value indicating whether or not this is an Update request
        /// </param>
        /// <returns>
        /// The <see cref="CreditCardRequest"/>.
        /// </returns>
        /// <remarks>
        /// Uses VerifyCard = true as a default option
        /// </remarks>
        public CreditCardRequest CreateCreditCardRequest(string paymentMethodNonce, IAddress billingAddress = null, IAddress shippingAddress = null, bool isUpdate = false)
        {
            return this.CreateCreditCardRequest(paymentMethodNonce, new CreditCardOptionsRequest() { VerifyCard = true }, billingAddress, isUpdate);
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

        #endregion

        #region Customer Request

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
                           Id = customer.Key.ToString(),
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
            var request = this.CreateCustomerRequest(customer);
            if (!string.IsNullOrEmpty(paymentMethodNonce)) request.PaymentMethodNonce = paymentMethodNonce;
            return request;
        }

        /// <summary>
        /// Creates a <see cref="CustomerRequest"/>.
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
            var request = this.CreateCustomerRequest(customer);

            if (!string.IsNullOrEmpty(paymentMethodNonce)) request.CreditCard = this.CreateCreditCardRequest(paymentMethodNonce, billingAddress, null, isUpdate);

            return request;
        }

        /// <summary>
        /// The create customer request.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="shippingAddress">
        /// The shipping address.
        /// </param>
        /// <param name="isUpdate">
        /// The is update.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerRequest"/>.
        /// </returns>
        public CustomerRequest CreateCustomerRequest(ICustomer customer, string paymentMethodNonce, IAddress billingAddress, IAddress shippingAddress, bool isUpdate = false)
        {
            var request = this.CreateCustomerRequest(customer);

            if (!string.IsNullOrEmpty(paymentMethodNonce)) request.CreditCard = this.CreateCreditCardRequest(paymentMethodNonce, billingAddress, shippingAddress, isUpdate);

            return request;
        }

        #endregion

        #region Payment Method

        /// <summary>
        /// Creates a <see cref="PaymentMethodRequest"/>.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="isDefault">
        /// The is default.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentMethodRequest"/>.
        /// </returns>
        public PaymentMethodRequest CreatePaymentMethodRequest(ICustomer customer, string paymentMethodNonce, bool isDefault = true)
        {
            Mandate.ParameterNotNullOrEmpty(paymentMethodNonce, "paymentMethodNonce");

            var request = new PaymentMethodRequest()
                              {
                                  CustomerId = customer.Key.ToString(),
                                  PaymentMethodNonce = paymentMethodNonce
                              };
            if (isDefault)
            request.Options = new PaymentMethodOptionsRequest()
                                  {
                                      MakeDefault = true
                                  };

            return request;
        }

        /// <summary>
        /// Creates a <see cref="PaymentMethodRequest"/> for an update.
        /// </summary>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <param name="updateExisting">
        /// The update existing.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentMethodRequest"/>.
        /// </returns>
        public PaymentMethodRequest CreatePaymentMethodRequest(IAddress billingAddress, bool updateExisting = true)
        {
            var addressRequest = CreatePaymentMethodAddressRequest(billingAddress);

            if (updateExisting) addressRequest.Options = new PaymentMethodAddressOptionsRequest() { UpdateExisting = true };

            return new PaymentMethodRequest()
                    {
                        BillingAddress = addressRequest                    
                    };
        }

        /// <summary>
        /// Creates a <see cref="PaymentMethodAddressRequest"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="PaymentMethodAddressRequest"/>.
        /// </returns>
        public PaymentMethodAddressRequest CreatePaymentMethodAddressRequest(IAddress address)
        {
            return new PaymentMethodAddressRequest()
                       {
                           FirstName = address.TrySplitFirstName(),
                           LastName = address.TrySplitLastName(),
                           Company = address.Organization,
                           StreetAddress = address.Address1,
                           ExtendedAddress = address.Address2,
                           Locality = address.Locality,
                           Region = address.Region,
                           PostalCode = address.PostalCode,
                           CountryCodeAlpha2 = address.CountryCode
                       };
        }

        #endregion

        #region Subscription

        /// <summary>
        /// Creates a <see cref="SubscriptionRequest"/>.
        /// </summary>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="planId">
        /// The plan id.
        /// </param>
        /// <param name="price">
        /// An optional price used to override the plan price.
        /// </param>
        /// <returns>
        /// The <see cref="SubscriptionRequest"/>.
        /// </returns>
        public SubscriptionRequest CreateSubscriptionRequest(string paymentMethodToken, string planId, decimal? price = null)
        {
            Mandate.ParameterNotNullOrEmpty(paymentMethodToken, "paymentMethodToken");
            Mandate.ParameterNotNullOrEmpty(planId, "planId");

            var request = new SubscriptionRequest()
                       {
                           PaymentMethodToken = paymentMethodToken, 
                           PlanId = planId,
                       };

            if (_settings.MerchantDescriptor.HasValues()) request.Descriptor = _settings.MerchantDescriptor.AsDescriptorRequest();

            if (price != null) request.Price = price.Value;

            return request;
        }

        /// <summary>
        /// Creates a <see cref="SubscriptionRequest"/>.
        /// </summary>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="planId">
        /// The plan id.
        /// </param>
        /// <param name="trialDuration">
        /// The trial duration.
        /// </param>
        /// <param name="trialDurationUnit">
        /// The trial duration unit.
        /// </param>
        /// <param name="addTrialPeriod">
        /// Adds a trial period to a plan that normally does not have one.
        /// </param>
        /// <returns>
        /// The <see cref="SubscriptionRequest"/>.
        /// </returns>
        public SubscriptionRequest CreateSubscriptionRequest(string paymentMethodToken, string planId, int trialDuration, SubscriptionDurationUnit trialDurationUnit, bool addTrialPeriod = false)
        {
            if (trialDurationUnit == null) trialDurationUnit = SubscriptionDurationUnit.MONTH;

            var request = CreateSubscriptionRequest(paymentMethodToken, planId);

            if (request.TrialDuration > 0) request.TrialDuration = trialDuration;

            request.TrialDurationUnit = trialDurationUnit;

            if (addTrialPeriod) request.HasTrialPeriod = true;

            return request;
        }

        /// <summary>
        /// Creates a <see cref="SubscriptionRequest"/>.
        /// </summary>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="planId">
        /// The plan id.
        /// </param>
        /// <param name="firstBillingDate">
        /// The first billing date.
        /// </param>
        /// <returns>
        /// The <see cref="SubscriptionRequest"/>.
        /// </returns>
        public SubscriptionRequest CreateSubscriptionRequest(string paymentMethodToken, string planId, DateTime firstBillingDate)
        {
            var request = CreateSubscriptionRequest(paymentMethodToken, planId);

            request.FirstBillingDate = firstBillingDate;

            return request;
        }

        /// <summary>
        /// The create subscription request.
        /// </summary>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="planId">
        /// The plan id.
        /// </param>
        /// <param name="billingDayOfMonth">
        /// The billing day of month.
        /// </param>
        /// <returns>
        /// The <see cref="SubscriptionRequest"/>.
        /// </returns>
        public SubscriptionRequest CreateSubscriptionRequest(string paymentMethodToken, string planId, int billingDayOfMonth)
        {
            Mandate.ParameterCondition(0 < billingDayOfMonth && 31 <= billingDayOfMonth, "billingDayOfMonth");

            var request = CreateSubscriptionRequest(paymentMethodToken, planId);
            request.BillingDayOfMonth = billingDayOfMonth;

            return request;
        }

        #endregion

        #region Transaction Request

        /// <summary>
        /// Creates a <see cref="TransactionRequest"/>.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment Method Nonce.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="transactionOption">
        /// The transaction Option.
        /// </param>
        /// <returns>
        /// The <see cref="TransactionRequest"/>.
        /// </returns>
        public TransactionRequest CreateTransactionRequest(IInvoice invoice, string paymentMethodNonce, ICustomer customer = null, TransactionOption transactionOption = TransactionOption.Authorize)
        {
            var request = new TransactionRequest()
                       {
                           Amount = invoice.Total,
                           OrderId = invoice.PrefixedInvoiceNumber(),
                           PaymentMethodNonce = paymentMethodNonce,
                           BillingAddress = CreateAddressRequest(invoice.GetBillingAddress()),
                           Channel = Constants.TransactionChannel
                       };

            if (customer != null) request.Customer = CreateCustomerRequest(customer);
            
            if (transactionOption == TransactionOption.SubmitForSettlement)
            {
                request.Options = new TransactionOptionsRequest() { SubmitForSettlement = true };
            }

            return request;
        }

        #endregion
    }    
}