namespace Merchello.Providers.Payment.Braintree.Services
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core;

    using Constants = Merchello.Providers.Constants;

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

            this._settings = settings;
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
        /// <param name="merchantAccountId">
        /// The merchant account Id
        /// </param>
        /// <returns>
        /// The <see cref="ClientTokenRequest"/>.
        /// </returns>
        public ClientTokenRequest CreateClientTokenRequest(Guid customerKey, string merchantAccountId)
        {
            var ctr = new ClientTokenRequest();

            if (customerKey != Guid.Empty)
            {
                ctr.CustomerId = customerKey.ToString();
            }

            if (!string.IsNullOrWhiteSpace(merchantAccountId))
            {
                ctr.MerchantAccountId = merchantAccountId;
            }

            return ctr;
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
            return this.CreateCreditCardRequest(paymentMethodNonce, new CreditCardOptionsRequest { VerifyCard = true }, billingAddress, isUpdate);
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
            return new CreditCardAddressRequest
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
                Options = new CreditCardAddressOptionsRequest { UpdateExisting = isUpdate }
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

            return new CustomerRequest
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

            var request = new PaymentMethodRequest
                              {
                                  CustomerId = customer.Key.ToString(),
                                  PaymentMethodNonce = paymentMethodNonce
                              };
            if (isDefault)
            request.Options = new PaymentMethodOptionsRequest
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
            var addressRequest = this.CreatePaymentMethodAddressRequest(billingAddress);

            if (updateExisting) addressRequest.Options = new PaymentMethodAddressOptionsRequest { UpdateExisting = true };

            return new PaymentMethodRequest
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
            return new PaymentMethodAddressRequest
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
        /// <param name="merchantAccountId"></param>
        /// <returns>
        /// The <see cref="SubscriptionRequest"/>.
        /// </returns>
        public SubscriptionRequest CreateSubscriptionRequest(string paymentMethodToken, string planId, decimal? price = null, string merchantAccountId = "")
        {
            Mandate.ParameterNotNullOrEmpty(paymentMethodToken, "paymentMethodToken");
            Mandate.ParameterNotNullOrEmpty(planId, "planId");

            var request = new SubscriptionRequest
                       {
                           PaymentMethodToken = paymentMethodToken, 
                           PlanId = planId
                       };

            if (!string.IsNullOrEmpty(merchantAccountId))
            {
                request.MerchantAccountId = merchantAccountId;
            }

            // TODO figure out the descriptor for nicer Credit Card statements
            // TODO https://www.braintreepayments.com/docs/dotnet/transactions/dynamic_descriptors
            //if (_settings.MerchantDescriptor.HasValues())
            //{
            //    request.Descriptor = _settings.MerchantDescriptor.AsDescriptorRequest();
            //}

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
        /// <param name="merchantAccountId"></param>
        /// <returns>
        /// The <see cref="SubscriptionRequest"/>.
        /// </returns>
        public SubscriptionRequest CreateSubscriptionRequest(string paymentMethodToken, string planId, int trialDuration, SubscriptionDurationUnit trialDurationUnit, bool addTrialPeriod = false, string merchantAccountId = "")
        {
            if (trialDurationUnit == null) trialDurationUnit = SubscriptionDurationUnit.MONTH;

            var request = this.CreateSubscriptionRequest(paymentMethodToken, planId, null, merchantAccountId);

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
            var request = this.CreateSubscriptionRequest(paymentMethodToken, planId);

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

            var request = this.CreateSubscriptionRequest(paymentMethodToken, planId);
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
        /// <param name="amount">
        /// The amount of the transaction
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
        /// <param name="merchantAccountId">
        /// Optional Merchant Id
        /// </param>
        /// <returns>
        /// The <see cref="TransactionRequest"/>.
        /// </returns>
        public TransactionRequest CreateTransactionRequest(IInvoice invoice, decimal amount, string paymentMethodNonce, ICustomer customer = null, TransactionOption transactionOption = TransactionOption.Authorize, string merchantAccountId = "")
        {
            var request = new TransactionRequest
                       {
                           Amount = amount,
                           OrderId = invoice.PrefixedInvoiceNumber(),
                           PaymentMethodNonce = paymentMethodNonce,
                           BillingAddress = this.CreateAddressRequest(invoice.GetBillingAddress()),
                           Channel = Constants.Braintree.TransactionChannel
                       };

            // Optional merchantAccountId
            if (!string.IsNullOrEmpty(merchantAccountId))
            {
                request.MerchantAccountId = merchantAccountId;
            }

            if (customer != null) request.Customer = this.CreateCustomerRequest(customer);
            
            if (transactionOption == TransactionOption.SubmitForSettlement)
            {
                request.Options = new TransactionOptionsRequest { SubmitForSettlement = true };
            }

            return request;
        }

        /// <summary>
        /// The create vault transaction request.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="amount">
        /// The amount to take
        /// </param>
        /// <param name="paymentMethodToken">
        /// The payment method token.
        /// </param>
        /// <param name="transactionOption">
        /// The transaction option.
        /// </param>
        /// <param name="merchantAccountId"></param>
        /// <returns>
        /// The <see cref="TransactionRequest"/>.
        /// </returns>
        public TransactionRequest CreateVaultTransactionRequest(IInvoice invoice, decimal amount, string paymentMethodToken, TransactionOption transactionOption = TransactionOption.SubmitForSettlement, string merchantAccountId = "")
        {
            var request = new TransactionRequest
            {
                Amount = amount,
                OrderId = invoice.PrefixedInvoiceNumber(),
                PaymentMethodToken = paymentMethodToken,
                BillingAddress = this.CreateAddressRequest(invoice.GetBillingAddress()),
                Channel = Constants.Braintree.TransactionChannel
            };

            // Optional merchantAccountId
            if (!string.IsNullOrEmpty(merchantAccountId))
            {
                request.MerchantAccountId = merchantAccountId;
            }

            if (transactionOption == TransactionOption.SubmitForSettlement)
            {
                request.Options = new TransactionOptionsRequest { SubmitForSettlement = true };
            }

            return request;
        }

        #endregion
    }    
}