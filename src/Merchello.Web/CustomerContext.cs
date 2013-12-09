using System;
using System.Web;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Web.Cache;
using Merchello.Web.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.EntityBase;
using umbraco.cms.businesslogic.member;
using Umbraco.Core;
using Umbraco.Web;

namespace Merchello.Web
{
    public class CustomerContext
    {
        private const string ConsumerCookieKey = "merchello";
        private readonly ICustomerService _customerService;
        private readonly UmbracoContext _umbracoContext;
        private readonly CacheHelper _cache;

        #region Constructors

        public CustomerContext()
            : this(UmbracoContext.Current)
        {}

        public CustomerContext(UmbracoContext umbracoContext)
            : this(MerchelloContext.Current, umbracoContext)
        {}

        internal CustomerContext(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(umbracoContext, "umbracoContext");

            _umbracoContext = umbracoContext;
            _customerService = merchelloContext.Services.CustomerService;
            _cache = merchelloContext.Cache;

            Initialize();
        }

        private void Initialize()
        {
            // see if the key is already in the request cache
            var cachedKey = _cache.RequestCache.GetCacheItem(ConsumerCookieKey);

            if (cachedKey != null)
            {
                var key = (Guid) cachedKey;              
                TryGetCustomer(key);
                return;
            }

            // retrieve the merchello consumer cookie
            var cookie = _umbracoContext.HttpContext.Request.Cookies[ConsumerCookieKey];
   
            if (cookie != null)
            {                
                Guid key;
                if (Guid.TryParse(EncryptionHelper.Decrypt(cookie.Value), out key))
                {
                    TryGetCustomer(key);
                }
                else
                {
                    LogHelper.Debug<CustomerContext>("Decrypted guid did not parse");
                    CreateAnonymousCustomer();
                } // consumer key parsing failed ... start over
            }
            else { CreateAnonymousCustomer(); } // a cookie was not found


        }

        #endregion

        /// <summary>
        /// Returns the current customer
        /// </summary>
        public ICustomerBase CurrentCustomer { get; private set; }

        /// <summary>
        /// Attempts to either retrieve an anonymous customer or an existing customer
        /// </summary>
        /// <param name="key">The key of the customer to retrieve</param>
        private void TryGetCustomer(Guid key)
        {
            var customer = (ICustomerBase)_cache.RuntimeCache.GetCacheItem(HttpCachingBacker.CostumerCacheKey(key));
            
            // check the cache for a previously retrieved customer
            if (customer != null)
            {
                CurrentCustomer = customer;
                return;
            }

            // If the member has been authenticated there is no need to create an anonymous record.
            // Either return an existing customer or create a new one for the member.
            //if (_umbracoContext.Security.IsMemberAuthorized())
            //{
            //    var member = Member.GetCurrentMember();

            //    customer = _customerService.GetByMemberId(member.Id) ??
            //                   _customerService.CreateCustomerWithId(member.Id);

            //    CacheCustomer(customer);
            //    CurrentCustomer = customer;
                
            //    return;                
            //}
           
            // try to get the customer
            customer = _customerService.GetAnyByKey(key);
           
            if (customer != null)
            {
                CurrentCustomer = customer;
                CacheCustomer(customer);
            }
            else // create a new anonymous customer
            {
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
            CacheCustomer(customer);
        }

        private void CacheCustomer(ICustomerBase customer)
        {
            // set/reset the cookie 
            // TODO decide how we want to deal with cookie persistence options - session based to start
            var cookie = new HttpCookie(ConsumerCookieKey)
            {
                Value = EncryptionHelper.Encrypt(customer.EntityKey.ToString())
            };
            _umbracoContext.HttpContext.Response.Cookies.Add(cookie);

            _cache.RequestCache.GetCacheItem(ConsumerCookieKey, () => customer.EntityKey);
            _cache.RuntimeCache.GetCacheItem(HttpCachingBacker.CostumerCacheKey(customer.EntityKey), () => customer, TimeSpan.FromMinutes(5), true);
        }
    }
}