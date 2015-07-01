namespace Merchello.Web.Discounts.Coupons
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines a Coupon.
    /// </summary>
    public interface ICoupon : IOffer
    {
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
        Attempt<IOfferResult<ILineItemContainer, ILineItem>> TryApply(ILineItemContainer container, ICustomerBase customer); 
            
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
        Attempt<IOfferResult<TConstraint, TReward>> TryApply<TConstraint, TReward>(TConstraint validateAgainst, ICustomerBase customer)
            where TConstraint : class
            where TReward : class;
    }
}