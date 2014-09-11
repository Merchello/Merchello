namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines an Order
    /// </summary>
    public interface IOrder : ILineItemContainer
    {
        /// <summary>
        /// Gets the invoice 'key'
        /// </summary>
        [DataMember]
        Guid InvoiceKey { get; }

        /// <summary>
        /// Gets or sets the order number prefix
        /// </summary>
        [DataMember]
        string OrderNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        [DataMember]
        int OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the date of the order
        /// </summary>
        DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets the order status key
        /// </summary>
        Guid OrderStatusKey { get; }

        /// <summary>
        /// Gets or sets the <see cref="IOrderStatus"/>
        /// </summary>
        IOrderStatus OrderStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this order has been exported to an external system
        /// </summary>
        [DataMember]
        bool Exported { get; set; }
    }
}