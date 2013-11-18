using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Represents an order line item
    /// </summary>
    /// <remarks>
    /// Needed for typed query mapper
    /// </remarks>
    [Serializable]
    [DataContract(IsReference = true)]
    public class OrderLineItem : LineItemBase, IOrderLineItem
    {
        public OrderLineItem(Guid containerKey, string name, string sku, decimal amount) 
            : base(containerKey, name, sku, amount)
        { }

        public OrderLineItem(Guid containerKey, string name, string sku, int quantity, decimal amount)
            : base(containerKey, name, sku, quantity, amount)
        { }

        public OrderLineItem(Guid containerKey, LineItemType lineItemType, string name, string sku, int quantity, decimal amount) 
            : base(containerKey, lineItemType, name, sku, quantity, amount)
        { }

        public OrderLineItem(Guid containerKey, LineItemType lineItemType, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData) 
            : base(containerKey, lineItemType, name, sku, quantity, amount, extendedData)
        { }

        public OrderLineItem(Guid containerKey, Guid lineItemTfKey, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData) 
            : base(containerKey, lineItemTfKey, name, sku, quantity, amount, extendedData)
        { }
    }

}