namespace Merchello.Core.Gateways.Taxation
{
    using System.Collections.Generic;
    using System.Globalization;

    using Merchello.Core.Models;

    /// <summary>
    /// A visitor class to identify taxable line items.
    /// </summary>
    internal class TaxableLineItemVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The line items identified as taxable.
        /// </summary>
        private readonly List<ILineItem> _lineItems = new List<ILineItem>();

        /// <summary>
        /// The tax rate to be applied to the line item.
        /// </summary>
        private readonly decimal _taxRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxableLineItemVisitor"/> class.
        /// </summary>
        /// <param name="taxRate">
        /// The tax rate.
        /// </param>
        public TaxableLineItemVisitor(decimal taxRate)
        {
            _taxRate = taxRate > 1 ? taxRate / 100 : taxRate;
        }

        /// <summary>
        /// Gets the line items identified as taxable line items
        /// </summary>
        public IEnumerable<ILineItem> TaxableLineItems
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
            if (!lineItem.ExtendedData.GetTaxableValue()) return;
            if (lineItem.LineItemType == LineItemType.Discount)
            {
                lineItem.ExtendedData.SetValue(Constants.ExtendedDataKeys.LineItemTaxAmount, (-lineItem.TotalPrice * this._taxRate).ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                lineItem.ExtendedData.SetValue(Constants.ExtendedDataKeys.LineItemTaxAmount, (lineItem.TotalPrice * this._taxRate).ToString(CultureInfo.InvariantCulture));
            }
            lineItem.ExtendedData.SetValue(Constants.ExtendedDataKeys.BaseTaxRate, this._taxRate.ToString());
            _lineItems.Add(lineItem);
        }
    }
}