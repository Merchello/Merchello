using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{

    /// <summary>
    /// Used in basket and invoice (product) items to identify if a specific shipping method is required.
    /// </summary>
    internal sealed class ShipMethodTypeField :TypeFieldMapper<ShipMethodType>, IShipmentMethodTypeField
    {
        internal ShipMethodTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        internal override void BuildCache()
        {

            // Ship Method Types
            AddUpdateCache(ShipMethodType.FlatRate, new TypeField("FlatRate", "Flat Rate", new Guid("1D0B73CF-AE9D-4501-83F5-FA0B2FEE1236")));
            AddUpdateCache(ShipMethodType.PercentTotal, new TypeField("PercentTotal", "Percent of Total", new Guid("B056DA45-3FB0-49AE-8349-6FCEB1465DF6")));
            AddUpdateCache(ShipMethodType.Carrier, new TypeField("Carrier", "Carrier", new Guid("4311536A-9554-43D4-8422-DEAAD214B469")));
        }

        /// <summary>
        /// Returns a custom shipment methods or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom shipment method</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(ShipMethods[alias]);
        }

        /// <summary>
        /// Flat rate shipping
        /// </summary>
        public ITypeField FlatRate
        {
            get { return GetTypeField(ShipMethodType.FlatRate); }
        }

        /// <summary>
        /// Shipping to be calculated as a percent of total invoice
        /// </summary>
        public ITypeField PercentTotal
        {
            get { return GetTypeField(ShipMethodType.PercentTotal); }
        }

        /// <summary>
        /// Carrier based shipping (UPS, USPS, ...)
        /// </summary>
        public ITypeField Carrier
        {
            get { return GetTypeField(ShipMethodType.Carrier); }
        }

        private static TypeFieldCollection ShipMethods
        {
            get { return Fields.ShipMethod; }
        }


    }
}
