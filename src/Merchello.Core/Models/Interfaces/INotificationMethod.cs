namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Defines a NotificationMethod.
    /// </summary>
    public interface INotificationMethod : IEntity
    {
        /// <summary>
        /// Gets or sets the name of the notification
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the brief description of the notification
        /// </summary>
        [DataMember]
        string Description { get; set; }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderSettings"/> key
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; }

        /// <summary>
        /// Gets or sets the service code
        /// </summary>
        [DataMember]
        string ServiceCode { get; set; }
    }
}