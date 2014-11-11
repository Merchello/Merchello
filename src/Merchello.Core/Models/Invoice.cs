namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// The invoice.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Invoice : VersionTaggedEntity, IInvoice
    {
        #region Fields

        /// <summary>
        /// The customer key selector.
        /// </summary>
        private static readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<Invoice, Guid?>(x => x.CustomerKey);

        /// <summary>
        /// The invoice number prefix selector.
        /// </summary>
        private static readonly PropertyInfo InvoiceNumberPrefixSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.InvoiceNumberPrefix);

        /// <summary>
        /// The invoice number selector.
        /// </summary>
        private static readonly PropertyInfo InvoiceNumberSelector = ExpressionHelper.GetPropertyInfo<Invoice, int>(x => x.InvoiceNumber);

        /// <summary>
        /// The invoice number prefix selector.
        /// </summary>
        private static readonly PropertyInfo PoNumberSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.PoNumber);


        /// <summary>
        /// The invoice date selector.
        /// </summary>
        private static readonly PropertyInfo InvoiceDateSelector = ExpressionHelper.GetPropertyInfo<Invoice, DateTime>(x => x.InvoiceDate);

        /// <summary>
        /// The invoice status selector.
        /// </summary>
        private static readonly PropertyInfo InvoiceStatusSelector = ExpressionHelper.GetPropertyInfo<Invoice, IInvoiceStatus>(x => x.InvoiceStatus);

        /// <summary>
        /// The bill to name selector.
        /// </summary>
        private static readonly PropertyInfo BillToNameSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToName);

        /// <summary>
        /// The bill to address 1 selector.
        /// </summary>
        private static readonly PropertyInfo BillToAddress1Selector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToAddress1);

        /// <summary>
        /// The bill to address 2 selector.
        /// </summary>
        private static readonly PropertyInfo BillToAddress2Selector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToAddress2);

        /// <summary>
        /// The bill to locality selector.
        /// </summary>
        private static readonly PropertyInfo BillToLocalitySelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToLocality);

        /// <summary>
        /// The bill to region selector.
        /// </summary>
        private static readonly PropertyInfo BillToRegionSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToRegion);

        /// <summary>
        /// The bill to postal code selector.
        /// </summary>
        private static readonly PropertyInfo BillToPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToPostalCode);

        /// <summary>
        /// The bill to country code selector.
        /// </summary>
        private static readonly PropertyInfo BillToCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToCountryCode);

        /// <summary>
        /// The bill to email selector.
        /// </summary>
        private static readonly PropertyInfo BillToEmailSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToEmail);

        /// <summary>
        /// The bill to phone selector.
        /// </summary>
        private static readonly PropertyInfo BillToPhoneSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToPhone);

        /// <summary>
        /// The bill to company selector.
        /// </summary>
        private static readonly PropertyInfo BillToCompanySelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToCompany);

        /// <summary>
        /// The exported selector.
        /// </summary>
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Invoice, bool>(x => x.Exported);

        /// <summary>
        /// The archived selector.
        /// </summary>
        private static readonly PropertyInfo ArchivedSelector = ExpressionHelper.GetPropertyInfo<Invoice, bool>(x => x.Archived);

        /// <summary>
        /// The total selector.
        /// </summary>
        private static readonly PropertyInfo TotalSelector = ExpressionHelper.GetPropertyInfo<Invoice, decimal>(x => x.Total);

        /// <summary>
        /// The orders changed selector.
        /// </summary>
        private static readonly PropertyInfo OrdersChangedSelector = ExpressionHelper.GetPropertyInfo<Invoice, OrderCollection>(x => x.Orders);

        /// <summary>
        /// The customer key.
        /// </summary>
        private Guid? _customerKey;

        /// <summary>
        /// The invoice number.
        /// </summary>
        private int _invoiceNumber;

        /// <summary>
        /// The invoice number prefix.
        /// </summary>
        private string _invoiceNumberPrefix;

        /// <summary>
        /// The purchase order number.
        /// </summary>
        private string _poNumber;

        /// <summary>
        /// The invoice date.
        /// </summary>
        private DateTime _invoiceDate;

        /// <summary>
        /// The invoice status.
        /// </summary>
        private IInvoiceStatus _invoiceStatus;

        /// <summary>
        /// The bill to name.
        /// </summary>
        private string _billToName;

        /// <summary>
        /// The bill to address 1.
        /// </summary>
        private string _billToAddress1;

        /// <summary>
        /// The bill to address 2.
        /// </summary>
        private string _billToAddress2;

        /// <summary>
        /// The bill to locality.
        /// </summary>
        private string _billToLocality;

        /// <summary>
        /// The bill to region.
        /// </summary>
        private string _billToRegion;

        /// <summary>
        /// The bill to postal code.
        /// </summary>
        private string _billToPostalCode;

        /// <summary>
        /// The bill to country code.
        /// </summary>
        private string _billToCountryCode;

        /// <summary>
        /// The bill to email.
        /// </summary>
        private string _billToEmail;

        /// <summary>
        /// The bill to phone.
        /// </summary>
        private string _billToPhone;

        /// <summary>
        /// The bill to company.
        /// </summary>
        private string _billToCompany;

        /// <summary>
        /// The exported.
        /// </summary>
        private bool _exported;

        /// <summary>
        /// The archived.
        /// </summary>
        private bool _archived;

        /// <summary>
        /// The total.
        /// </summary>
        private decimal _total;

        /// <summary>
        /// The examine id.
        /// </summary>
        private int _examineId = 1;

        /// <summary>
        /// The items.
        /// </summary>
        private LineItemCollection _items;

        /// <summary>
        /// The orders.
        /// </summary>
        private OrderCollection _orders;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Invoice"/> class.
        /// </summary>
        /// <param name="invoiceStatus">
        /// The invoice status.
        /// </param>
        internal Invoice(IInvoiceStatus invoiceStatus)
            : this(invoiceStatus, new Address())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Invoice"/> class.
        /// </summary>
        /// <param name="invoiceStatus">
        /// The invoice status.
        /// </param>
        /// <param name="billToAddress">
        /// The bill to address.
        /// </param>
        internal Invoice(IInvoiceStatus invoiceStatus, IAddress billToAddress)
            : this(invoiceStatus, billToAddress, new LineItemCollection(), new OrderCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Invoice"/> class.
        /// </summary>
        /// <param name="invoiceStatus">
        /// The invoice status.
        /// </param>
        /// <param name="billToAddress">
        /// The bill to address.
        /// </param>
        /// <param name="lineItemCollection">
        /// The line item collection.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        internal Invoice(IInvoiceStatus invoiceStatus, IAddress billToAddress, LineItemCollection lineItemCollection, OrderCollection orders)
        {
            Mandate.ParameterNotNull(invoiceStatus, "invoiceStatus");
            Mandate.ParameterNotNull(billToAddress, "billToAddress");
            Mandate.ParameterNotNull(lineItemCollection, "lineItemCollection");
            Mandate.ParameterNotNull(orders, "orders");

            _invoiceStatus = invoiceStatus;

            _billToName = billToAddress.Name;
            _billToAddress1 = billToAddress.Address1;
            _billToAddress2 = billToAddress.Address2;
            _billToLocality = billToAddress.Locality;
            _billToRegion = billToAddress.Region;
            _billToPostalCode = billToAddress.PostalCode;
            _billToCountryCode = billToAddress.CountryCode;
            _billToPhone = billToAddress.Phone;

            _items = lineItemCollection;
            _orders = orders;
            _invoiceDate = DateTime.Now;

        }

        /// <summary>
        /// Gets or sets the unique customer 'key' to associated with the invoice
        /// </summary>
        [DataMember]
        public Guid? CustomerKey
        {
            get
            {
                return _customerKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _customerKey = value;
                    return _customerKey;
                }, 
                _customerKey, 
                CustomerKeySelector);
            }
        }


        /// <summary>
        /// Gets or sets the optional invoice number prefix
        /// </summary>
        [DataMember]
        public string InvoiceNumberPrefix
        {
            get
            {
                return _invoiceNumberPrefix;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _invoiceNumberPrefix = value;
                    return _invoiceNumberPrefix;
                }, 
                _invoiceNumberPrefix, 
                InvoiceNumberPrefixSelector);
            }
        }

        /// <summary>
        /// Gets or sets the invoice number
        /// </summary>
        [DataMember]
        public int InvoiceNumber
        {
            get
            {
                return _invoiceNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _invoiceNumber = value;
                    return _invoiceNumber;
                }, 
                _invoiceNumber, 
                InvoiceNumberSelector);
            }
        }

        /// <summary>
        /// Gets or sets the po number.
        /// </summary>
        [DataMember]
        public string PoNumber
        {
            get
            {
                return _poNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _poNumber = value;
                        return _poNumber;
                    },
                _poNumber,
                PoNumberSelector);
            }
        }

        /// <summary>
        /// Gets or sets the invoice date
        /// </summary>
        [DataMember]
        public DateTime InvoiceDate
        {
            get
            {
                return _invoiceDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _invoiceDate = value;
                    return _invoiceDate;
                }, 
                _invoiceDate, 
                InvoiceDateSelector);
            }
        }

        /// <summary>
        /// Gets the key for the invoice status associated with this invoice
        /// </summary>
        [DataMember]
        public Guid InvoiceStatusKey
        {
            get { return _invoiceStatus.Key; }            
        }

        /// <summary>
        /// Gets or sets the invoice status.
        /// </summary>
        [DataMember]
        public IInvoiceStatus InvoiceStatus
        {
            get
            {
                return _invoiceStatus;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _invoiceStatus = value;
                    return _invoiceStatus;
                }, 
                _invoiceStatus, 
                InvoiceStatusSelector);
            }
        }

        /// <summary>
        /// Gets or sets the full name to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToName
        {
            get
            {
                return _billToName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToName = value;
                    return _billToName;
                }, 
                _billToName, 
                BillToNameSelector);
            }
        }

        /// <summary>
        /// Gets or sets the address line 1 to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToAddress1
        {
            get
            {
                return _billToAddress1;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToAddress1 = value;
                    return _billToAddress1;
                }, 
                _billToAddress1, 
                BillToAddress1Selector);
            }
        }

        /// <summary>
        /// Gets or sets the address line 2 to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToAddress2
        {
            get
            {
                return _billToAddress2;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToAddress2 = value;
                    return _billToAddress2;
                }, 
                _billToAddress2, 
                BillToAddress2Selector);
            }
        }

        /// <summary>
        /// Gets or sets the city or locality to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToLocality
        {
            get
            {
                return _billToLocality;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToLocality = value;
                    return _billToLocality;
                }, 
                _billToLocality, 
                BillToLocalitySelector);
            }
        }

        /// <summary>
        /// Gets or sets the state, region or province to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToRegion
        {
            get
            {
                return _billToRegion;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToRegion = value;
                    return _billToRegion;
                }, 
                _billToRegion, 
                BillToRegionSelector);
            }
        }

        /// <summary>
        /// Gets or sets the postal code to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToPostalCode
        {
            get
            {
                return _billToPostalCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToPostalCode = value;
                    return _billToPostalCode;
                }, 
                _billToPostalCode, 
                BillToPostalCodeSelector);
            }
        }

        /// <summary>
        /// Gets or sets the country code to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToCountryCode
        {
            get
            {
                return _billToCountryCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToCountryCode = value;
                    return _billToCountryCode;
                }, 
                _billToCountryCode, 
                BillToCountryCodeSelector);
            }
        }

        /// <summary>
        /// Gets or sets the email address to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToEmail
        {
            get
            {
                return _billToEmail;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToEmail = value;
                    return _billToEmail;
                },
                    _billToEmail,
                    BillToEmailSelector);
            }
        }

        /// <summary>
        /// Gets or sets the phone number to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToPhone
        {
            get
            {
                return _billToPhone;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToPhone = value;
                    return _billToPhone;
                }, 
                _billToPhone, 
                BillToPhoneSelector);
            }
        }

        /// <summary>
        /// Gets or sets the company name to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToCompany
        {
            get
            {
                return _billToCompany;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _billToCompany = value;
                    return _billToCompany;
                }, 
                _billToCompany, 
                BillToCompanySelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this invoice has been exported to an external system
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
        /// Gets or sets a value indicating whether or not this invoice has been archived
        /// </summary>
        [DataMember]
        public bool Archived
        {
            get
            {
                return _archived;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _archived = value;
                    return _archived;
                }, 
                _archived, 
                ArchivedSelector);
            }
        }

        /// <summary>
        /// Gets or sets the total invoice amount
        /// </summary>
        [DataMember]
        public decimal Total
        {
            get
            {
                return _total;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                {
                    _total = value;
                    return _total;
                }, 
                _total, 
                TotalSelector);
            }
        }

        /// <summary>
        /// Gets or sets the collection of orders associated with the invoice
        /// </summary>
        [DataMember]
        public OrderCollection Orders
        {
            get
            {
                return _orders;
            }

            set
            {
                _orders = value;
                _orders.CollectionChanged += OrdersChanged;
            }
        }

        /// <summary>
        /// Gets the <see cref="ILineItem"/>s in the invoice
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

        /// <summary>
        /// The orders changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OrdersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(OrdersChangedSelector);
        }
    }
}