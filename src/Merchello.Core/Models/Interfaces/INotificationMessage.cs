using System;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a notification message
    /// </summary>
    public interface INotificationMessage : IEntity
    {
        /// <summary>
        /// Optional key for Notification Trigger Rule
        /// </summary>
        [DataMember]
        Guid? RuleKey { get; set; }

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
        /// The path or text src
        /// </summary>
        [DataMember]
        string Message { get; set; }

        /// <summary>
        /// The maximum length of the message to be sent
        /// </summary>
        [DataMember]
        int MaxLength { get; set; }

        /// <summary>
        /// True/false indicating whether or not the string value of Message is actually a path to a file to read
        /// </summary>
        [DataMember]
        bool MessageIsFilePath { get; set; }

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