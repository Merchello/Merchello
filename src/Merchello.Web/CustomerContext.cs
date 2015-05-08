
namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Cache;
    using Merchello.Core.Configuration;
    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Web.Security;

    /// <summary>
    /// Represents the customer context.
    /// </summary>
    public class CustomerContext : CustomerContextBase
    {
        /// <summary>
        /// The member service.
        /// </summary>
        private readonly IMemberService _memberService;

        /// <summary>
        /// The membership helper.
        /// </summary>
        private readonly MembershipHelper _membershipHelper;

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
            : base(merchelloContext, umbracoContext)
        {
             Mandate.ParameterNotNull(memberService, "memberService");

            _memberService = memberService;            
            _membershipHelper = new MembershipHelper(umbracoContext);            
        }

        #endregion


        /// <summary>
        /// Attempts to either retrieve an anonymous customer or an existing customer
        /// </summary>
        /// <param name="key">The key of the customer to retrieve</param>
        protected override void TryGetCustomer(Guid key)
        {
            var customer = (ICustomerBase)Cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(key));

            var isLoggedIn = (bool)Cache.RequestCache.GetCacheItem(CacheKeys.CustomerIsLoggedIn(key), () => _membershipHelper.IsLoggedIn());

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
                    // persisted customer record.
                    if (isLoggedIn)
                    {                        
                        var memberId = _membershipHelper.GetCurrentMemberId();
                        var member = _memberService.GetById(memberId);
                       
                        // By default, Merchello only creates Merchello Customers if the MemberType is listed in the 
                        // merchello.config file
                        if (MerchelloConfiguration.Current.CustomerMemberTypes.Any(x => x == member.ContentTypeAlias))
                        {                      
                            // Use the configured strategy to convert the members basket from an anonymous basket to 
                            // a customer basket
                            ConvertBasket(customer, memberId.ToString(CultureInfo.InvariantCulture), member.Username);

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
                    // User may have logged out and logged in with a different customer
                    // Addresses issue http://issues.merchello.com/youtrack/issue/M-454
                    this.EnsureIsLoggedInCustomer(customer, _membershipHelper.GetCurrentMemberId().ToString(CultureInfo.InvariantCulture));
                }

                ContextData.Key = customer.Key;

                return;
            }

            // Customer has not been cached so we have to start from scratch.
            customer = CustomerService.GetAnyByKey(key);
           
            if (customer != null)
            {
                CurrentCustomer = customer;
                ContextData.Key = customer.Key;

                // TODO RSS this should be moved to the base class
                // The current Membership Providers "ID or Key" is stored in the ContextData so that we can "ensure" that the current logged
                // in member is the same as the reference we have to a previously logged in member in the same browser.
                if (isLoggedIn) ContextData.Values.Add(new KeyValuePair<string, string>(UmbracoMemberIdDataKey, _membershipHelper.GetCurrentMemberId().ToString(CultureInfo.InvariantCulture)));
                
                CacheCustomer(customer);
            }
            else 
            {
                // create a new anonymous customer
                CreateAnonymousCustomer();
            }
        }

    }
}