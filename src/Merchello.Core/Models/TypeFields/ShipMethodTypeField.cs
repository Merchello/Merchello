using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
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
            get { return Constants.ShipMethodType.FlatRate; }
        }

        /// <summary>
        /// Shipping to be calculated as a percent of total invoice
        /// </summary>
        public static ITypeField PercentTotal
        {
            get { return Constants.ShipMethodType.PercentTotal; }
        }

        /// <summary>
        /// Carrier based shipping (UPS, USPS, ...)
        /// </summary>
        public static ITypeField Carrier
        {
            get { return Constants.ShipMethodType.Carrier; }
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
