namespace Merchello.Core.Marketing.Coupons
{
    using Merchello.Core.Models;

    /// <summary>
    /// A base class used for defining coupon campaign activities.
    /// </summary>
    public abstract class CouponBase : CampaignActivityBase, IDiscountActivity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponBase"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        protected CouponBase(CampaignActivitySettings settings)
            : base(settings)
        {
        }
    }
}