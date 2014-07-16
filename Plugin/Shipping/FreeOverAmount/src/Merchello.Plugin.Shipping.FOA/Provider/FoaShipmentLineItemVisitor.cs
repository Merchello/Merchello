using System;
using Merchello.Core.Models;

namespace Merchello.Plugin.Shipping.FOA.Provider
{
    public class FoaShipmentLineItemVisitor : ILineItemVisitor
    {
        public void Visit(ILineItem lineItem)
        {
            if (!lineItem.ExtendedData.DefinesProductVariant()) return;

            // adjust the total price
            if (UseOnSalePriceIfOnSale)
            {
                TotalPrice += lineItem.ExtendedData.GetOnSaleValue()
                    ? lineItem.ExtendedData.GetSalePriceValue()
                    : lineItem.ExtendedData.GetPriceValue();
            }
            else
            {
                TotalPrice += lineItem.ExtendedData.GetPriceValue() * lineItem.Quantity;
            }
        }

        public decimal TotalPrice { get; set; }

        /// <summary>
        /// True/false indicating whether or not to use the OnSale price in the total price calculation
        /// </summary>
        public bool UseOnSalePriceIfOnSale { get; set; }
    }
}