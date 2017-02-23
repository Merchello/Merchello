namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using Merchello.Core;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Base class for coupon constraints.
    /// </summary>
    public abstract class CouponConstraintBase : OfferConstraintComponentBase<ILineItemContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponConstraintBase"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        protected CouponConstraintBase(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Utility method to build a successful <see cref="Attempt{ILineItemCollection}"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        protected Attempt<ILineItemContainer> Success(ILineItemContainer value)
        {
            return Attempt<ILineItemContainer>.Succeed(value);
        }

        /// <summary>
        /// Utility method to build a failed <see cref="Attempt{ILineItemCollection}"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        protected Attempt<ILineItemContainer> Fail(ILineItemContainer value, string message)
        {
            var att = this.GetOfferComponentAttribute();
            var msg = string.Format("{0}: {1}", att.Name, message);
            return Attempt<ILineItemContainer>.Fail(value, new OfferRedemptionException(msg));
        }
    }
}