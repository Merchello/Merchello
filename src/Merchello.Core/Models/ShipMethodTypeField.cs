using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Used in basket and invoice (product) items to identify if a specific shipping method is required.
    /// </summary>
    public class ShipMethodTypeField :TypeFieldProxyBase
    {
        /// <summary>
        /// Flat rate shipping
        /// </summary>
        public static ITypeField FlatRate
        {
            get { return GetTypeField(ShipMethods["FlatRate"]); }
        }

        /// <summary>
        /// Shipping to be calculated as a percent of total invoice
        /// </summary>
        public static ITypeField PercentTotal
        {
            get { return GetTypeField(ShipMethods["PercentTotal"]); }
        }

        /// <summary>
        /// Carrier based shipping (UPS, USPS, ...)
        /// </summary>
        public static ITypeField Carrier
        {
            get { return GetTypeField(ShipMethods["Carrier"]); }
        }

        /// <summary>
        /// Returns a custom shipment methods or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom shipment method</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        public new static ITypeField Custom(string alias)
        {
            return GetTypeField(ShipMethods[alias]);
        }

        private static TypeFieldCollection ShipMethods
        {
            get { return Fields.ShipMethod; }
        }
    }
}
