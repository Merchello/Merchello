namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using Lucene.Net.Index;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The coupon discount line item reward visitor.
    /// </summary>
    internal class CouponDiscountLineItemRewardVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The amount configured in the reward
        /// </summary>
        private readonly decimal _amount;

        /// <summary>
        /// The max quantity configured in the reward.
        /// </summary>
        private readonly int _maxQuantity;

        /// <summary>
        /// The type of adjustment configured in the reward.
        /// </summary>
        private readonly CouponDiscountLineItemReward.Adjustment _adjustment;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponDiscountLineItemRewardVisitor"/> class.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="maxQuantity">
        /// The max quantity.
        /// </param>
        /// <param name="adjustment">
        /// The adjustment.
        /// </param>
        public CouponDiscountLineItemRewardVisitor(
            decimal amount,
            int maxQuantity,
            CouponDiscountLineItemReward.Adjustment adjustment)
        {
            Discount = 0;
            _amount = amount;
            _maxQuantity = maxQuantity;
            _adjustment = adjustment;
        }

        /// <summary>
        /// Gets the discount.
        /// </summary>
        public decimal Discount { get; private set; }

        /// <summary>
        /// Visits the line item.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            // only applies to products
            if (lineItem.LineItemType != LineItemType.Product) return;

            var quantity = lineItem.Quantity;
            if (_maxQuantity > 0 && quantity > _maxQuantity) quantity = _maxQuantity;

            // this could be different than the total line item price if the max quantity for
            // the reward is lower than the actual quantity of the line item
            var adjustedPrice = quantity * lineItem.Price;

            Discount += _adjustment == CouponDiscountLineItemReward.Adjustment.Flat
                            ? _amount * quantity
                            : adjustedPrice * _amount / 100;
        }

    }
}