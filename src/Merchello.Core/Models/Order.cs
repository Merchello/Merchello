using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents an Order
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Order : VersionTaggedEntity, IOrder
    {
        private Guid _invoiceKey;
        private string _orderNumberPrefix;
        private int _orderNumber;
        private DateTime _orderDate;
        private IOrderStatus _orderStatus;
        private bool _exported;
        private int _examineId = 1;
        private LineItemCollection _items;

        internal Order(IOrderStatus orderStatus, Guid invoiceKey)
            : this(orderStatus, invoiceKey, new LineItemCollection())
        { }

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

        private static readonly PropertyInfo InvoiceKeySelector = ExpressionHelper.GetPropertyInfo<Order, Guid>(x => x.InvoiceKey);
        private static readonly PropertyInfo OrderNumberPrefixSelector = ExpressionHelper.GetPropertyInfo<Order, string>(x => x.OrderNumberPrefix);
        private static readonly PropertyInfo OrderNumberSelector = ExpressionHelper.GetPropertyInfo<Order, int>(x => x.OrderNumber);
        private static readonly PropertyInfo OrderDateSelector = ExpressionHelper.GetPropertyInfo<Order, DateTime>(x => x.OrderDate);
        private static readonly PropertyInfo OrderStatusSelector = ExpressionHelper.GetPropertyInfo<Order, IOrderStatus>(x => x.OrderStatus);
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Order, bool>(x => x.Exported);

        /// <summary>
        /// The invoice 'key'
        /// </summary>
        [DataMember]
        public Guid InvoiceKey 
        {
            get { return _invoiceKey; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _invoiceKey = value;
                    return _invoiceKey;
                }, _invoiceKey, InvoiceKeySelector);
            }
        }

        [IgnoreDataMember]
        internal int ExamineId
        {
            get { return _examineId; }
            set { _examineId = value; }
        }


        /// <summary>
        /// The order number prefix
        /// </summary>
        [DataMember]
        public string OrderNumberPrefix 
        {
            get { return _orderNumberPrefix; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _orderNumberPrefix = value;
                    return _orderNumberPrefix;
                }, _orderNumberPrefix, OrderNumberPrefixSelector);
            }
        }

        /// <summary>
        /// The unique OrderNumber
        /// </summary>
        [DataMember]
        public int OrderNumber
        {
            get { return _orderNumber; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _orderNumber = value;
                    return _orderNumber;
                }, _orderNumber, OrderNumberSelector);
            }
        }

        /// <summary>
        /// The date of the order
        /// </summary>
        [DataMember]
        public DateTime OrderDate
        {
            get { return _orderDate; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _orderDate = value;
                    return _orderDate;
                }, _orderDate, OrderDateSelector);
            }
        }

        /// <summary>
        /// The order status key
        /// </summary>
        [DataMember]
        public Guid OrderStatusKey 
        {
            get { return _orderStatus.Key; }
            
        }

        /// <summary>
        /// Gets or sets the 
        /// </summary>
        [DataMember]
        public IOrderStatus OrderStatus
        {
            get { return _orderStatus; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _orderStatus = value;
                    return _orderStatus;
                }, _orderStatus, OrderStatusSelector);
            }
        }

        /// <summary>
        /// Indicates whether or not this order has been exported to an external system
        /// </summary>
        [DataMember]
        public bool Exported
        {
            get { return _exported; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _exported = value;
                    return _exported;
                }, _exported, ExportedSelector);
            }
        }

                /// <summary>
        /// The <see cref="ILineItem"/>s in the invoice
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
    }
}