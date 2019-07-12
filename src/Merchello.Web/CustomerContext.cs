
namespace Merchello.Web
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Merchello.Core;
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
        /// The member service.
        /// </summary>
        private readonly IMemberService _memberService = ApplicationContext.Current.Services.MemberService;

        /// <summary>
        /// The Umbraco <see cref="MembershipHelper"/>.
        /// </summary>
        /// <remarks>
        /// This is actually used within the property.
        /// </remarks>
        // ReSharper disable once UnassignedField.Compiler
        private MembershipHelper _membershipHelper;

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
        /// Returns true or false indicating whether or not the current membership user is logged in.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/> indicating whether the current user is logged in.
        /// </returns>
        protected override bool GetIsCurrentlyLoggedIn()
        {
            return MembershipHelper.IsLoggedIn();
        }

        /// <summary>
        /// The get membership provider user name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetMembershipProviderUserName()
        {
            var member = _memberService.GetById(MembershipHelper.GetCurrentMemberId());
            return member.Username;
        }

        /// <summary>
        /// The get membership provider key.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetMembershipProviderKey()
        {
            var id = MembershipHelper.GetCurrentMemberId();
            return id == 0 ? string.Empty : id.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The ensure customer creation and convert basket.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        protected override void EnsureCustomerCreationAndConvertBasket(ICustomerBase customer)
        {
            if (!customer.IsAnonymous)
            {
                return;
            }

            var membershipProviderKey = this.MembershipProviderKey(customer.Key);
            if (!int.TryParse(membershipProviderKey, NumberStyles.None, CultureInfo.InvariantCulture, out var memberId))
            {
                return;
            }

            var member = _memberService.GetById(memberId);
            if (member != null && MerchelloConfiguration.Current.CustomerMemberTypes.Any(x => x == member.ContentTypeAlias))
            {
                base.EnsureCustomerCreationAndConvertBasket(customer);
            }
        }

    }
}