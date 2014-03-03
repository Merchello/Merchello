using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Represents an order line item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class OrderLineItem : LineItemBase, IOrderLineItem
    {

        private Guid? _shipmentKey;
        private bool _backOrder;

        public OrderLineItem(string name, string sku, decimal amount) 
            : base(name, sku, amount)
        { }

        public OrderLineItem(string name, string sku, int quantity, decimal amount)
            : base(name, sku, quantity, amount)
        { }

        public OrderLineItem(LineItemType lineItemType, string name, string sku, int quantity, decimal price) 
            : base(lineItemType, name, sku, quantity, price)
        { }

        public OrderLineItem(LineItemType lineItemType, string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData) 
            : base(lineItemType, name, sku, quantity, price, extendedData)
        { }

        public OrderLineItem(Guid lineItemTfKey, string name, string sku, int quantity, decimal price, ExtendedDataCollection extendedData) 
            : base(lineItemTfKey, name, sku, quantity, price, extendedData)
        { }

        private static readonly PropertyInfo ShipmentKeySelector = ExpressionHelper.GetPropertyInfo<OrderLineItem, Guid?>(x => x.LineItemTfKey);
        private static readonly PropertyInfo BackOrderSelector = ExpressionHelper.GetPropertyInfo<OrderLineItem, bool>(x => x.BackOrder);

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

        /// <summary>
        /// True false indicating whether or not this line item represents a back order line item
        /// </summary>
        public bool BackOrder
        {
            get { return _backOrder; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _backOrder = value;
                    return _backOrder;
                }, _backOrder, BackOrderSelector);
            }
        }
    }

}