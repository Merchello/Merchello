
namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Security;

    /// <summary>
    /// Represents the customer context.
    /// </summary>
    public class CustomerContext
    {
        #region Fields

        /// <summary>
        /// The key used to store the Umbraco Member Id in CustomerContextData.
        /// </summary>
        private const string UmbracoMemberIdDataKey = "umbMemberId";

        /// <summary>
        /// The consumer cookie key.
        /// </summary>
        private const string CustomerCookieName = "merchello";

        /// <summary>
        /// The merchello context.
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The customer service.
        /// </summary>
        private readonly ICustomerService _customerService;

        /// <summary>
        /// The member service.
        /// </summary>
        private readonly IMemberService _memberService;

        /// <summary>
        /// The <see cref="UmbracoContext"/>.
        /// </summary>
        private readonly UmbracoContext _umbracoContext;

        /// <summary>
        /// The <see cref="CacheHelper"/>.
        /// </summary>
        private readonly CacheHelper _cache;

        /// <summary>
        /// The membership helper.
        /// </summary>
        private readonly MembershipHelper _membershipHelper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerContext"/> class.
        /// </summary>
        public CustomerContext()
            : this(UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerContext"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        public CustomerContext(UmbracoContext umbracoContext)
            : this(MerchelloContext.Current, ApplicationContext.Current.Services.MemberService, umbracoContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerContext"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="memberService">
        /// The member Service.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        internal CustomerContext(IMerchelloContext merchelloContext, IMemberService memberService, UmbracoContext umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(umbracoContext, "umbracoContext");
            Mandate.ParameterNotNull(memberService, "memberService");

            _merchelloContext = merchelloContext;
            _umbracoContext = umbracoContext;
            _customerService = merchelloContext.Services.CustomerService;
            _memberService = memberService;
            _cache = merchelloContext.Cache;

            _membershipHelper = new MembershipHelper(_umbracoContext);

            Initialize();
        }

        #endregion

        /// <summary>
        /// Gets the current customer
        /// </summary>
        public ICustomerBase CurrentCustomer { get; private set; }

        /// <summary>
        /// Gets the context data.
        /// </summary>
        public CustomerContextData ContextData { get; private set; }

        /// <summary>
        /// Initializes this class with default values
        /// </summary>
        private void Initialize()
        {
            // see if the key is already in the request cache
            var cachedContextData = _cache.RequestCache.GetCacheItem(CustomerCookieName);

            if (cachedContextData != null)
            {
                ContextData = (CustomerContextData)cachedContextData;
                var key = ContextData.Key;
                TryGetCustomer(key);
                return;
            }

            // retrieve the merchello consumer cookie
            var cookie = _umbracoContext.HttpContext.Request.Cookies[CustomerCookieName];

            if (cookie != null)
            {                
                try
                {
                    ContextData = cookie.ToCustomerContextData();
                    TryGetCustomer(ContextData.Key);
                }
                catch (Exception ex)
                {
                    LogHelper.Error<CustomerContext>("Decrypted guid did not parse", ex);
                    CreateAnonymousCustomer();
                }                             
            }
            else 
            {
                CreateAnonymousCustomer();
            } // a cookie was not found
        }

        /// <summary>
        /// Attempts to either retrieve an anonymous customer or an existing customer
        /// </summary>
        /// <param name="key">The key of the customer to retrieve</param>
        private void TryGetCustomer(Guid key)
        {
            var customer = (ICustomerBase)_cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(key));

            var isLoggedIn = (bool)_cache.RequestCache.GetCacheItem(CacheKeys.CustomerIsLoggedIn(key), () => _membershipHelper.IsLoggedIn());

            // check the cache for a previously retrieved customer
            if (customer != null)
            {
                CurrentCustomer = customer;

                if (customer.IsAnonymous)
                {
                    if (isLoggedIn)
                    {                        
                        var memberId = _membershipHelper.GetCurrentMemberId();
                        var member = _memberService.GetById(memberId);

                        if (MerchelloConfiguration.Current.CustomerMemberTypes.Any(x => x == member.ContentTypeAlias))
                        {                          
                            var anonymousBasket = Basket.GetBasket(_merchelloContext, customer);

                            customer = _customerService.GetByLoginName(member.Username) ??
                                            _customerService.CreateCustomerWithKey(member.Username);


                            ContextData.Key = customer.Key;
                            ContextData.Values.Add(new KeyValuePair<string, string>(UmbracoMemberIdDataKey, memberId.ToString(CultureInfo.InvariantCulture)));
                            var customerBasket = Basket.GetBasket(_merchelloContext, customer);

                            //// convert the customer basket
                            ConvertBasket(anonymousBasket, customerBasket);

                            CacheCustomer(customer);
                            CurrentCustomer = customer;

                            return;
                        }
                    }
                }
                else if (customer.IsAnonymous == false && isLoggedIn == false)
                {
                    CreateAnonymousCustomer();
                    return;
                }
                else if (customer.IsAnonymous == false && isLoggedIn)
                {
                    this.EnsureIsLoggedInCustomer(customer);
                }

                ContextData.Key = customer.Key;

                return;
            }

            // try to get the customer
            customer = _customerService.GetAnyByKey(key);
           
            if (customer != null)
            {
                CurrentCustomer = customer;
                ContextData.Key = customer.Key;
                if (isLoggedIn) ContextData.Values.Add(new KeyValuePair<string, string>(UmbracoMemberIdDataKey, _membershipHelper.GetCurrentMemberId().ToString(CultureInfo.InvariantCulture)));
                CacheCustomer(customer);
            }
            else 
            {
                // create a new anonymous customer
                CreateAnonymousCustomer();
            }
        }

        /// <summary>
        /// Invalidates the current initialization and starts over
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        private void Reinitialize(ICustomerBase customer)
        {
            // customer has logged out, so we need to go back to an anonymous customer
            var cookie = _umbracoContext.HttpContext.Request.Cookies[CustomerCookieName];

            if (cookie == null)
            {
                this.Initialize();
                return;
            }

            cookie.Expires = DateTime.Now.AddDays(-1);

            _cache.RequestCache.ClearCacheItem(CustomerCookieName);
            _cache.RuntimeCache.ClearCacheItem(CacheKeys.CustomerCacheKey(customer.Key));

            Initialize();      
        }

        /// <summary>
        /// Provides an assertion that the customer cookie is associated with the correct customer Umbraco member relation.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <remarks>
        /// http://issues.merchello.com/youtrack/issue/M-454
        /// </remarks>
        private void EnsureIsLoggedInCustomer(ICustomerBase customer)
        {
            if (_cache.RequestCache.GetCacheItem(CacheKeys.EnsureIsLoggedInCustomerValidated(customer.Key)) != null) return;

            var memberId = _membershipHelper.GetCurrentMemberId();
            var dataValue = ContextData.Values.FirstOrDefault(x => x.Key == UmbracoMemberIdDataKey);

            // If the dataValues do not contain the umbraco member id reinitialize
            if (!string.IsNullOrEmpty(dataValue.Value))
            {
                // Assert are equal
                if (!dataValue.Value.Equals(memberId.ToString(CultureInfo.InvariantCulture))) this.Reinitialize(customer);
                return;
            }

            if (dataValue.Value != memberId.ToString(CultureInfo.InvariantCulture)) this.Reinitialize(customer);
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
        private void ConvertBasket(IBasket anonymousBasket, IBasket customerBasket)
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
        /// Creates an anonymous customer
        /// </summary>
        private void CreateAnonymousCustomer()
        {
            var customer = _customerService.CreateAnonymousCustomerWithKey();
            CurrentCustomer = customer;
            ContextData = new CustomerContextData()
            {
                Key = customer.Key
            };

            CacheCustomer(customer);
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
            // TODO decide how we want to deal with cookie persistence options
            var cookie = new HttpCookie(CustomerCookieName)
            {
                Value = ContextData.ToJson()
            };

            // Ensure a session cookie for Anonymous customers
            if (customer.IsAnonymous) cookie.Expires = DateTime.MinValue;

            _umbracoContext.HttpContext.Response.Cookies.Add(cookie);

            _cache.RequestCache.GetCacheItem(CustomerCookieName, () => ContextData);
            _cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(customer.Key), () => customer, TimeSpan.FromMinutes(5), true);
        }
    }
}