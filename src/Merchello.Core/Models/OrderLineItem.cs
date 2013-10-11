using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Defines a order list item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal sealed class OrderLineItem : LineItemBase, IOrderLineItem
    {

        public OrderLineItem(int containerId, string name, string sku, decimal amount)
            : this(containerId, name, sku, 1, amount)
        { }

        public OrderLineItem(int containerId, string name, string sku, int quantity, decimal amount)
            : this(containerId, LineItemType.Product, name, sku, quantity, amount)
        { }

        public OrderLineItem(int containerId, LineItemType lineItemType, string name, string sku, int quantity, decimal amount)
            : this(containerId, EnumTypeFieldConverter.LineItemType.GetTypeField(lineItemType).TypeKey, name, sku, quantity, amount)
        { }

        internal OrderLineItem(int containerId, Guid lineItemTfKey, string name, string sku, int quantity, decimal amount)
            : base(containerId, lineItemTfKey)
        {
            Name = name;
            Sku = sku;
            Quantity = quantity;
            Amount = amount;
            LineItemTfKey = lineItemTfKey;

            ResetDirtyProperties();
        }

    }

}