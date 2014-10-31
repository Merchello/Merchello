namespace Merchello.Core.Strategies.Packaging
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// Line item visitor intended to filter "Basket" items for shippable products
    /// </summary>
    public class ShippableProductVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The _line items.
        /// </summary>
        private readonly List<ILineItem> _lineItems = new List<ILineItem>();

        /// <summary>
        /// Gets the shippable items.
        /// </summary>
        public IEnumerable<ILineItem> ShippableItems
        {
            get { return _lineItems; }
        } 

        /// <summary>
        /// The visit.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            // For the first release we are going to assume everything shippable is a product listed in the Merchello catalog
            if (lineItem.ExtendedData.ContainsProductVariantKey() && lineItem.ExtendedData.GetShippableValue() && lineItem.LineItemType == LineItemType.Product)
            {                
                _lineItems.Add(lineItem.AsLineItemOf<OrderLineItem>());
            }
        }
    }
}