namespace Merchello.Web.Workflow
{
    using Merchello.Core.Sales;
    using Merchello.Web.Discounts.Coupons;

    /// <summary>
    /// Marker interface for <see cref="IBasket"/> based checkouts
    /// </summary>
    public interface IBasketSalePreparation : ISalePreparationBase
    {
        /// <summary>
        /// Attempts to add a coupon offer to the sale.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="ICouponRedemptionResult"/>.
        /// </returns>
        ICouponRedemptionResult RedeemCouponOffer(string offerCode);
    }
}