namespace Merchello.Core.Marketing.Discounts.Coupons
{
    using Merchello.Core.Discounts;
    using Merchello.Core.Marketing.Discounts.Offer;

    /// <summary>
    /// Defines a Coupon.
    /// </summary>
    public interface ICoupon : IDiscountOffer
    {
        /// <summary>
        /// Gets or sets the redeem code.
        /// </summary>
        string RedeemCode { get; set; }
    }
}