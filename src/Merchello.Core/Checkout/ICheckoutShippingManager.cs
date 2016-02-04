namespace Merchello.Core.Checkout
{
    using System.Collections.Generic;

    using Merchello.Core.Gateways.Shipping;

    /// <summary>
    /// Defines a CheckoutShippingManager
    /// </summary>
    public interface ICheckoutShippingManager
    {
        /// <summary>
        /// Saves a <see cref="IShipmentRateQuote"/> as a shipment line item
        /// </summary>
        /// <param name="approvedShipmentRateQuote">
        /// The <see cref="IShipmentRateQuote"/> to be saved
        /// </param>
        void SaveShipmentRateQuote(IShipmentRateQuote approvedShipmentRateQuote);

        /// <summary>
        /// Saves a collection of <see cref="IShipmentRateQuote"/>s as shipment line items
        /// </summary>
        /// <param name="approvedShipmentRateQuotes">
        /// The collection of <see cref="IShipmentRateQuote"/>s to be saved
        /// </param>
        void SaveShipmentRateQuote(IEnumerable<IShipmentRateQuote> approvedShipmentRateQuotes);

        /// <summary>
        /// Clears all <see cref="IShipmentRateQuote"/>s previously saved
        /// </summary>
        void ClearShipmentRateQuotes();
    }
}