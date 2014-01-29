using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents a shipment rate quote
    /// </summary>
    public class ShipmentRateQuote : IShipmentRateQuote
    {

        public ShipmentRateQuote(IShipMethod shipMethod)
            : this(shipMethod, new ExtendedDataCollection())
        { }

        public ShipmentRateQuote(IShipMethod shipMethod, ExtendedDataCollection extendedData)
        {
            Mandate.ParameterNotNull(shipMethod, "shipMethod");
            Mandate.ParameterNotNull(extendedData, "extendedData");

            ShipMethod = shipMethod;
            ExtendedData = extendedData;
        }

        /// <summary>
        /// The ShipMethod used to quote the rate
        /// </summary>
        public IShipMethod ShipMethod { get; private set; }

        /// <summary>
        /// The rate quoted by the ShipMethod
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// An extended data collection to store any additional information returned from
        /// a carrier based shipping.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; private set; }

    }
}