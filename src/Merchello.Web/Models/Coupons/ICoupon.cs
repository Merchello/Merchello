namespace Merchello.Web.Models.Coupons
{
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Discounts.Coupons.Constraints;
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
        IEnumerable<DiscountConstraintBase> Constraints { get; }

        /// <summary>
        /// Gets the rewards.
        /// </summary>
        IEnumerable<RewardBase> Rewards { get; }
    }
}