namespace Merchello.Web.Models.Coupons
{
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Discounts.Coupons.Constraints;
    using Merchello.Core.Marketing.Offer;

    /// <summary>
    /// Defines a Coupon.
    /// </summary>
    public interface ICoupon : IOffer
    {
        /// <summary>
        /// Gets the constraints.
        /// </summary>
        IEnumerable<DiscountConstraintBase> Constraints { get; }
    }
}