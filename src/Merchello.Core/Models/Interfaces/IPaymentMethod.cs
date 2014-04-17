using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a payment method
    /// </summary>
    public interface IPaymentMethod : IGatewayProviderMethod
    {
        /// <summary>
        /// The key associated with the gateway provider for the payment
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; }

        /// <summary>
        /// The name of the payment method
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// The description of the payment method
        /// </summary>
        [DataMember]
        string Description { get; set; }

        /// <summary>
        /// The payment code of the payment method
        /// </summary>
        [DataMember]
        string PaymentCode { get; set; }
    }
}