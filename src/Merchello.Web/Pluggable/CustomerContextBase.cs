namespace Merchello.Web.Pluggable
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web;

    using Merchello.Core;
    using Merchello.Core.Cache;
    using Merchello.Core.Configuration;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Customer;
    using Merchello.Web.Workflow;

    using Newtonsoft.Json;

    using Umbraco.Core;
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

        /// <summary>
        /// The number of days in which to persist an anonymous customer cookie.
        /// </summary>
        private readonly int _anonCookieExpireDays;

        #endregion

        #region Constructors

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
            this._anonCookieExpireDays = MerchelloConfiguration.Current.AnonymousCustomerCookieExpiresDays;
            this._customerService = merchelloContext.Services.CustomerService;
            this._cache = merchelloContext.Cache;
            this.Initialize();
        }

        #endregion

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

        #region ContextData

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
            CurrentCustomer.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.CustomerContextDataPrefix + key, value);

            if (CurrentCustomer.IsAnonymous)
            {
                _customerService.Save((IAnonymousCustomer)CurrentCustomer);
            }
            else
            {
                _customerService.Save((ICustomer)CurrentCustomer);
            }
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
            return CurrentCustomer.ExtendedData.GetValue(Core.Constants.ExtendedDataKeys.CustomerContextDataPrefix + key);
        }

        #endregion

        /// <summary>
        /// Reinitializes the customer context
        /// </summary>
        /// <param name="customer">
        /// The <see cref="CustomerBase"/>
        /// </param>
        /// <remarks>
        /// Sometimes useful to clear the various caches used internally in the customer context
        /// </remarks>
        public virtual void Reinitialize(ICustomerBase customer)
        {
            // customer has logged out, so we need to go back to an anonymous customer
            var cookie = this._umbracoContext.HttpContext.Request.Cookies[CustomerCookieName];

            if (cookie == null)
            {
                this.Initialize();
                return;
            }

            //// cookie.Expires = DateTime.Now.AddDays(-1);

            this._cache.RequestCache.ClearCacheItem(CustomerCookieName);
            this._cache.RuntimeCache.ClearCacheItem(CacheKeys.CustomerCacheKey(customer.Key));

            this.Initialize();
        }

        /// <summary>
        /// Returns true or false indicating whether or not the current membership user is logged in.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> indicating whether the current user is logged in.
        /// </returns>
        protected abstract bool GetIsCurrentlyLoggedIn();

        /// <summary>
        /// Gets the member/user login or user name used to sign in
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <remarks>
        /// Merchello makes the association between membership provider users and Merchello customers by username
        /// </remarks>
        protected abstract string GetMembershipProviderUserName();

        /// <summary>
        /// Gets the unique ID from the Membership Provider
        /// </summary>
        /// <returns>
        /// The ID or key from the Membership provider as a string value 
        /// </returns>
        protected abstract string GetMembershipProviderKey();


        /// <summary>
        /// Attempts to either retrieve an anonymous customer or an existing customer
        /// </summary>
        /// <param name="key">The key of the customer to retrieve</param>
        protected virtual void TryGetCustomer(Guid key)
        {
            // REFACTOR-v3 - this should come directly from the service as this is redundant and creates
            // a second (context specific) cache item.  However, since we're not cloning the cached item
            // out of cache this does create somewhat of a protection against accidently changing values.
            // Also, ideally, we should use a proxy of ICustomerBase so that the customer values are immutable.
            var customer = (ICustomerBase)Cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(key));

            // use the IsLoggedIn method to check which gets/sets the value in the Request Cache
            var isLoggedIn = this.IsLoggedIn(key);

            // Check the cache for a previously retrieved customer.
            // There can be many requests for the current customer during a single request.
            if (customer != null)
            {
                CurrentCustomer = customer;

                // No we need to assert whether or not the authentication status has changed
                // during this request - the user either logged in or has logged out.
                if (customer.IsAnonymous)
                {
                    // We have an anonymous customer but the user is now authenticated so we may want to create an 
                    // customer and convert the basket
                    if (isLoggedIn)
                    {
                        this.EnsureCustomerCreationAndConvertBasket(customer);
                    }
                }
                else if (customer.IsAnonymous == false && isLoggedIn == false)
                {
                    // The customer that was found was not anonymous and yet the member is 
                    // not logged in.
                    var values = customer.ExtendedData.GetItemsByKeyPrefix(Core.Constants.ExtendedDataKeys.CustomerContextDataPrefix);
                    CreateAnonymousCustomer();
                    UpdateContextData(CurrentCustomer, values);
                    return;
                }
                else if (customer.IsAnonymous == false && isLoggedIn)
                {
                    // User may have logged out and logged in with a different customer
                    // Addresses issue http://issues.merchello.com/youtrack/issue/M-454
                    this.EnsureIsLoggedInCustomer(customer, this.GetMembershipProviderKey());
                    return;
                }

                // The customer key MUST be set in the ContextData
                ContextData.Key = customer.Key;
                return;
            }

            // Customer has not been cached so we have to start from scratch.
            customer = CustomerService.GetAnyByKey(key);

            if (customer != null)
            {
                //// There is either a Customer or Anonymous Customer record

                CurrentCustomer = customer;
                ContextData.Key = customer.Key;

                // The current Membership Providers "ID or Key" is stored in the ContextData so that we can "ensure" that the current logged
                // in member is the same as the reference we have to a previously logged in member in the same browser.
                if (isLoggedIn) ContextData.Pid = this.GetMembershipProviderKey(); 
                
                ////ContextData.Values.Add(new KeyValuePair<string, string>(UmbracoMemberIdDataKey, this.GetMembershipProviderKey()));

                // FYI this is really only to set the customer cookie so this entire block
                // should be merged into the section of code directly above.
                CacheCustomer(customer);
            }
            else
            {
                //// No records were found - create a new Anonymous Customer
                CreateAnonymousCustomer();
            }
        }

        /// <summary>
        /// The ensure customer and convert basket.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        protected virtual void EnsureCustomerCreationAndConvertBasket(ICustomerBase customer)
        {
            ConvertBasket(customer, this.GetMembershipProviderKey(), this.GetMembershipProviderUserName());
        }

        /// <summary>
        /// Converts an anonymous customer's basket to a customer basket
        /// </summary>
        /// <param name="original">
        /// The anonymous customer - <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="membershipId">
        /// The Membership Providers .
        /// </param>
        /// <param name="customerLoginName">
        /// The customer login name.
        /// </param>
        protected void ConvertBasket(ICustomerBase original, string membershipId, string customerLoginName)
        {
            var anonymousBasket = Basket.GetBasket(this._merchelloContext, original);

            var customer = this.CustomerService.GetByLoginName(customerLoginName) ??
                            this.CustomerService.CreateCustomerWithKey(customerLoginName);


            this.ContextData.Key = customer.Key;
            this.ContextData.Pid = membershipId;

            //// this.ContextData.Values.Add(new KeyValuePair<string, string>(UmbracoMemberIdDataKey, membershipId));

            var customerBasket = Basket.GetBasket(this._merchelloContext, customer);

            //// convert the customer basket
            ConvertBasket(anonymousBasket, customerBasket);
            CopyContextData(original, customer);

            this.CacheCustomer(customer);
            this.CurrentCustomer = customer;
        }

        /// <summary>
        /// Wrapper to cache the logged in status in the request cache
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool IsLoggedIn(Guid key)
        {
            return (bool)Cache.RequestCache.GetCacheItem(CacheKeys.CustomerIsLoggedIn(key), () => this.GetIsCurrentlyLoggedIn());
        }

        /// <summary>
        /// Wrapper to cache the membership username in the request cache
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string MembershipUserName(Guid key)
        {
            return (string)Cache.RequestCache.GetCacheItem(CacheKeys.CustomerMembershipUserName(key), this.GetMembershipProviderUserName);
        }

        /// <summary>
        /// Wrapper to cache the membership provider key (id) from the request cache
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string MembershipProviderKey(Guid key)
        {
            return (string)Cache.RequestCache.GetCacheItem(CacheKeys.CustomerMembershipProviderKey(key), this.GetMembershipProviderKey);
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
        private static void ConvertBasket(IBasket anonymousBasket, IBasket customerBasket)
        {
            var type = Type.GetType(
                            MerchelloConfiguration.Current.GetStrategyElement(
                                "DefaultAnonymousBasketConversionStrategy").Type);

            var attempt = ActivatorHelper.CreateInstance<BasketConversionBase>(
                type,
                new object[] { anonymousBasket, customerBasket });

            if (!attempt.Success)
            {
                MultiLogHelper.Error<CustomerContext>("Failed to convert anonymous basket to customer basket", attempt.Exception);
                return;
            }

            attempt.Result.Merge();
        }

        /// <summary>
        /// Copies the context data stored in the customer extended data collection.
        /// </summary>
        /// <param name="original">
        /// The original.
        /// </param>
        /// <param name="converted">
        /// The converted.
        /// </param>
        private static void CopyContextData(ICustomerBase original, ICustomerBase converted)
        {
            var values = original.ExtendedData.GetItemsByKeyPrefix(Core.Constants.ExtendedDataKeys.CustomerContextDataPrefix);
            UpdateContextData(converted, values);
        }

        /// <summary>
        /// Updates the context data.
        /// </summary>
        /// <param name="converted">
        /// The converted.
        /// </param>
        /// <param name="ctxValues">
        /// The context values.
        /// </param>
        private static void UpdateContextData(ICustomerBase converted, IEnumerable<KeyValuePair<string, string>> ctxValues)
        {
            foreach (var value in ctxValues)
            {
                converted.ExtendedData.SetValue(value.Key, value.Value);
            }
        }

        /// <summary>
        /// Creates an anonymous customer
        /// </summary>
        private void CreateAnonymousCustomer()
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private void EnsureIsLoggedInCustomer(ICustomerBase customer, string membershipId)
        {
            if (this._cache.RequestCache.GetCacheItem(CacheKeys.EnsureIsLoggedInCustomerValidated(customer.Key)) != null) return;

            //// var dataValue = this.ContextData.Values.FirstOrDefault(x => x.Key == UmbracoMemberIdDataKey);

            // If the dataValues do not contain the umbraco member id reinitialize
            if (!string.IsNullOrEmpty(this.ContextData.Pid))
            {
                // Assert are equal
                if (!this.ContextData.Pid.Equals(membershipId)) this.Reinitialize(customer);
                return;
            }

            if (this.ContextData.Pid != membershipId) this.Reinitialize(customer);
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
                var parsedOk = false;
                try
                {
                    this.ContextData = cookie.ToCustomerContextData();
                    parsedOk = true;
                }
                catch (JsonException ex)
                {
                    MultiLogHelper.Error<CustomerContext>("Decrypted guid did not parse", ex);
                    this.CreateAnonymousCustomer();
                }

                if (parsedOk) this.TryGetCustomer(this.ContextData.Key);
            }
            else
            {
                this.CreateAnonymousCustomer();
            } // a cookie was not found
        }

        /// <summary>
        /// The caches the customer.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        private void CacheCustomer(ICustomerBase customer)
        {
            // set/reset the cookie 
            var cookie = new HttpCookie(CustomerCookieName)
            {
                Value = this.ContextData.ToJson()
            };

            // Ensure a session cookie for Anonymous customers
            if (customer.IsAnonymous)
            {
                if (_anonCookieExpireDays <= 0)
                {
                    cookie.Expires = DateTime.MinValue;
                }
                else
                {
                    var expires = DateTime.Now.AddDays(_anonCookieExpireDays);
                    cookie.Expires = expires;
                }
            }

            this._umbracoContext.HttpContext.Response.Cookies.Add(cookie);

            this._cache.RequestCache.GetCacheItem(CustomerCookieName, () => this.ContextData);
            this._cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(customer.Key), () => customer, TimeSpan.FromMinutes(15), true);
        }
    }
}