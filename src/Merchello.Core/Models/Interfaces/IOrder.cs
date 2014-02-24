using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines an Order
    /// </summary>
    public interface IOrder : ILineItemContainer
    {
        /// <summary>
        /// The invoice 'key'
        /// </summary>
        [DataMember]
        Guid InvoiceKey { get; }

        /// <summary>
        /// The order number prefix
        /// </summary>
        [DataMember]
        string OrderNumberPrefix { get; set; }

        /// <summary>
        /// The order number
        /// </summary>
        [DataMember]
        int OrderNumber { get; }

        /// <summary>
        /// The date of the order
        /// </summary>
        DateTime OrderDate { get; set; }

        /// <summary>
        /// The order status key
        /// </summary>
        Guid OrderStatusKey { get; set; }

        /// <summary>
        /// Indicates whether or not this order has been exported to an external system
        /// </summary>
        [DataMember]
        bool Exported { get; set; }

    }
}