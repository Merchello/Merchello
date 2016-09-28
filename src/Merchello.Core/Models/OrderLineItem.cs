namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an order line item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class OrderLineItem : LineItemBase, IOrderLineItem
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The shipment key.
        /// </summary>
        private Guid? _shipmentKey;

        /// <summary>
        /// The back order.
        /// </summary>
        private bool _backOrder;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderLineItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        public OrderLineItem(string name, string sku, decimal amount)
            : base(name, sku, amount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderLineItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        public OrderLineItem(string name, string sku, int quantity, decimal amount)
            : base(name, sku, quantity, amount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderLineItem"/> class.
        /// </summary>
        /// <param name="lineItemType">
        /// The line item type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        public OrderLineItem(LineItemType lineItemType, string name, string sku, int quantity, decimal price)
            : base(lineItemType, name, sku, quantity, price)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderLineItem"/> class.
        /// </summary>
        /// <param name="lineItemType">
        /// The line item type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public OrderLineItem(
            LineItemType lineItemType,
            string name,
            string sku,
            int quantity,
            decimal price,
            ExtendedDataCollection extendedData)
            : base(lineItemType, name, sku, quantity, price, extendedData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderLineItem"/> class.
        /// </summary>
        /// <param name="lineItemTfKey">
        /// The line item type field key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        public OrderLineItem(
            Guid lineItemTfKey,
            string name,
            string sku,
            int quantity,
            decimal price,
            ExtendedDataCollection extendedData)
            : base(lineItemTfKey, name, sku, quantity, price, extendedData)
        {
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? ShipmentKey
        {
            get
            {
                return _shipmentKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _shipmentKey, _ps.Value.ShipmentKeySelector);
            }
        }

        /// <inheritdoc/>
        public bool BackOrder
        {
            get
            {
                return _backOrder;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _backOrder, _ps.Value.BackOrderSelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The shipment key selector.
            /// </summary>
            public readonly PropertyInfo ShipmentKeySelector = ExpressionHelper.GetPropertyInfo<OrderLineItem, Guid?>(x => x.LineItemTfKey);

            /// <summary>
            /// The back order selector.
            /// </summary>
            public readonly PropertyInfo BackOrderSelector = ExpressionHelper.GetPropertyInfo<OrderLineItem, bool>(x => x.BackOrder);
        }
    }
}