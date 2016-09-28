namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using EntityBase;

    /// <summary>
    /// Represents an Order
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Order : VersionTaggedEntity, IOrder
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// The invoice key.
        /// </summary>
        private Guid _invoiceKey;

        /// <summary>
        /// The order number prefix.
        /// </summary>
        private string _orderNumberPrefix;

        /// <summary>
        /// The order number.
        /// </summary>
        private int _orderNumber;

        /// <summary>
        /// The order date.
        /// </summary>
        private DateTime _orderDate;

        /// <summary>
        /// The order status.
        /// </summary>
        private IOrderStatus _orderStatus;

        /// <summary>
        /// True or false indicating whether or not this order has been exported.
        /// </summary>
        private bool _exported;

        /// <summary>
        /// The examine id.
        /// </summary>
        private int _examineId = 1;

        /// <summary>
        /// The items.
        /// </summary>
        private LineItemCollection _items;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="orderStatus">
        /// The order status.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        internal Order(IOrderStatus orderStatus, Guid invoiceKey)
            : this(orderStatus, invoiceKey, new LineItemCollection())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        /// <param name="orderStatus">
        /// The order status.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="lineItemCollection">
        /// The line item collection.
        /// </param>
        internal Order(IOrderStatus orderStatus, Guid invoiceKey, LineItemCollection lineItemCollection)
        {
            Ensure.ParameterNotNull(orderStatus, "orderStatus");
            Ensure.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
            Ensure.ParameterNotNull(lineItemCollection, "lineItemCollection");

            _invoiceKey = invoiceKey;
            _orderStatus = orderStatus;
            _items = lineItemCollection;

            _orderDate = DateTime.Now;
        }


        /// <inheritdoc/>
        [DataMember]
        public Guid InvoiceKey 
        {
            get
            {
                return _invoiceKey;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(value, ref _invoiceKey, _ps.Value.InvoiceKeySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string OrderNumberPrefix 
        {
            get
            {
                return _orderNumberPrefix;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _orderNumberPrefix, _ps.Value.OrderNumberPrefixSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int OrderNumber
        {
            get
            {
                return _orderNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _orderNumber, _ps.Value.OrderNumberSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public DateTime OrderDate
        {
            get
            {
                return _orderDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _orderDate, _ps.Value.OrderDateSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid OrderStatusKey 
        {
            get { return _orderStatus.Key; }            
        }

        /// <inheritdoc/>
        [DataMember]
        public IOrderStatus OrderStatus
        {
            get
            {
                return _orderStatus;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _orderStatus, _ps.Value.OrderStatusSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Exported
        {
            get
            {
                return _exported;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _exported, _ps.Value.ExportedSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public LineItemCollection Items
        {
            get
            {
                return _items;
            }

            internal set
            {
                _items = value;
            }
        }

        /// <inheritdoc/>
        [IgnoreDataMember]
        internal int ExamineId
        {
            get { return _examineId; }
            set { _examineId = value; }
        }

        /// <inheritdoc/>
        public void Accept(ILineItemVisitor visitor)
        {
            Items.Accept(visitor);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The invoice key selector.
            /// </summary>
            public readonly PropertyInfo InvoiceKeySelector = ExpressionHelper.GetPropertyInfo<Order, Guid>(x => x.InvoiceKey);

            /// <summary>
            /// The order number prefix selector.
            /// </summary>
            public readonly PropertyInfo OrderNumberPrefixSelector = ExpressionHelper.GetPropertyInfo<Order, string>(x => x.OrderNumberPrefix);

            /// <summary>
            /// The order number selector.
            /// </summary>
            public readonly PropertyInfo OrderNumberSelector = ExpressionHelper.GetPropertyInfo<Order, int>(x => x.OrderNumber);

            /// <summary>
            /// The order date selector.
            /// </summary>
            public readonly PropertyInfo OrderDateSelector = ExpressionHelper.GetPropertyInfo<Order, DateTime>(x => x.OrderDate);

            /// <summary>
            /// The order status selector.
            /// </summary>
            public readonly PropertyInfo OrderStatusSelector = ExpressionHelper.GetPropertyInfo<Order, IOrderStatus>(x => x.OrderStatus);

            /// <summary>
            /// The exported selector.
            /// </summary>
            public readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Order, bool>(x => x.Exported);
        }
    }
}