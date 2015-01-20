namespace Merchello.Web.Models.ContentEditing
{
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Gateways.Shipping;

    /// <summary>
    /// The shipping gateway method display.
    /// </summary>
    public class ShippingGatewayMethodDisplay : DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the <see cref="GatewayResourceDisplay"/>.
        /// </summary>
        public GatewayResourceDisplay GatewayResource { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ShipCountryDisplay"/>.
        /// </summary>
        public ShipCountryDisplay ShipCountry { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ShipMethodDisplay"/>.
        /// </summary>
        public ShipMethodDisplay ShipMethod { get; set; }
    }

    #region Mapping extensions

    /// <summary>
    /// The shipping gateway method display extensions.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ShippingGatewayMethodDisplayExtensions
    {
        /// <summary>
        /// Maps a <see cref="ShippingGatewayMethodBase"/> to <see cref="ShippingGatewayMethodDisplay"/>.
        /// </summary>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <returns>
        /// The <see cref="ShippingGatewayMethodDisplay"/>.
        /// </returns>
        internal static ShippingGatewayMethodDisplay ToShippingGatewayMethodDisplay(this IShippingGatewayMethod method)
        {
            return AutoMapper.Mapper.Map<ShippingGatewayMethodDisplay>(method);
        }
    }

    #endregion
}