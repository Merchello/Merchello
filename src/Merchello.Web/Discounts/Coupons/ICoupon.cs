namespace Merchello.Web.Discounts.Coupons
{
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;

    /// <summary>
    /// Defines a Coupon.
    /// </summary>
    public interface ICoupon : IOffer
    {
        /// <summary>
        /// Gets the constraints.
        /// </summary>
        IEnumerable<OfferConstraintComponentBase> Constraints { get; }

        /// <summary>
        /// Gets the rewards.
        /// </summary>
        OfferRewardComponentBase Reward { get; }
    }
}