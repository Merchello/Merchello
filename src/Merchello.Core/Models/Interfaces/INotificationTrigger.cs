using System;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a notification trigger
    /// </summary>
    internal interface INotificationTrigger : IEntity
    {
        /// <summary>
        /// The name of the trigger
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A unique string "binding" constructed from the ServiceName and Event to Listen For
        /// </summary>
        string Binding { get; }

        /// <summary>
        /// An optional reference key to further constrain the trigger
        /// </summary>
        /// <remarks>
        /// 
        /// eg.  InvoiceStatus.Key or OrderStatus.Key
        /// 
        /// </remarks>
        Guid? EntityKey { get; set; }
    }
}