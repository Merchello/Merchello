namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The maximum quantity constraint visitor.
    /// </summary>
    internal class MaximumQuantityConstraintVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The maximum quantity.
        /// </summary>
        private readonly int _maximum;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaximumQuantityConstraintVisitor"/> class.
        /// </summary>
        /// <param name="max">
        /// The max.
        /// </param>
        public MaximumQuantityConstraintVisitor(int max)
        {
            _maximum = max;
            ModifiedItems = new List<ILineItem>();
        }

        /// <summary>
        /// Gets or sets the modified items.
        /// </summary>
        public List<ILineItem> ModifiedItems { get; set; } 

        /// <summary>
        /// Visits the line item an asserts the maximum quantity.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            if (lineItem.LineItemType == LineItemType.Product)
            {
                if (lineItem.Quantity > _maximum) lineItem.Quantity = _maximum;
                ModifiedItems.Add(lineItem);
            }
            else
            {
                ModifiedItems.Add(lineItem);
            }
        }
    }
}