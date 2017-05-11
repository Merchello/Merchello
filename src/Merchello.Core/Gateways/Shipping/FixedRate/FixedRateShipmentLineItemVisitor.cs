namespace Merchello.Core.Gateways.Shipping.FixedRate
{
    using Merchello.Core.Models;

    /// <summary>
    /// Visitor class that calculates 
    /// </summary>
    public class FixedRateShipmentLineItemVisitor : ILineItemVisitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRateShipmentLineItemVisitor"/> class.
        /// </summary>
        public FixedRateShipmentLineItemVisitor()
        {
            TotalPrice = 0M;
            TotalWeight = 0M;
            UseOnSalePriceIfOnSale = false;
        }

        /// <summary>
        /// Gets a value the TotalWeight from ExtendedData
        /// </summary>
        public decimal TotalWeight { get; private set; }

        /// <summary>
        /// Gets a value the TotalPrice form ExtendedData
        /// </summary>
        public decimal TotalPrice { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use the OnSale price in the total price calculation
        /// </summary>
        public bool UseOnSalePriceIfOnSale { get; set; }

        /// <summary>
        /// Visits the line item.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            if (!lineItem.ExtendedData.DefinesProductVariant()) return;

            // adjust the total weight
            TotalWeight += lineItem.ExtendedData.GetWeightValue() * lineItem.Quantity;

            // adjust the total price
            if (UseOnSalePriceIfOnSale)
            {
                TotalPrice += lineItem.ExtendedData.GetOnSaleValue()
                    ? lineItem.ExtendedData.GetSalePriceValue() * lineItem.Quantity
                    : lineItem.ExtendedData.GetPriceValue() * lineItem.Quantity;
            }
            else
            {
                TotalPrice += lineItem.ExtendedData.GetPriceValue() * lineItem.Quantity;
            }
        }
    }
}
