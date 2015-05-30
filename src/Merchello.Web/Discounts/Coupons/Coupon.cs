namespace Merchello.Web.Discounts.Coupons
{
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Discounts;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a discount Coupon.
    /// </summary>
    public class Coupon : OfferBase, ICoupon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coupon"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IOfferSettings"/>.
        /// </param>
        public Coupon(IOfferSettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the collection discount constraints.
        /// </summary>
        public IEnumerable<OfferConstraintComponentBase> Constraints { get; private set; }

        /// <summary>
        /// Gets the collection offer rewards.
        /// </summary>
        public IEnumerable<RewardBase> Rewards { get; private set; }
    }
}