namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
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
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

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
        /// The currency code.
        /// </summary>
        private string _currencyCode;

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

        /// <summary>
        /// The notes.
        /// </summary>
        private IEnumerable<INote> _notes;

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
            : this(invoiceStatus, billToAddress, new LineItemCollection(), new OrderCollection(), Enumerable.Empty<INote>().ToArray())
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
        /// <param name="notes">
        /// The notes collection
        /// </param>
        internal Invoice(IInvoiceStatus invoiceStatus, IAddress billToAddress, LineItemCollection lineItemCollection, OrderCollection orders, INote[] notes)
        {
            Ensure.ParameterNotNull(invoiceStatus, "invoiceStatus");
            Ensure.ParameterNotNull(billToAddress, "billToAddress");
            Ensure.ParameterNotNull(lineItemCollection, "lineItemCollection");
            Ensure.ParameterNotNull(orders, "orders");
            Ensure.ParameterNotNull(notes, "notes");

            _invoiceStatus = invoiceStatus;

            _billToName = billToAddress.Name;
            _billToAddress1 = billToAddress.Address1;
            _billToAddress2 = billToAddress.Address2;
            _billToLocality = billToAddress.Locality;
            _billToRegion = billToAddress.Region;
            _billToPostalCode = billToAddress.PostalCode;
            _billToCountryCode = billToAddress.CountryCode;
            _billToPhone = billToAddress.Phone;
            _notes = notes;
            _items = lineItemCollection;
            _orders = orders;
            _invoiceDate = DateTime.Now;

        }

        /// <inheritdoc/>
        [DataMember]
        public Guid? CustomerKey
        {
            get
            {
                return _customerKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _customerKey, _ps.Value.CustomerKeySelector);
            }
        }


        /// <inheritdoc/>
        [DataMember]
        public string InvoiceNumberPrefix
        {
            get
            {
                return _invoiceNumberPrefix;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _invoiceNumberPrefix, _ps.Value.InvoiceNumberPrefixSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public int InvoiceNumber
        {
            get
            {
                return _invoiceNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _invoiceNumber, _ps.Value.InvoiceNumberSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string PoNumber
        {
            get
            {
                return _poNumber;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _poNumber, _ps.Value.PoNumberSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public DateTime InvoiceDate
        {
            get
            {
                return _invoiceDate;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _invoiceDate, _ps.Value.InvoiceDateSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid InvoiceStatusKey
        {
            get { return _invoiceStatus.Key; }            
        }

        /// <inheritdoc/>
        [DataMember]
        public IInvoiceStatus InvoiceStatus
        {
            get
            {
                return _invoiceStatus;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _invoiceStatus, _ps.Value.InvoiceStatusSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToName
        {
            get
            {
                return _billToName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToName, _ps.Value.BillToNameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToAddress1
        {
            get
            {
                return _billToAddress1;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToAddress1, _ps.Value.BillToAddress1Selector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToAddress2
        {
            get
            {
                return _billToAddress2;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToAddress2, _ps.Value.BillToAddress2Selector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToLocality
        {
            get
            {
                return _billToLocality;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToLocality, _ps.Value.BillToLocalitySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToRegion
        {
            get
            {
                return _billToRegion;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToRegion, _ps.Value.BillToRegionSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToPostalCode
        {
            get
            {
                return _billToPostalCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToPostalCode, _ps.Value.BillToPostalCodeSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToCountryCode
        {
            get
            {
                return _billToCountryCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToCountryCode, _ps.Value.BillToCountryCodeSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToEmail
        {
            get
            {
                return _billToEmail;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToEmail, _ps.Value.BillToEmailSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToPhone
        {
            get
            {
                return _billToPhone;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToPhone, _ps.Value.BillToPhoneSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string BillToCompany
        {
            get
            {
                return _billToCompany;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _billToCompany, _ps.Value.BillToCompanySelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string CurrencyCode
        {
            get
            {
                return _currencyCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _currencyCode, _ps.Value.CurrencyCodeSelector);
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
        public bool Archived
        {
            get
            {
                return _archived;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _archived, _ps.Value.ArchivedSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal Total
        {
            get
            {
                return _total;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _total, _ps.Value.TotalSelector);
            }
        }

        /// <inheritdoc/>
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


        /// <inheritdoc/>
        [DataMember]
        public IEnumerable<INote> Notes
        {
            get
            {
                return _notes;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _notes, _ps.Value.NotesSelector);
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
            this.Items.Accept(visitor);
        }


        /// <summary>
        /// Handles the order collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OrdersChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.OrdersChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The customer key selector.
            /// </summary>
            public readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<Invoice, Guid?>(x => x.CustomerKey);

            /// <summary>
            /// The invoice number prefix selector.
            /// </summary>
            public readonly PropertyInfo InvoiceNumberPrefixSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.InvoiceNumberPrefix);

            /// <summary>
            /// The invoice number selector.
            /// </summary>
            public readonly PropertyInfo InvoiceNumberSelector = ExpressionHelper.GetPropertyInfo<Invoice, int>(x => x.InvoiceNumber);

            /// <summary>
            /// The invoice number prefix selector.
            /// </summary>
            public readonly PropertyInfo PoNumberSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.PoNumber);


            /// <summary>
            /// The invoice date selector.
            /// </summary>
            public readonly PropertyInfo InvoiceDateSelector = ExpressionHelper.GetPropertyInfo<Invoice, DateTime>(x => x.InvoiceDate);

            /// <summary>
            /// The invoice status selector.
            /// </summary>
            public readonly PropertyInfo InvoiceStatusSelector = ExpressionHelper.GetPropertyInfo<Invoice, IInvoiceStatus>(x => x.InvoiceStatus);

            /// <summary>
            /// The bill to name selector.
            /// </summary>
            public readonly PropertyInfo BillToNameSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToName);

            /// <summary>
            /// The bill to address 1 selector.
            /// </summary>
            public readonly PropertyInfo BillToAddress1Selector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToAddress1);

            /// <summary>
            /// The bill to address 2 selector.
            /// </summary>
            public readonly PropertyInfo BillToAddress2Selector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToAddress2);

            /// <summary>
            /// The bill to locality selector.
            /// </summary>
            public readonly PropertyInfo BillToLocalitySelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToLocality);

            /// <summary>
            /// The bill to region selector.
            /// </summary>
            public readonly PropertyInfo BillToRegionSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToRegion);

            /// <summary>
            /// The bill to postal code selector.
            /// </summary>
            public readonly PropertyInfo BillToPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToPostalCode);

            /// <summary>
            /// The bill to country code selector.
            /// </summary>
            public readonly PropertyInfo BillToCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToCountryCode);

            /// <summary>
            /// The bill to email selector.
            /// </summary>
            public readonly PropertyInfo BillToEmailSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToEmail);

            /// <summary>
            /// The bill to phone selector.
            /// </summary>
            public readonly PropertyInfo BillToPhoneSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToPhone);

            /// <summary>
            /// The bill to company selector.
            /// </summary>
            public readonly PropertyInfo BillToCompanySelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToCompany);

            /// <summary>
            /// The bill to company selector.
            /// </summary>
            public readonly PropertyInfo CurrencyCodeSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.CurrencyCode);

            /// <summary>
            /// The exported selector.
            /// </summary>
            public readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Invoice, bool>(x => x.Exported);

            /// <summary>
            /// The archived selector.
            /// </summary>
            public readonly PropertyInfo ArchivedSelector = ExpressionHelper.GetPropertyInfo<Invoice, bool>(x => x.Archived);

            /// <summary>
            /// The total selector.
            /// </summary>
            public readonly PropertyInfo TotalSelector = ExpressionHelper.GetPropertyInfo<Invoice, decimal>(x => x.Total);

            /// <summary>
            /// The orders changed selector.
            /// </summary>
            public readonly PropertyInfo OrdersChangedSelector = ExpressionHelper.GetPropertyInfo<Invoice, OrderCollection>(x => x.Orders);

            /// <summary>
            /// The notes selector.
            /// </summary>
            public readonly PropertyInfo NotesSelector = ExpressionHelper.GetPropertyInfo<Invoice, IEnumerable<INote>>(x => x.Notes);
        }
    }
}