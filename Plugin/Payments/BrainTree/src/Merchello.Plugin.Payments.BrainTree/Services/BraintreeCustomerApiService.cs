namespace Merchello.Plugin.Payments.Braintree.Services
{
    using System;
    using global::Braintree;
    using global::Braintree.Exceptions;
    using Core;
    using Core.Models;
    using Exceptions;
    using Models;
    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

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

            var request = RequestFactory.CreateCustomerRequest(customer, paymentMethodNonce, billingAddress);

            Creating.RaiseEvent(new Core.Events.NewEventArgs<CustomerRequest>(request), this);

            var result = this.BraintreeGateway.Customer.Create(request);

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

            var request = RequestFactory.CreateCustomerRequest(customer, paymentMethodNonce, billingAddress, true);

            Updating.RaiseEvent(new SaveEventArgs<CustomerRequest>(request), this);

            var result = this.BraintreeGateway.Customer.Update(customer.Key.ToString(), request);

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
            if (!this.Exists(customer)) return false;
           
            this.BraintreeGateway.Customer.Delete(customer.Key.ToString());
            this.RuntimeCache.ClearCacheItem(this.MakeCustomerCacheKey(customer));

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
                var braintreeCustomer = this.RuntimeCache.GetCacheItem(this.MakeCustomerCacheKey(customer), () => this.BraintreeGateway.Customer.Find(customer.Key.ToString()));

                return braintreeCustomer != null;
            }
            catch (Exception)
            {
                RuntimeCache.ClearCacheItem(this.MakeCustomerCacheKey(customer));
                return false;
            }
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
            return BraintreeGateway.Customer.Search(query);
        }

        /// <summary>
        /// Performs a direct get all operation against the BrainTree API.
        /// </summary>
        /// <returns>
        /// The <see cref="ResourceCollection{Customer}"/>.
        /// </returns>
        public ResourceCollection<Customer> GetAll()
        {
            return BraintreeGateway.Customer.All();
        }
    }
}