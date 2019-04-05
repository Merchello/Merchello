namespace Merchello.Providers.Payment.Braintree.Services
{
    using System;

    using global::Braintree;
    using global::Braintree.Exceptions;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree.Models;
    using Merchello.Providers.Payment.Exceptions;
    using Merchello.Providers.Payment.Models;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    using Customer = global::Braintree.Customer;

    /// <summary>
    /// The braintree customer service.
    /// </summary>
    internal class BraintreeCustomerApiService : BraintreeApiServiceBase, IBraintreeCustomerApiService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeCustomerApiService(BraintreeProviderSettings settings)
            : this(Core.MerchelloContext.Current, settings)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeCustomerApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        internal BraintreeCustomerApiService(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
            : base(merchelloContext, settings)
        {
        }

        #region Events

        /// <summary>
        /// Occurs before the Create
        /// </summary>
        public static event TypedEventHandler<BraintreeCustomerApiService, Core.Events.NewEventArgs<CustomerRequest>> Creating;

        /// <summary>
        /// Occurs after Create
        /// </summary>
        public static event TypedEventHandler<BraintreeCustomerApiService, Core.Events.NewEventArgs<Customer>> Created;

        /// <summary>
        /// Occurs before Save
        /// </summary>
        public static event TypedEventHandler<BraintreeCustomerApiService, SaveEventArgs<CustomerRequest>> Updating;

        /// <summary>
        /// Occurs after Save
        /// </summary>
        public static event TypedEventHandler<BraintreeCustomerApiService, SaveEventArgs<Customer>> Updated;

        #endregion

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

            var request = this.RequestFactory.CreateCustomerRequest(customer, paymentMethodNonce, billingAddress);

            Creating.RaiseEvent(new Core.Events.NewEventArgs<CustomerRequest>(request), this);

            // attempt the API call
            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Create customer attempt for CustomerKey: {0}, name: {1}", customer.Key, customer.FullName));
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Customer.Create(request));

            if (!attempt.Success) return Attempt<Customer>.Fail(attempt.Exception);

            var result = attempt.Result;

            if (result.IsSuccess())
            {
                Created.RaiseEvent(new Core.Events.NewEventArgs<Customer>(result.Target), this);

                return Attempt.Succeed((Customer)this.RuntimeCache.GetCacheItem(this.MakeCustomerCacheKey(customer), () => result.Target));
            }

            var error = new BraintreeApiException(result.Errors);
            LogHelper.Error<BraintreeCustomerApiService>("Braintree API Customer Create return a failure", error);

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

            LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Update customer attempt for CustomerKey: {0}, name: {1}", customer.Key, customer.FullName));
            var request = this.RequestFactory.CreateCustomerRequest(customer, paymentMethodNonce, billingAddress, true);

            Updating.RaiseEvent(new SaveEventArgs<CustomerRequest>(request), this);

            // attempt the API call
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Customer.Update(customer.Key.ToString(), request));

            if (!attempt.Success) return Attempt<Customer>.Fail(attempt.Exception);

            var result = attempt.Result;

            if (result.IsSuccess())
            {
                var cacheKey = this.MakeCustomerCacheKey(customer);
                this.RuntimeCache.ClearCacheItem(cacheKey);

                Updated.RaiseEvent(new SaveEventArgs<Customer>(result.Target), this);

                return Attempt<Customer>.Succeed((Customer)this.RuntimeCache.GetCacheItem(cacheKey, () => result.Target));
            }

            var error = new BraintreeApiException(result.Errors);
            LogHelper.Error<BraintreeCustomerApiService>("Braintree API Customer Create return a failure", error);

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
            return !this.Exists(customer) || this.Delete(customer.Key.ToString());
        }

        /// <summary>
        /// Deletes the Braintree <see cref="Customer"/> by it's customer id.
        /// </summary>
        /// <param name="customerId">
        /// The customer id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Delete(string customerId)
        {
            try
            {
                LogHelper.Info<BraintreeTransactionApiService>(string.Format("Braintree Delete customer attempt for CustomerKey: {0}", customerId));
                this.BraintreeGateway.Customer.Delete(customerId);
                this.RuntimeCache.ClearCacheItem(this.MakeCustomerCacheKey(customerId));
            }
            catch (Exception ex)
            {
                LogHelper.Error<BraintreeCustomerApiService>("Braintree API customer delete request failed.", ex);
                return false;
            }
            

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
                var cacheKey = this.MakeCustomerCacheKey(customer);

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
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.ClientToken.generate(this.RequestFactory.CreateClientTokenRequest(Guid.Empty)));

            return attempt.Success ? attempt.Result : string.Empty;
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

            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.ClientToken.generate(this.RequestFactory.CreateClientTokenRequest(customer.Key)));

            return attempt.Success ? attempt.Result : string.Empty;
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
            var cacheKey = this.MakeCustomerCacheKey(customer);
        
            var braintreeCustomer = this.RuntimeCache.GetCacheItem(cacheKey);

            if (braintreeCustomer == null)
            {
                var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Customer.Find(customer.Key.ToString()));

                if (!attempt.Success)
                {
                    return false;
                }

                braintreeCustomer = attempt.Result;

                this.RuntimeCache.GetCacheItem(cacheKey, () => braintreeCustomer);
            }

            return braintreeCustomer != null;            
        }

        /// <summary>
        /// Performs a direct search query again the BrainTree API
        /// </summary>
        /// <param name="query">
        /// The <see cref="CustomerSearchRequest"/>
        /// </param>
        /// <returns>
        /// The <see cref="ResourceCollection{Customer}"/>.
        /// </returns>
        public ResourceCollection<Customer> Search(CustomerSearchRequest query)
        {
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Customer.Search(query));
            return attempt.Success ? attempt.Result : null;
        }

        /// <summary>
        /// Performs a direct get all operation against the BrainTree API.
        /// </summary>
        /// <returns>
        /// The <see cref="ResourceCollection{Customer}"/>.
        /// </returns>
        public ResourceCollection<Customer> GetAll()
        {
            var attempt = this.TryGetApiResult(() => this.BraintreeGateway.Customer.All());

            return attempt.Success ? attempt.Result : null;
        }
    }
}