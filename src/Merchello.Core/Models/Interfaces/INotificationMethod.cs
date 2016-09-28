namespace Merchello.Core.Models
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a notification method.
    /// </summary>
    public interface INotificationMethod : IEntity
    {
        /// <summary>
        /// Gets or sets the name of the notification
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a brief description of the notification
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderSettings"/> key
        /// </summary>
        Guid ProviderKey { get; }

        /// <summary>
        /// Gets or sets the service code
        /// </summary>
        string ServiceCode { get; set; }
    }
}