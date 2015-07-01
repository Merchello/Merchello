namespace Merchello.Web.Discounts.Coupons
{
    using System;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The coupon redemption result extensions.
    /// </summary>
    public static class CouponRedemptionResultExtensions
    {
        /// <summary>
        /// Maps an <see cref="IOfferResult{TConstraint,TAward}"/> to <see cref="ICouponRedemptionResult"/>
        /// </summary>
        /// <param name="attempt">
        /// The attempt.
        /// </param>
        /// <param name="coupon">
        /// The coupon.
        /// </param>
        /// <returns>
        /// The <see cref="ICouponRedemptionResult"/>.
        /// </returns>
        public static ICouponRedemptionResult AsCouponRedemptionResult(this Attempt<IOfferResult<ILineItemContainer, ILineItem>> attempt, ICoupon coupon = null)
        {
            var result = attempt.Success
                             ? new CouponRedemptionResult(attempt.Result.Award, attempt.Result.Messages)
                             : new CouponRedemptionResult(
                                   attempt.Exception,
                                   attempt.Result != null ? attempt.Result.Messages : null);
            result.Coupon = coupon;
            return result;
        }  
    }
}