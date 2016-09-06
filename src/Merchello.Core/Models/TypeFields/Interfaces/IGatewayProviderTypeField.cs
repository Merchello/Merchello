namespace Merchello.Core.Models.TypeFields
{
    using Merchello.Core.Models.TypeFields.Interfaces;

    /// <summary>
    /// Represents a gateway provider type field
    /// </summary>
    public interface IGatewayProviderTypeField : ITypeFieldMapper<GatewayProviderType>
    {
        /// <summary>
        /// Gets the <see cref="ITypeField"/> for the payment providers
        /// </summary>
        ITypeField Payment { get; }

        /// <summary>
        /// Gets the <see cref="ITypeField"/> for the notification providers
        /// </summary>
        ITypeField Notification { get; }

        /// <summary>
        /// Gets the <see cref="ITypeField"/> for shipping providers
        /// </summary>
        ITypeField Shipping { get; }


        /// <summary>
        /// Gets the <see cref="ITypeField"/> for the taxation providers
        /// </summary>
        ITypeField Taxation { get; }
    }
}