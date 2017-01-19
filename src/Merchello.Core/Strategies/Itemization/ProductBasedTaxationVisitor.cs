namespace Merchello.Core.Strategies.Itemization
{
    using System.Collections.Generic;
    using System.Globalization;

    using Merchello.Core.Models;

    /// <summary>
    /// Represents a visitor that separates taxes included in line items for that were included in product pricing.
    /// </summary>
    public class ProductBasedTaxationVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The product line items.
        /// </summary>
        private readonly List<ILineItem> _productLineItems = new List<ILineItem>();

        /// <summary>
        /// The tax line items.
        /// </summary>
        private readonly List<ILineItem> _taxLineItems = new List<ILineItem>();


        /// <summary>
        /// Gets the product line items without taxes included.
        /// </summary>
        public IEnumerable<ILineItem> ProductLineItems
        {
            get { return this._productLineItems; }
        }

        /// <summary>
        /// Gets the tax line items created based on taxes that were included in the product pricing.
        /// </summary>
        public IEnumerable<ILineItem> TaxLineItems
        {
            get { return this._taxLineItems; }  
        } 

        /// <summary>
        /// Visits a line items.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Visit(ILineItem item)
        {
            // We only need product line items
            if (item.LineItemType != LineItemType.Product && item.LineItemType != LineItemType.Tax) return;

            // Just add Tax items are list for return
            if (item.LineItemType == LineItemType.Tax)
            {
                _taxLineItems.Add(item.AsLineItemWithKeyOf<InvoiceLineItem>());
                return;
            }

            // Ensure this item has taxes included in product pricing.
            var taxesIncludedInPrice = bool.Parse(item.ExtendedData.GetValue(Constants.ExtendedDataKeys.TaxIncludedInProductPrice));
            if (!taxesIncludedInPrice)
            {
                _productLineItems.Add(item.AsLineItemWithKeyOf<InvoiceLineItem>());
                return;
            }

            // TODO merchCouponAdjustedProductPreTaxTotal

            // Determine whether or not this item was purchased was onsale
            var onsale = item.ExtendedData.GetOnSaleValue();

            var priceKey = onsale 
                            ? Constants.ExtendedDataKeys.ProductSalePriceNoTax
                            : Constants.ExtendedDataKeys.ProductPriceNoTax;
            var taxKey = onsale
                            ? Constants.ExtendedDataKeys.ProductSalePriceTaxAmount
                            : Constants.ExtendedDataKeys.ProductPriceTaxAmount;

            if (item.ExtendedData.ContainsKey(priceKey) && item.ExtendedData.ContainsKey(taxKey))
            {
                var clone = item.AsLineItemWithKeyOf<InvoiceLineItem>();
                clone.Price = ConvertToDecimal(item.ExtendedData.GetValue(priceKey));
                _productLineItems.Add(clone);

                var taxItem = new InvoiceLineItem(LineItemType.Tax, string.Format("{0} Tax", item.Name), string.Format("{0}-Tax", item.Sku), item.Quantity, ConvertToDecimal(item.ExtendedData.GetValue(taxKey)));
                _taxLineItems.Add(taxItem);
            }
        }



        /// <summary>
        /// The get decimal value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private static decimal ConvertToDecimal(string value)
        {
            decimal converted;
            return decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out converted) ? converted : 0;
        }
    }
}