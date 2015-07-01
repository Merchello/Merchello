namespace Merchello.Web.Discounts.Coupons
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    /// <summary>
    /// Marker interface for the CouponRedemptionResult.
    /// </summary>
    public interface ICouponRedemptionResult : IOfferRedemptionResult<ILineItem>
    {
        /// <summary>
        /// Gets the coupon.
        /// </summary>
        ICoupon Coupon { get;  }
    }
}