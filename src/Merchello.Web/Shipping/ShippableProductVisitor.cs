using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Shipping
{
    /// <summary>
    /// 
    /// </summary>
    public class ShippableProductVisitor : ILineItemVisitor
    {
        private readonly List<ILineItem> _lineItems = new List<ILineItem>();

        public void Visit(ILineItem lineItem)
        {
            // For the first release we are going to assume everything shippable is a product listed in the Merchello catalog
            if (lineItem.ExtendedData.ContainsProductVariantKey() && lineItem.ExtendedData.GetShippableValue() && lineItem.LineItemType == LineItemType.Product)
            {                
              _lineItems.Add(lineItem);
            }
        }

        public IEnumerable<ILineItem> ShippableItems 
        {
            get { return _lineItems; }
        } 
    }
}