namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using global::Braintree;
    using Core;
    using Core.Models;

    using global::Braintree.Exceptions;

    using Merchello.Plugin.Payments.Braintree.Exceptions;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The braintree customer service.
    /// </summary>
    internal class BraintreeCustomerService : BraintreeServiceBase, IBraintreeCustomerService
    {
        /// <summary>
        /// The <see cref="CustomerRequestFactory"/>.
        /// </summary>
        private readonly Lazy<CustomerRequestFactory> _requestFactory = new Lazy<CustomerRequestFactory>(() => new CustomerRequestFactory());  

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerService"/> class.
        /// </summary>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        public BraintreeCustomerService(BraintreeGateway braintreeGateway)
            : this(Core.MerchelloContext.Current, braintreeGateway)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="braintreeGateway">
        /// The braintree gateway.
        /// </param>
        internal BraintreeCustomerService(IMerchelloContext merchelloContext, BraintreeGateway braintreeGateway)
            : base(merchelloContext, braintreeGateway)
        {
        }

        /// <summary>
        /// Creates a Braintree <see cref="Customer"/> from a Merchello <see cref="ICustomer"/>
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethodNonce">The "nonce-from-the-client"</param>
        /// <param name="billingAddress">The billing address</param>
        /// <returns>
        /// The <see cref="Attempt{Customer}"/>.
        /// </returns>
        public Attempt<Customer> Create(ICustomer customer, string paymentMethodNonce = "", IAddress billingAddress = null)
        {
            if (Exists(customer)) return Attempt.Succeed(GetBraintreeCustomer(customer));

           var request = _requestFactory.Value.CreateCustomerRequest(customer, paymentMethodNonce, billingAddress);

            var result = BraintreeGateway.Customer.Create(request);

            if (result.IsSuccess())
            {
                return Attempt.Succeed((Customer)RuntimeCache.GetCacheItem(Caching.CacheKeys.BraintreeCustomer(customer.Key), () => result.Target));
            }

            var error = new BraintreeApiException(result.Errors);
            LogHelper.Error<BraintreeCustomerService>("Braintree API Customer Create return a failure", error);

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
        /// <returns>
        /// The <see cref="Customer"/>.
        /// </returns>
        public Attempt<Customer> Update(ICustomer customer, string paymentMethodNonce = "", IAddress billingAddress = null)
        {
            throw new NotImplementedException();
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
           
            BraintreeGateway.Customer.Delete(customer.Key.ToString());
            RuntimeCache.ClearCacheItem(Caching.CacheKeys.BraintreeCustomer(customer.Key));

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
            var customer = MerchelloContext.Services.CustomerService.GetByKey(customerKey);

            return GetBraintreeCustomer(customer, createOnNotFound);
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

            if (Exists(customer))
            {
                return (Customer)RuntimeCache.GetCacheItem(Caching.CacheKeys.BraintreeCustomer(customer.Key), () => BraintreeGateway.Customer.Find(customer.Key.ToString()));
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
            return BraintreeGateway.ClientToken.generate(_requestFactory.Value.CreateClientTokenRequest(Guid.Empty));
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
        /// </exception>
        public string GenerateClientRequestToken(ICustomer customer)
        {
            var braintreeCustomer = GetBraintreeCustomer(customer);

            if (braintreeCustomer == null) throw new BraintreeException("Failed to retrieve and/or create a Braintree Customer");

            return BraintreeGateway.ClientToken.generate(_requestFactory.Value.CreateClientTokenRequest(customer.Key));
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
                var braintreeCustomer = RuntimeCache.GetCacheItem(Caching.CacheKeys.BraintreeCustomer(customer.Key), () => BraintreeGateway.Customer.Find(customer.Key.ToString()));

                return braintreeCustomer != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}