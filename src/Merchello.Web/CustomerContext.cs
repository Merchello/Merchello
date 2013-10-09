using System;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Web.Cache;
using umbraco.cms.businesslogic.member;
using Umbraco.Core;
using Umbraco.Web;

namespace Merchello.Web
{
    public class CustomerContext
    {
        private const string ConsumerCookieKey = "merchello";
        private readonly MerchelloContext _merchelloContext;
        private readonly UmbracoContext _umbracoContext;
        private readonly CacheHelper _cache;
        private IConsumer _consumer;

        public CustomerContext()
            : this(MerchelloContext.Current, UmbracoContext.Current)
        {}


        public CustomerContext(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(umbracoContext, "umbracoContext");

            _umbracoContext = umbracoContext;
            _merchelloContext = merchelloContext;
            _cache = merchelloContext.Cache;

            InitCustomer();
        }

        private void InitCustomer()
        {            
            // retrieve the merchello consumer cookie
            var cookie = _umbracoContext.HttpContext.Request.Cookies[ConsumerCookieKey];         
   
            if (cookie != null)
            {                
                Guid key;
                if (!Guid.TryParse(EncryptionHelper.Decrypt(cookie.Value), out key))
                { 
                    var consumer = _cache.RuntimeCache.GetCacheItem(CachingBacker.ConsumerCacheKey(key));

                    if (consumer != null)
                    {
                        _consumer = (IConsumer) consumer;
                    }
                    else { BuildConsumer(key); } // consumer not cached
                }
                else { GetConsumer(); } // consumer key parsing failed ... start over
            }
            else { GetConsumer(); } // a cookie was not found
        }

        private Guid GetConsumerKey()
        {
            // see if the key is already in the request cache
            var cachedKey = _cache.RequestCache.GetCacheItem(ConsumerCookieKey);
            if (cachedKey != null) return (Guid)cachedKey;
 
            // retrieve the merchello consumer cookie
            var cookie = _umbracoContext.HttpContext.Request.Cookies[ConsumerCookieKey];
           
            if (cookie != null)
            {
                Guid key;
                if (!Guid.TryParse(EncryptionHelper.Decrypt(cookie.Value), out key))
                {
                    _cache.RequestCache.GetCacheItem(ConsumerCookieKey, () => key);
                    return key;
                }
            }

            // first hit to site? 
            if (_umbracoContext.Security.IsMemberAuthorized())
            {
                var member = Member.GetCurrentMember();
                // future will look for returning customers
                var customer = _merchelloContext.Services.CustomerService.GetByMemberId(member.Id);
                if (customer != null)
                {
                    _cache.RequestCache.GetCacheItem(ConsumerCookieKey, () => customer.Key);
                    _cache.RuntimeCache.GetCacheItem(CachingBacker.ConsumerCacheKey(customer.Key), () => customer, TimeSpan.FromMinutes(5), true);
                    return customer.Key;
                }
            }
            else
            {
                // create a new anonymous customer

            }

            return Guid.Empty;
        }

        

        private void GetConsumer()
        {
            throw new NotImplementedException();
        }

        private void BuildConsumer(Guid key)
        {
            throw new NotImplementedException();
        }


    }
}