namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines the ShipmentMethodTypeField
    /// </summary>
    public interface IShipmentMethodTypeField : ITypeFieldMapper<ShipMethodType>
    {
        /// <summary>
        /// The flat rate type
        /// </summary>
        ITypeField FlatRate { get; }

        /// <summary>
        /// The percent total type
        /// </summary>
        ITypeField PercentTotal { get; }

        /// <summary>
        /// The carrier type
        /// </summary>
        ITypeField Carrier { get; }
    }
}