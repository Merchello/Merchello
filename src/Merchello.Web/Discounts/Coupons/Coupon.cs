namespace Merchello.Web.Discounts.Coupons
{
    using System;

    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

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
        /// Tries to apply the coupon
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<IOfferResult<ILineItemContainer, ILineItem>> TryApply(ILineItemContainer container, ICustomerBase customer)
        {
            return TryApply<ILineItemContainer, ILineItem>(container, customer);
        }

        /// <summary>
        /// Tries to apply the coupon
        /// </summary>
        /// <param name="validateAgainst">
        /// The validate against.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TReward">
        /// The type of reward
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<IOfferResult<TConstraint, TReward>> TryApply<TConstraint, TReward>(TConstraint validateAgainst, ICustomerBase customer)
            where TConstraint : class
            where TReward : class
        {
            var noReward = new OfferRedemptionException("The reward for this coupon has not been configured.");
            
            return this.Reward == null ? 
                Attempt<IOfferResult<TConstraint, TReward>>.Fail(noReward) : 
                this.TryToAward<TConstraint, TReward>(validateAgainst, customer);
        }
    }
}