
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
    using Merchello.Web.Pluggable;

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
        /// The Umbraco <see cref="MembershipHelper"/>.
        /// </summary>
        private MembershipHelper _membershipHelper;

        /// <summary>
        /// The member service.
        /// </summary>
        private readonly IMemberService _memberService = ApplicationContext.Current.Services.MemberService;

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
            : base(MerchelloContext.Current, umbracoContext)
        {
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="MembershipHelper"/>.
        /// </summary>
        private MembershipHelper MembershipHelper
        {
            get
            {
                return _membershipHelper ?? new MembershipHelper(UmbracoContext);
            }
        }

        /// <summary>
        /// Attempts to either retrieve an anonymous customer or an existing customer
        /// </summary>
        /// <param name="key">The key of the customer to retrieve</param>
        protected override void TryGetCustomer(Guid key)
        {
            var customer = (ICustomerBase)Cache.RuntimeCache.GetCacheItem(CacheKeys.CustomerCacheKey(key));

            var isLoggedIn = (bool)Cache.RequestCache.GetCacheItem(CacheKeys.CustomerIsLoggedIn(key), () => MembershipHelper.IsLoggedIn());

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
                        var memberId = MembershipHelper.GetCurrentMemberId();
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
                    // The customer that was found was not anonymous and yet the member is 
                    // not logged in.
                    CreateAnonymousCustomer();
                    return;
                }
                else if (customer.IsAnonymous == false && isLoggedIn)
                {
                    // User may have logged out and logged in with a different customer
                    // Addresses issue http://issues.merchello.com/youtrack/issue/M-454
                    this.EnsureIsLoggedInCustomer(customer, MembershipHelper.GetCurrentMemberId().ToString(CultureInfo.InvariantCulture));
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

                // TODO RSS this should be moved to the base class
                // The current Membership Providers "ID or Key" is stored in the ContextData so that we can "ensure" that the current logged
                // in member is the same as the reference we have to a previously logged in member in the same browser.
                if (isLoggedIn) ContextData.Values.Add(new KeyValuePair<string, string>(UmbracoMemberIdDataKey, _membershipHelper.GetCurrentMemberId().ToString(CultureInfo.InvariantCulture)));
                
                // Cache the customer so that for each request we don't have to do a bunch of
                // DB lookups.
                CacheCustomer(customer);
            }
            else 
            {
                //// No records were found - create a new Anonymous Customer
                CreateAnonymousCustomer();
            }
        }
    }
}