namespace Merchello.Core.Discounts
{
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