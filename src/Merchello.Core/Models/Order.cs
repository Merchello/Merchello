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
        private Guid _orderStatusKey;
        private bool _exported;
        private LineItemCollection _items;

        internal Order(Guid orderStatusKey, Guid invoiceKey)
            : this(orderStatusKey, invoiceKey, new LineItemCollection())
        { }

        internal Order(Guid orderStatusKey, Guid invoiceKey, LineItemCollection lineItemCollection)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(orderStatusKey), "orderStatusKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
            Mandate.ParameterNotNull(lineItemCollection, "lineItemCollection");

            _invoiceKey = invoiceKey;
            _orderStatusKey = orderStatusKey;
            _items = lineItemCollection;

            _orderDate = DateTime.Now;
        }

        private static readonly PropertyInfo InvoiceKeySelector = ExpressionHelper.GetPropertyInfo<Order, Guid>(x => x.InvoiceKey);
        private static readonly PropertyInfo OrderNumberPrefixSelector = ExpressionHelper.GetPropertyInfo<Order, string>(x => x.OrderNumberPrefix);
        private static readonly PropertyInfo OrderNumberSelector = ExpressionHelper.GetPropertyInfo<Order, int>(x => x.OrderNumber);
        private static readonly PropertyInfo OrderDateSelector = ExpressionHelper.GetPropertyInfo<Order, DateTime>(x => x.OrderDate);
        private static readonly PropertyInfo OrderStatusKeySelector = ExpressionHelper.GetPropertyInfo<Order, Guid>(x => x.OrderStatusKey);
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
            get { return _orderStatusKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _orderStatusKey = value;
                    return _orderStatusKey;
                }, _orderStatusKey, OrderStatusKeySelector);
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