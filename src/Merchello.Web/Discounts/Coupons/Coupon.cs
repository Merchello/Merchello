namespace Merchello.Web.Discounts.Coupons
{
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// Represents a discount Coupon.
    /// </summary>
    public class Coupon : OfferBase, ICoupon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coupon"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IOfferSettings"/>.
        /// </param>
        public Coupon(IOfferSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the collection discount constraints.
        /// </summary>
        public IEnumerable<OfferConstraintComponentBase> Constraints { get; private set; }

        /// <summary>
        /// Gets the coupon reward.
        /// </summary>
        public OfferRewardComponentBase Reward { get; private set; }

        public override Attempt<IOfferAwardResult<object>> TryToAward(object constrainBy, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }
    }
}