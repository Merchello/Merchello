namespace Merchello.Plugin.Payments.Braintree.Api
{
    using System;

    using global::Braintree;
    using global::Braintree.Exceptions;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Plugin.Payments.Braintree.Exceptions;
    using Merchello.Plugin.Payments.Braintree.Models;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The braintree customer service.
    /// </summary>
    internal class BraintreeCustomerApiProvider : BraintreeApiProviderBase, IBraintreeCustomerApiProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerApiProvider"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeCustomerApiProvider(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerApiProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        internal BraintreeCustomerApiProvider(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : base(merchelloContext, settings)
        {
        }

        /// <summary>
        /// Creates a Braintree <see cref="Customer"/> from a Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The "nonce-from-the-client"
        /// </param>
        /// <param name="billingAddress">
        /// The billing address
        /// </param>
        /// <param name="shippingAddress">
        /// The shipping Address.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{Customer}"/>.
        /// </returns>
        public Attempt<Customer> Create(ICustomer customer, string paymentMethodNonce = "", IAddress billingAddress = null, IAddress shippingAddress = null)
        {
            if (this.Exists(customer)) return Attempt.Succeed(this.GetBraintreeCustomer(customer));

           var request = RequestFactory.CreateCustomerRequest(customer, paymentMethodNonce, billingAddress);

            var result = this.BraintreeGateway.Customer.Create(request);

            if (result.IsSuccess())
            {
                return Attempt.Succeed((Customer)this.RuntimeCache.GetCacheItem(this.MakeCacheKey(customer), () => result.Target));
            }

            var error = new BraintreeApiException(result.Errors);
            LogHelper.Error<BraintreeCustomerApiProvider>("Braintree API Customer Create return a failure", error);

            return Attempt<Customer>.Fail(error);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">The "nonce-from-the-client"</param>
        /// <param name="billingAddress">The customer billing address</param>
        /// <param name="shippinggAddress">The shipping address</param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        public Attempt<Customer> Update(ICustomer customer, string paymentMethodNonce = "", IAddress billingAddress = null, IAddress shippinggAddress = null)
        {
            if (!this.Exists(customer)) return Attempt<Customer>.Fail(new NullReferenceException("Could not finde matching Braintree customer."));

            var request = RequestFactory.CreateCustomerRequest(customer, paymentMethodNonce, billingAddress, true);

            var result = this.BraintreeGateway.Customer.Update(customer.Key.ToString(), request);

            if (result.IsSuccess())
            {
                var cacheKey = this.MakeCacheKey(customer);
                this.RuntimeCache.ClearCacheItem(cacheKey);

                return Attempt<Customer>.Succeed((Customer)this.RuntimeCache.GetCacheItem(cacheKey, () => result.Target));
            }

            var error = new BraintreeApiException(result.Errors);
            LogHelper.Error<BraintreeCustomerApiProvider>("Braintree API Customer Create return a failure", error);

            return Attempt<Customer>.Fail(error);
        }


        /// <summary>
        /// Deletes the Braintree <see cref="Customer"/> corresponding with the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Delete(ICustomer customer)
        {
            if (!this.Exists(customer)) return false;
           
            this.BraintreeGateway.Customer.Delete(customer.Key.ToString());
            this.RuntimeCache.ClearCacheItem(this.MakeCacheKey(customer));

            return true;
        }

        /// <summary>
        /// Gets the Braintree <see cref="Customer"/> corresponding to the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="createOnNotFound">True or false indicating whether or not the customer should be automatically created if not found</param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        public Customer GetBraintreeCustomer(Guid customerKey, bool createOnNotFound = true)
        {
            var customer = this.MerchelloContext.Services.CustomerService.GetByKey(customerKey);

            return this.GetBraintreeCustomer(customer, createOnNotFound);
        }

        /// <summary>
        /// Gets the Braintree <see cref="Customer"/> corresponding to the Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="createOnNotFound">
        /// True or false indicating whether or not the customer should be automatically created if not found
        /// </param>
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        public Customer GetBraintreeCustomer(ICustomer customer, bool createOnNotFound = true)
        {
            Mandate.ParameterNotNull(customer, "customer");

            if (this.Exists(customer))
            {
                var cacheKey = this.MakeCacheKey(customer);

                return (Customer)this.RuntimeCache.GetCacheItem(cacheKey, () => this.BraintreeGateway.Customer.Find(customer.Key.ToString()));
            }

            if (!createOnNotFound) return null;

            var attempt = this.Create(customer);

            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// The generate client request token.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GenerateClientRequestToken()
        {
            return this.BraintreeGateway.ClientToken.generate(RequestFactory.CreateClientTokenRequest(Guid.Empty));
        }

        /// <summary>
        /// The generate client request token.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="BraintreeException">
        /// Throws an exception if the braintree customer returns null
        /// </exception>
        public string GenerateClientRequestToken(ICustomer customer)
        {
            var braintreeCustomer = this.GetBraintreeCustomer(customer);

            if (braintreeCustomer == null) throw new BraintreeException("Failed to retrieve and/or create a Braintree Customer");

            return this.BraintreeGateway.ClientToken.generate(RequestFactory.CreateClientTokenRequest(customer.Key));
        }

        /// <summary>
        /// Adds a credit card to an existing customer.
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddCreditCardToCustomer(ICustomer customer, string paymentMethodNonce, IAddress billingAddress = null)
        {
            return AddCreditCardToCustomer(customer, paymentMethodNonce, string.Empty, billingAddress);
        }

        /// <summary>
        /// Adds a credit card to an existing customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">
        /// The payment method nonce.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="billingAddress">
        /// The billing address.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddCreditCardToCustomer(ICustomer customer, string paymentMethodNonce, string token, IAddress billingAddress = null)
        {
            var request = RequestFactory.CreatePaymentMethodRequest(customer, paymentMethodNonce);
            
            if (!string.IsNullOrEmpty(token)) request.Token = token;

            if (billingAddress != null) request.BillingAddress = RequestFactory.CreatePaymentMethodAddressRequest(billingAddress);

            var result = BraintreeGateway.PaymentMethod.Create(request);

            if (result.IsSuccess()) return true;

            var error = new BraintreeApiException(result.Errors);

            LogHelper.Error<BraintreeCustomerApiProvider>("Failed to add a credit card to a customer", error);

            return false;
        }

        /// <summary>
        /// Returns true or false indicating whether the customer exists in Braintree
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Exists(ICustomer customer)
        {
            try
            {
                var braintreeCustomer = this.RuntimeCache.GetCacheItem(this.MakeCacheKey(customer), () => this.BraintreeGateway.Customer.Find(customer.Key.ToString()));

                return braintreeCustomer != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Makes a cache key.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string MakeCacheKey(ICustomer customer)
        {
            return Caching.CacheKeys.BraintreeCustomer(customer.Key);
        }
    }
}