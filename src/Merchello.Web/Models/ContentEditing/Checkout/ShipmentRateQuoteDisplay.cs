namespace Merchello.Web.Models.ContentEditing.Checkout
{
    using System.Collections.Generic;

    using Merchello.Core.Gateways.Shipping;

    /// <summary>
    /// A model for shipment rate quotes in the back office.
    /// </summary>
    public class ShipmentRateQuoteDisplay
    {
        /// <summary>
        /// Gets or sets the ship method.
        /// </summary>
        public ShipMethodDisplay ShipMethod { get; set; }

        /// <summary>
        /// Gets or sets the shipment.
        /// </summary>
        public ShipmentDisplay Shipment { get; set; }

        /// <summary>
        /// Gets or sets the rate.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }
    }

    /// <summary>
    /// Maps a shipment rate quote.
    /// </summary>
    internal static class ShipmentRateQuoteDisplayExtensions
    {
        /// <summary>
        /// The to shipment rate quote display.
        /// </summary>
        /// <param name="quote">
        /// The quote.
        /// </param>
        /// <returns>
        /// The <see cref="ShipmentRateQuoteDisplay"/>.
        /// </returns>
        public static ShipmentRateQuoteDisplay ToShipmentRateQuoteDisplay(this IShipmentRateQuote quote)
        {
            return AutoMapper.Mapper.Map<ShipmentRateQuoteDisplay>(quote);
        }
    }
}