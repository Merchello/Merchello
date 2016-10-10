namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a payment method
    /// </summary>
    public interface IPaymentMethod : IGatewayProviderMethod
    {
        /// <summary>
        /// Gets the key associated with the gateway provider for the payment
        /// </summary>
        Guid ProviderKey { get; }

        /// <summary>
        /// Gets or sets the name of the payment method
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the payment method
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the payment code of the payment method
        /// </summary>
        string PaymentCode { get; set; }
    }
}