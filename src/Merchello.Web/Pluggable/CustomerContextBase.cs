namespace Merchello.Web.Pluggable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using Merchello.Core;
    using Merchello.Core.Cache;
    using Merchello.Core.Configuration;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Customer;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web;

    /// <summary>
    /// A base class for defining customer contexts for various membership providers.
    /// </summary>
    public abstract class CustomerContextBase : ICustomerContext
    {
        #region Fields

        /// <summary>
        /// The key used to store the Umbraco Member Id in CustomerContextData.
        /// </summary>
        protected const string UmbracoMemberIdDataKey = "umbMemberId";

        /// <summary>
        /// The consumer cookie key.
        /// </summary>
        protected const string CustomerCookieName = "merchello";

        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The customer service.
        /// </summary>
        private readonly ICustomerService _customerService;

        /// <summary>
        /// The <see cref="UmbracoContext"/>.
        /// </summary>
        private readonly UmbracoContext _umbracoContext;

        /// <summary>
        /// The <see cref="CacheHelper"/>.
        /// </summary>
        private readonly CacheHelper _cache;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerContextBase"/> class.
        /// </summary>
        protected CustomerContextBase()
            : this(UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerContextBase"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>.
        /// </param>
        protected CustomerContextBase(UmbracoContext umbracoContext)
            : this(MerchelloContext.Current, umbracoContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerContextBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="umbracoContext">
        /// The <see cref="UmbracoContext"/>.
        /// </param>
        protected CustomerContextBase(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(umbracoContext, "umbracoContext");
           
            this._merchelloContext = merchelloContext;
            this._umbracoContext = umbracoContext;
            this._customerService = merchelloContext.Services.CustomerService;
            this._cache = merchelloContext.Cache;
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the current customer.
        /// </summary>
        public ICustomerBase CurrentCustomer { get; protected set; }

        /// <summary>
        /// Gets the context data.
        /// </summary>
        protected CustomerContextData ContextData { get; private set; }        

        /// <summary>
        /// Gets the <see cref="UmbracoContext"/>.
        /// </summary>
        protected UmbracoContext UmbracoContext 
        {
            get
            {
                return this._umbracoContext;
            } 
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        protected CacheHelper Cache
        {
            get
            {
                return this._cache;
            }
        }

        /// <summary>
        /// Gets the <see cref="ICustomerService"/>.
        /// </summary>
        protected ICustomerService CustomerService
        {
            get
            {
                return this._customerService;
            }
        }       

        /// <summary>
        /// Sets a value in the encrypted Merchello cookie
        /// </summary>
        /// <param name="key">
        /// The key for the value
        /// </param>
        /// <param name="value">
        /// The actual value to be save.
        /// </param>
        /// <remarks>
        /// Keep in mind this is just a cookie which has limited size.  This is intended for 
        /// small bits of information.
        /// </remarks>
        public void SetValue(string key, string value)
        {
            if (this.ContextData.Values.Any(x => x.Key == key))
            {
                var idx = this.ContextData.Values.FindIndex(x => x.Key == key);
                this.ContextData.Values.RemoveAt(idx);
            }

            this.ContextData.Values.Add(new KeyValuePair<string, string>(key, value));

            this.CacheCustomer(this.CurrentCustomer);
        }

        /// <summary>
        /// Gets a value from the encrypted Merchello cookie
        /// </summary>
        /// <param name="key">
        /// The key of the value to retrieve
        /// </param>
        /// <returns>
        /// The value stored in the cookie as a string.
        /// </returns>
        public string GetValue(string key)
        {
            return this.ContextData.Values.FirstOrDefault(x => x.Key == key).Value;
        }

        /// <summary>
        /// Reinitializes the customer context
        /// </summary>
        /// <param name="customer">
        /// The <see cref="CustomerBase"/>
        /// </param>
        /// <remarks>
        /// Sometimes useful to clear the various caches used internally in the customer context
        /// </remarks>
        public void Reinitialize(ICustomerBase customer)
        {
            // customer has logged out, so we need to go back to an anonymous customer
            var cookie = this._umbracoContext.HttpContext.Request.Cookies[CustomerCookieName];

            if (cookie == null)
            {
                this.Initialize();
                return;
            }

            cookie.Expires = DateTime.Now.AddDays(-1);

            this._cache.RequestCache.ClearCacheItem(CustomerCookieName);
            this._cache.RuntimeCache.ClearCacheItem(CacheKeys.CustomerCacheKey(customer.Key));

            this.Initialize();
        }        

        /// <summary>
        /// Converts an anonymous basket to a customer basket.
        /// </summary>
        /// <param name="anonymousBasket">
        /// The anonymous basket.
        /// </param>
        /// <param name="customerBasket">
        /// The customer basket.
        /// </param>
        protected void ConvertBasket(IBasket anonymousBasket, IBasket customerBasket)
        {
            var type = Type.GetType(
                            MerchelloConfiguration.Current.GetStrategyElement(
                                "DefaultAnonymousBasketConversionStrategy").Type);

            var attempt = ActivatorHelper.CreateInstance<BasketConversionBase>(
                type,
                new object[] { anonymousBasket, customerBasket });

            if (!attempt.Success)
            {
                LogHelper.Error<CustomerContext>("Failed to convert anonymous basket to customer basket", attempt.Exception);
                return;
            }

            attempt.Result.Merge();
        }

        /// <summary>
        /// Attempts to either retrieve an anonymous customer or an existing customer
        /// </summary>
        /// <param name="key">The key of the customer to retrieve</param>
        protected abstract void TryGetCustomer(Guid key);


        /// <summary>
        /// The caches the customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        protected void CacheCustomer(ICustomerBase customer)
        {
            // set/reset the cookie 
            // TODO decide how we want to deal with cookie persistence options
            var cookie = new HttpCookie(CustomerCookieName)
            {
                Value = this.ContextData.ToJson()
            };

            // Ensure a session cookie for Anonymous customers
            // TODO - on persisted authenticcation, we need to synch the cookie expiration
            if (customer.IsAnonymous) cookie.Expires = DateTime.MinValue;

            this._umbracoContext.HttpContext.Response.Cookies.Add(cookie);

            this._cache.RequestCache.GetCacheItem(CustomerCookieName, () => this.ContextData);
            this._cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(customer.Key), () => customer, TimeSpan.FromMinutes(5), true);
        }

        /// <summary>
        /// Converts an anonymous customer's basket to a customer basket
        /// </summary>
        /// <param name="customer">
        /// The anonymous customer - <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="membershipId">
        /// The Membership Providers .
        /// </param>
        /// <param name="customerLoginName">
        /// The customer login name.
        /// </param>
        protected void ConvertBasket(ICustomerBase customer, string membershipId, string customerLoginName)
        {
            var anonymousBasket = Basket.GetBasket(this._merchelloContext, customer);

            customer = this.CustomerService.GetByLoginName(customerLoginName) ??
                            this.CustomerService.CreateCustomerWithKey(customerLoginName);


            this.ContextData.Key = customer.Key;
            this.ContextData.Values.Add(new KeyValuePair<string, string>(UmbracoMemberIdDataKey, membershipId));
            var customerBasket = Basket.GetBasket(this._merchelloContext, customer);

            //// convert the customer basket
            this.ConvertBasket(anonymousBasket, customerBasket);

            this.CacheCustomer(customer);
            this.CurrentCustomer = customer;
        }

        /// <summary>
        /// Creates an anonymous customer
        /// </summary>
        protected void CreateAnonymousCustomer()
        {
            var customer = this._customerService.CreateAnonymousCustomerWithKey();
            this.CurrentCustomer = customer;
            this.ContextData = new CustomerContextData()
            {
                Key = customer.Key
            };

            this.CacheCustomer(customer);
        }

        /// <summary>
        /// Provides an assertion that the customer cookie is associated with the correct customer Umbraco member relation.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="membershipId">The Membership Provider's id used.  Usually an int or Guid value</param>
        /// <remarks>
        /// Addresses issue http://issues.merchello.com/youtrack/issue/M-454
        /// </remarks>
        protected void EnsureIsLoggedInCustomer(ICustomerBase customer, string membershipId)
        {
            if (this._cache.RequestCache.GetCacheItem(CacheKeys.EnsureIsLoggedInCustomerValidated(customer.Key)) != null) return;

            var dataValue = this.ContextData.Values.FirstOrDefault(x => x.Key == UmbracoMemberIdDataKey);

            // If the dataValues do not contain the umbraco member id reinitialize
            if (!string.IsNullOrEmpty(dataValue.Value))
            {
                // Assert are equal
                if (!dataValue.Value.Equals(membershipId)) this.Reinitialize(customer);
                return;
            }

            if (dataValue.Value != membershipId) this.Reinitialize(customer);
        }

        /// <summary>
        /// Initializes this class with default values
        /// </summary>
        private void Initialize()
        {
            // see if the key is already in the request cache
            var cachedContextData = this._cache.RequestCache.GetCacheItem(CustomerCookieName);

            if (cachedContextData != null)
            {
                this.ContextData = (CustomerContextData)cachedContextData;
                var key = this.ContextData.Key;
                this.TryGetCustomer(key);
                return;
            }

            // retrieve the merchello consumer cookie
            var cookie = this._umbracoContext.HttpContext.Request.Cookies[CustomerCookieName];

            if (cookie != null)
            {
                try
                {
                    this.ContextData = cookie.ToCustomerContextData();
                    this.TryGetCustomer(this.ContextData.Key);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<CustomerContext>("Decrypted guid did not parse", ex);
                    this.CreateAnonymousCustomer();
                }
            }
            else
            {
                this.CreateAnonymousCustomer();
            } // a cookie was not found
        }
    }
}