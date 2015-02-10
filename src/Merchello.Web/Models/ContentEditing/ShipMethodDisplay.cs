namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;

    /// <summary>
    /// The ship method display.
    /// </summary>
    public class ShipMethodDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the provider key.
        /// </summary>
        public Guid ProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the ship country key.
        /// </summary>
        public Guid ShipCountryKey { get; set; }

        /// <summary>
        /// Gets or sets the surcharge.
        /// </summary>
        public decimal Surcharge { get; set; }

        /// <summary>
        /// Gets or sets the service code.
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether taxable.
        /// </summary>
        public bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets the provinces.
        /// </summary>
        public IEnumerable<ShipProvinceDisplay> Provinces { get; set; }
    }

    #region Mapping extensions

    /// <summary>
    /// The ship method display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ShipMethodDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="IShippingGatewayMethod"/> to <see cref="ShipMethodDisplay"/>
        /// </summary>
        /// <param name="shipMethod">
        /// The shipping gateway method.
        /// </param>
        /// <returns>
        /// The <see cref="ShipMethodDisplay"/>
        /// </returns>

        internal static ShipMethodDisplay ToShipMethodDisplay(this IShipMethod shipMethod)
        {
            return AutoMapper.Mapper.Map<ShipMethodDisplay>(shipMethod);
        }

        /// <summary>
        /// Maps <see cref="IShippingGatewayMethod"/> to <see cref="ShipMethodDisplay"/>
        /// </summary>
        /// <param name="shippingGatewayMethod">
        /// The shipping gateway method.
        /// </param>
        /// <returns>
        /// The <see cref="ShipMethodDisplay"/>
        /// </returns>
        internal static ShipMethodDisplay ToShipMethodDisplay(this IShippingGatewayMethod shippingGatewayMethod)
        {
            return AutoMapper.Mapper.Map<ShipMethodDisplay>(shippingGatewayMethod);
        }
    }

    #endregion
}
