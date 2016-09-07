namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Represents an GatewayProviderTypeField
    /// </summary>
    public interface IGatewayProviderTypeField : ITypeFieldMapper<GatewayProviderType>
    {
        /// <summary>
        /// Gets the payment <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Payment { get; }

        /// <summary>
        /// Gets the notification <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Notification { get; }

        /// <summary>
        /// Gets the shipping <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Shipping { get; }

        /// <summary>
        /// Gets the taxation <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Taxation { get; }
    }
}