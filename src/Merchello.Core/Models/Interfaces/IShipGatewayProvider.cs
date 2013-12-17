namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Defines a shipping gateway provider
    /// </summary>
    public interface IShipGatewayProvider : IGatewayProvider
    {
        /// <summary>
        /// The ship methods associated with the provider
        /// </summary>
        ShipMethodCollection ShipMethods { get; set; }

    }
}