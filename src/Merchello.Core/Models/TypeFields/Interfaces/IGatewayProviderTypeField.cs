namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines the provider gateway type field
    /// </summary>
    public interface IGatewayProviderTypeField : ITypeFieldMapper<GatewayProviderType>
    {
        /// <summary>
        /// The <see cref="ITypeField"/> for the payment providers
        /// </summary>
        ITypeField Payment { get; }

        /// <summary>
        /// The <see cref="ITypeField"/> for the notification providers
        /// </summary>
        ITypeField Notification { get; }

        /// <summary>
        /// The <see cref="ITypeField"/> for shipping providers
        /// </summary>
        ITypeField Shipping { get; }


        /// <summary>
        /// The <see cref="ITypeField"/> for the taxation providers
        /// </summary>
        ITypeField Taxation { get; }
    }
}