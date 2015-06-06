namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using System.Collections.Generic;
    using System.Linq;

    using Lucene.Net.Index;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The coupon discount line item reward visitor.
    /// </summary>
    internal class CouponDiscountLineItemRewardVisitor : ILineItemVisitor
    {
        
        /// <summary>
        /// The max quantity configured in the reward.
        /// </summary>
        private readonly int _maxQuantity;

        /// <summary>
        /// The list of matching items.
        /// </summary>
        private readonly List<ILineItem> _filteredItems = new List<ILineItem>();

        /// <summary>
        /// The all line items.
        /// </summary>
        private readonly List<ILineItem> _allLineItems = new List<ILineItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponDiscountLineItemRewardVisitor"/> class.
        /// </summary>
        /// <param name="maxQuantity">
        /// The max quantity.
        /// </param>
        public CouponDiscountLineItemRewardVisitor(int maxQuantity)
        {
            _maxQuantity = maxQuantity;
        }

        /// <summary>
        /// Gets the filtered items.
        /// </summary>
        public IEnumerable<ILineItem> FilteredItems
        {
            get
            {
                return this._filteredItems; 
            }
        }

        /// <summary>
        /// Gets the modified collection of all line items.
        /// </summary>
        public IEnumerable<ILineItem> AllLineItems
        {
            get
            {
                return this._allLineItems;
            }
        }

        /// <summary>
        /// Visits the line item.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            var quantity = lineItem.Quantity;
            if (_maxQuantity > 0 && quantity > _maxQuantity) quantity = _maxQuantity;

            var resultingLineItem = lineItem.AsLineItemOf<InvoiceLineItem>();
            
            // this could be different than the total line item price if the max quantity for
            // the reward is lower than the actual quantity of the line item
            resultingLineItem.Quantity = quantity;

            _allLineItems.Add(resultingLineItem);

            // only applies to products
            if (resultingLineItem.LineItemType != LineItemType.Product) return;

            this._filteredItems.Add(lineItem);

        }

    }
}