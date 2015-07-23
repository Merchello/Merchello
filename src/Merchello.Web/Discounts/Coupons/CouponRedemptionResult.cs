namespace Merchello.Web.Discounts.Coupons
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    /// <summary>
    /// The result that is in an attempt to apply a coupon to a sale.
    /// </summary>
    public class CouponRedemptionResult : OfferRedemptionResultBase<ILineItem>, ICouponRedemptionResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponRedemptionResult"/> class for success
        /// </summary>
        /// <param name="award">
        /// The award.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        public CouponRedemptionResult(ILineItem award, IEnumerable<string> messages = null)
            : base(award, messages)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponRedemptionResult"/> class for failure
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="messages">
        /// The messages.
        /// </param>
        public CouponRedemptionResult(Exception exception, IEnumerable<string> messages = null)
            : base(exception, messages)
        {
        }

        /// <summary>
        /// Gets or sets the coupon.
        /// </summary>
        public ICoupon Coupon { get; internal set; }
    }
}