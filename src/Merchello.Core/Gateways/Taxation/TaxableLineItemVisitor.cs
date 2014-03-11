using System.Collections.Generic;
using System.Globalization;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    internal class TaxableLineItemVisitor : ILineItemVisitor
    {
        private readonly List<ILineItem> _lineItems = new List<ILineItem>();
        private readonly decimal _taxRate;
        public TaxableLineItemVisitor(decimal taxRate)
        {
            _taxRate = taxRate > 1 ? taxRate/100 : taxRate;
        }

        public void Visit(ILineItem lineItem)
        {
            if (lineItem.ExtendedData.GetTaxableValue())
            {
                lineItem.ExtendedData.SetValue(Constants.ExtendedDataKeys.LineItemTaxAmount, (lineItem.TotalPrice * _taxRate).ToString(CultureInfo.InvariantCulture));
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