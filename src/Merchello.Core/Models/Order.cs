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
        #region Fields

        /// <summary>
        /// The invoice key selector.
        /// </summary>
        private static readonly PropertyInfo InvoiceKeySelector = ExpressionHelper.GetPropertyInfo<Order, Guid>(x => x.InvoiceKey);

        /// <summary>
        /// The order number prefix selector.
        /// </summary>
        private static readonly PropertyInfo OrderNumberPrefixSelector = ExpressionHelper.GetPropertyInfo<Order, string>(x => x.OrderNumberPrefix);

        /// <summary>
        /// The order number selector.
        /// </summary>
        private static readonly PropertyInfo OrderNumberSelector = ExpressionHelper.GetPropertyInfo<Order, int>(x => x.OrderNumber);

        /// <summary>
        /// The order date selector.
        /// </summary>
        private static readonly PropertyInfo OrderDateSelector = ExpressionHelper.GetPropertyInfo<Order, DateTime>(x => x.OrderDate);

        /// <summary>
        /// The order status selector.
        /// </summary>
        private static readonly PropertyInfo OrderStatusSelector = ExpressionHelper.GetPropertyInfo<Order, IOrderStatus>(x => x.OrderStatus);

        /// <summary>
        /// The exported selector.
        /// </summary>
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Order, bool>(x => x.Exported);

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
            Mandate.ParameterNotNull(orderStatus, "orderStatus");
            Mandate.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
            Mandate.ParameterNotNull(lineItemCollection, "lineItemCollection");

            _invoiceKey = invoiceKey;
            _orderStatus = orderStatus;
            _items = lineItemCollection;

            _orderDate = DateTime.Now;
        }


        /// <summary>
        /// Gets the invoice 'key'
        /// </summary>
        [DataMember]
        public Guid InvoiceKey 
        {
            get
            {
                return _invoiceKey;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _invoiceKey = value;
                    return _invoiceKey;
                }, 
                _invoiceKey,
                InvoiceKeySelector);
            }
        }        

        /// <summary>
        /// Gets or sets The order number prefix
        /// </summary>
        [DataMember]
        public string OrderNumberPrefix 
        {
            get
            {
                return _orderNumberPrefix;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                    _orderNumberPrefix = value;
                    return _orderNumberPrefix;
                }, 
                _orderNumberPrefix,
                OrderNumberPrefixSelector);
            }
        }

        /// <summary>
        /// Gets or sets the unique OrderNumber
        /// </summary>
        [DataMember]
        public int OrderNumber
        {
            get
            {
                return _orderNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                    _orderNumber = value;
                    return _orderNumber;
                }, 
                _orderNumber,
                OrderNumberSelector);
            }
        }

        /// <summary>
        /// Gets or sets the date of the order
        /// </summary>
        [DataMember]
        public DateTime OrderDate
        {
            get
            {
                return _orderDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                    _orderDate = value;
                    return _orderDate;
                }, 
                _orderDate, 
                OrderDateSelector);
            }
        }

        /// <summary>
        /// Gets the order status key
        /// </summary>
        [DataMember]
        public Guid OrderStatusKey 
        {
            get { return _orderStatus.Key; }            
        }

        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        [DataMember]
        public IOrderStatus OrderStatus
        {
            get
            {
                return _orderStatus;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                    _orderStatus = value;
                    return _orderStatus;
                }, 
                _orderStatus, 
                OrderStatusSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this order has been exported to an external system
        /// </summary>
        [DataMember]
        public bool Exported
        {
            get
            {
                return _exported;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                    _exported = value;
                    return _exported;
                }, 
                _exported, 
                ExportedSelector);
            }
        }

                /// <summary>
        /// Gets the <see cref="ILineItem"/>s in the order
        /// </summary>
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

        /// <summary>
        /// Gets or sets the examine id.
        /// </summary>
        [IgnoreDataMember]
        internal int ExamineId
        {
            get { return _examineId; }
            set { _examineId = value; }
        }
    }
}