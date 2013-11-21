using System;
using System.Reflection;
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

        private Guid? _shipmentKey;

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

        private static readonly PropertyInfo ShipmentKeySelector = ExpressionHelper.GetPropertyInfo<OrderLineItem, Guid?>(x => x.LineItemTfKey);

        /// <summary>
        /// The unique key (guid) associated with the shipment record in which this item was shipped.
        /// </summary>
        [DataMember]
        public Guid? ShipmentKey
        {
            get { return _shipmentKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _shipmentKey = value;
                    return _shipmentKey;
                }, _shipmentKey, ShipmentKeySelector);
            }
        }

    }

}