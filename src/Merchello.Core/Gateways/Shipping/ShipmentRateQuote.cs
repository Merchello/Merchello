namespace Merchello.Core.Gateways.Shipping
{
    using Models;

    /// <summary>
    /// Represents a shipment rate quote
    /// </summary>
    public class ShipmentRateQuote : IShipmentRateQuote
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentRateQuote"/> class.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="shipMethod">
        /// The ship method.
        /// </param>
        public ShipmentRateQuote(IShipment shipment, IShipMethod shipMethod)
            : this(shipment, shipMethod, new ExtendedDataCollection())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentRateQuote"/> class.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="shipMethod">
        /// The ship method.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public ShipmentRateQuote(IShipment shipment, IShipMethod shipMethod, ExtendedDataCollection extendedData)
        {
            Mandate.ParameterNotNull(shipment, "shipment");
            Mandate.ParameterNotNull(shipMethod, "shipMethod");
            Mandate.ParameterNotNull(extendedData, "extendedData");

            shipment.ShipMethodKey = shipMethod.Key;

            Shipment = shipment;
            ShipMethod = shipMethod;
            ExtendedData = extendedData;

            this.Initialize();
        }

        /// <summary>
        /// Gets the <see cref="IShipment"/> associated with this rate quote
        /// </summary>
        public IShipment Shipment { get; private set; }

        /// <summary>
        /// Gets the ShipMethod used to quote the rate
        /// </summary>
        public IShipMethod ShipMethod { get; private set; }

        /// <summary>
        /// Gets or sets the rate quoted by the ShipMethod
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets the extended data collection to store any additional information returned from
        /// a carrier based shipping.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; private set; }

        /// <summary>
        /// Initializes values
        /// </summary>
        private void Initialize()
        {
            http://issues.merchello.com/youtrack/issue/M-458
            Shipment.ShipMethodKey = ShipMethod.Key;
        }
    }
}