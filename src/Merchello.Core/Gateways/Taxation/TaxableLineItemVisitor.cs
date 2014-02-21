using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    internal class TaxableLineItemVisitor : ILineItemVisitor
    {
        private readonly List<ILineItem> _lineItems = new List<ILineItem>();

        public void Visit(ILineItem lineItem)
        {
            if (lineItem.ExtendedData.GetTaxableValue())
            {
                _lineItems.Add(lineItem);
            }
        }

        /// <summary>
        /// Gets the line items identified as taxable line items
        /// </summary>
        public IEnumerable<ILineItem> TaxableLineItems
        {
            get { return _lineItems; }
        }
    }
}