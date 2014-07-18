namespace Merchello.Web
{
    using System;
    using System.Web;

    using Merchello.Core;
    using Merchello.Core.Cache;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Customer;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web;
    using Umbraco.Web.Security;

    /// <summary>
    /// Represents the customer context.
    /// </summary>
    public class CustomerContext
    {
        #region Fields

        /// <summary>
        /// The consumer cookie key.
        /// </summary>
        private const string CustomerCookieName = "merchello";

        /// <summary>
        /// The _customer service.
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
            : this(MerchelloContext.Current, umbracoContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerContext"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        internal CustomerContext(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(umbracoContext, "umbracoContext");

            _umbracoContext = umbracoContext;
            _customerService = merchelloContext.Services.CustomerService;
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
                ContextData = cookie.ToCustomerContextData();

                try
                {
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

            //// check to see if member is signed in
            //// TODO RSS add configuration check for valid member type
           
            var customer = (ICustomerBase)_cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(key));
            
            // check the cache for a previously retrieved customer
            if (customer != null)
            {
                CurrentCustomer = customer;
                
                // customer.IsAnonymous and Member is not anonymous -> convert to ICustomer ... retrieve new or create

                ContextData.Key = customer.Key;

                return;
            }

            //// TODO persisted customers

            //// If the member has been authenticated there is no need to create an anonymous record.
            //// Either return an existing customer or create a new one for the member.
            ////if (_umbracoContext.Security.IsMemberAuthorized())
            ////{
            ////    var member = Member.GetCurrentMember();

            ////    customer = _customerService.GetByMemberId(member.Id) ??
            ////                   _customerService.CreateCustomerWithId(member.Id);

            ////    CacheCustomer(customer);
            ////    CurrentCustomer = customer;
                
            ////    return;                
            ////}
           
            // try to get the customer
            customer = _customerService.GetAnyByKey(key);
           
            if (customer != null)
            {
                CurrentCustomer = customer;
                ContextData.Key = customer.Key;
                CacheCustomer(customer);
            }
            else 
            {
                // create a new anonymous customer
                CreateAnonymousCustomer();
            }
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

            _umbracoContext.HttpContext.Response.Cookies.Add(cookie);

            _cache.RequestCache.GetCacheItem(CustomerCookieName, () => ContextData);
            _cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(customer.Key), () => customer, TimeSpan.FromMinutes(5), true);
        }
    }
}