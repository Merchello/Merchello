using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a notification
    /// </summary>
    internal interface INotification : IEntity
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
        /// The path to the src
        /// </summary>
        [DataMember]
        string Src { get; set; }

        /// <summary>
        /// The notifiy status key
        /// </summary>
        [DataMember]
        Guid StatusKey { get; set; }

        /// <summary>
        /// The recipients of the notification
        /// </summary>
        string Recipients { get; set; }

        /// <summary>
        /// True/false indicating whether or not this notification should be sent to the customer
        /// </summary>
        bool SendToCustomer { get; set; }

        /// <summary>
        /// True/false indicating whether or not this notification is disabled
        /// </summary>
        bool Disabled { get; set; }


    }
}