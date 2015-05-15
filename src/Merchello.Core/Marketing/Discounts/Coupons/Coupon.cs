namespace Merchello.Core.Marketing.Discounts.Coupons
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Discounts;
    using Merchello.Core.Marketing.Discounts.Offer;
    using Merchello.Core.Marketing.Discounts.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a discount Coupon.
    /// </summary>
    public class Coupon : DiscountOfferBase, ICoupon
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coupon"/> class.
        /// </summary>
        /// <param name="reward">
        /// The <see cref="IDiscountReward"/>.
        /// </param>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        public Coupon(IDiscountReward reward, IEnumerable<IDiscountConstraint> constraints)
            : base(reward, constraints)
        {
        }

        /// <summary>
        /// Gets or sets the redeem code.
        /// </summary>
        public string RedeemCode { get; set; }

        /// <summary>
        /// Applies the discount to the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/> indicating whether or not the application was successful.
        /// </returns>
        protected override Attempt<IDiscountReward> DoApplyReward(ILineItemContainer collection)
        {
            throw new NotImplementedException();
        }
    }
}