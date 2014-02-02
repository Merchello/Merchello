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

        public OrderLineItem(string name, string sku, decimal amount) 
            : base(name, sku, amount)
        { }

        public OrderLineItem(string name, string sku, int quantity, decimal amount)
            : base(name, sku, quantity, amount)
        { }

        public OrderLineItem(LineItemType lineItemType, string name, string sku, int quantity, decimal amount) 
            : base(lineItemType, name, sku, quantity, amount)
        { }

        public OrderLineItem(LineItemType lineItemType, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData) 
            : base(lineItemType, name, sku, quantity, amount, extendedData)
        { }

        public OrderLineItem(Guid lineItemTfKey, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData) 
            : base(lineItemTfKey, name, sku, quantity, amount, extendedData)
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