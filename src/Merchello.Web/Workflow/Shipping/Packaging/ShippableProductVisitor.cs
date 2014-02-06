using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Workflow.Shipping.Packaging
{
    /// <summary>
    /// Line item visitor intended to filter "Basket" items for shippable products
    /// </summary>
    public class ShippableProductVisitor : ILineItemVisitor
    {
        private readonly List<ILineItem> _lineItems = new List<ILineItem>();
      
        public void Visit(ILineItem lineItem)
        {
            // For the first release we are going to assume everything shippable is a product listed in the Merchello catalog
            if (lineItem.ExtendedData.ContainsProductVariantKey() && lineItem.ExtendedData.GetShippableValue() && lineItem.LineItemType == LineItemType.Product)
            {                
                _lineItems.Add(lineItem.ConvertToNewLineItemOf<OrderLineItem>());
            }
        }

        public IEnumerable<ILineItem> ShippableItems 
        {
            get { return _lineItems; }
        } 
    }
}