using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    public interface INotificationMethod : IEntity
    {
        /// <summary>
        /// The name of the notification
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// A brief description of the notification
        /// </summary>
        [DataMember]
        string Description { get; set; }

        /// <summary>
        /// The <see cref="IGatewayProviderSettings"/> key
        /// </summary>
        [DataMember]
        Guid ProviderKey { get; }

        /// <summary>
        /// The service code
        /// </summary>
        [DataMember]
        string ServiceCode { get; set; }
    }
}