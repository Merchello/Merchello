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
        public OrderLineItem(int containerId, string name, string sku, decimal amount) 
            : base(containerId, name, sku, amount)
        { }

        public OrderLineItem(int containerId, string name, string sku, int quantity, decimal amount)
            : base(containerId, name, sku, quantity, amount)
        { }

        public OrderLineItem(int containerId, LineItemType lineItemType, string name, string sku, int quantity, decimal amount) 
            : base(containerId, lineItemType, name, sku, quantity, amount)
        { }

        public OrderLineItem(int containerId, LineItemType lineItemType, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData) 
            : base(containerId, lineItemType, name, sku, quantity, amount, extendedData)
        { }

        public OrderLineItem(int containerId, Guid lineItemTfKey, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData) 
            : base(containerId, lineItemTfKey, name, sku, quantity, amount, extendedData)
        { }
    }

}