using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class Invoice : Entity, IInvoice
    {
        private Guid? _customerKey;
        private int _invoiceNumber;
        private DateTime _invoiceDate;
        private Guid _invoiceStatusKey;
        private string _billToName;
        private string _billToAddress1;
        private string _billToAddress2;
        private string _billToLocality;
        private string _billToRegion;
        private string _billToPostalCode;
        private string _billToCountryCode;
        private string _billToEmail;
        private string _billToPhone;
        private string _billToCompany;
        private bool _exported;
        private bool _paid;
        private decimal _amount;
        private LineItemCollection _items;

        internal Invoice(Guid invoiceStatusKey)
            : this(invoiceStatusKey, new Address())
        { }

        internal Invoice(Guid invoiceStatusKey, IAddress billToAddress)
            : this(invoiceStatusKey, billToAddress, new LineItemCollection())
        { }

        internal Invoice(Guid invoiceStatusKey, IAddress billToAddress, LineItemCollection lineItemCollection)
        {
            Mandate.ParameterCondition(Guid.Empty != invoiceStatusKey, "invoiceStatusKey");
            Mandate.ParameterNotNull(billToAddress, "billToAddress");
            Mandate.ParameterNotNull(lineItemCollection, "lineItemCollection");

            _invoiceStatusKey = invoiceStatusKey;

            _billToName = billToAddress.Name;
            _billToAddress1 = billToAddress.Address1;
            _billToAddress2 = billToAddress.Address2;
            _billToLocality = billToAddress.Locality;
            _billToRegion = billToAddress.Region;
            _billToPostalCode = billToAddress.PostalCode;
            _billToCountryCode = billToAddress.CountryCode;
            _billToPhone = billToAddress.Phone;

            _items = lineItemCollection;

        }

        private static readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<Invoice, Guid?>(x => x.CustomerKey);
        private static readonly PropertyInfo InvoiceNumberSelector = ExpressionHelper.GetPropertyInfo<Invoice, int>(x => x.InvoiceNumber);
        private static readonly PropertyInfo InvoiceDateSelector = ExpressionHelper.GetPropertyInfo<Invoice, DateTime>(x => x.InvoiceDate);
        private static readonly PropertyInfo InvoiceStatusKeySelector = ExpressionHelper.GetPropertyInfo<Invoice, Guid>(x => x.InvoiceStatusKey);
        private static readonly PropertyInfo BillToNameSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToName);
        private static readonly PropertyInfo BillToAddress1Selector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToAddress1);
        private static readonly PropertyInfo BillToAddress2Selector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToAddress2);
        private static readonly PropertyInfo BillToLocalitySelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToLocality);
        private static readonly PropertyInfo BillToRegionSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToRegion);
        private static readonly PropertyInfo BillToPostalCodeSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToPostalCode);
        private static readonly PropertyInfo BillToCountryCodeSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToCountryCode);
        private static readonly PropertyInfo BillToEmailSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToEmail);
        private static readonly PropertyInfo BillToPhoneSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToPhone);
        private static readonly PropertyInfo BillToCompanySelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.BillToCompany);
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Invoice, bool>(x => x.Exported);
        private static readonly PropertyInfo PaidSelector = ExpressionHelper.GetPropertyInfo<Invoice, bool>(x => x.Paid);
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<Invoice, decimal>(x => x.Amount);

        /// <summary>
        /// The unique customer 'key' to associated with the invoice
        /// </summary>
        [DataMember]
        public Guid? CustomerKey
        {
            get { return _customerKey; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _customerKey = value;
                    return _customerKey;
                }, _customerKey, CustomerKeySelector);
            }
        }

        /// <summary>
        /// The invoice number
        /// </summary>
        [DataMember]
        public int InvoiceNumber
        {
            get { return _invoiceNumber; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _invoiceNumber = value;
                    return _invoiceNumber;
                }, _invoiceNumber, InvoiceNumberSelector);
            }
        }

        /// <summary>
        /// The invoice data
        /// </summary>
        [DataMember]
        public DateTime InvoiceDate
        {
            get { return _invoiceDate; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _invoiceDate = value;
                    return _invoiceDate;
                }, _invoiceDate, InvoiceDateSelector);
            }
        }

        /// <summary>
        /// The id for the invoice status associated with this invoice
        /// </summary>
        [DataMember]
        public Guid InvoiceStatusKey
        {
            get { return _invoiceStatusKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _invoiceStatusKey = value;
                    return _invoiceStatusKey;
                }, _invoiceStatusKey, InvoiceStatusKeySelector);
            }
        }


        /// <summary>
        /// The full name to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToName
        {
            get { return _billToName; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToName = value;
                    return _billToName;
                }, _billToName, BillToNameSelector);
            }
        }

        /// <summary>
        /// The adress line 1 to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToAddress1
        {
            get { return _billToAddress1; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToAddress1 = value;
                    return _billToAddress1;
                }, _billToAddress1, BillToAddress1Selector);
            }
        }

        /// <summary>
        /// The address line 2 to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToAddress2
        {
            get { return _billToAddress2; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToAddress2 = value;
                    return _billToAddress2;
                }, _billToAddress2, BillToAddress2Selector);
            }
        }

        /// <summary>
        /// The city or locality to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToLocality
        {
            get { return _billToLocality; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToLocality = value;
                    return _billToLocality;
                }, _billToLocality, BillToLocalitySelector);
            }
        }

        /// <summary>
        /// The state, region or province to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToRegion
        {
            get { return _billToRegion; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToRegion = value;
                    return _billToRegion;
                }, _billToRegion, BillToRegionSelector);
            }
        }

        /// <summary>
        /// The postal code to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToPostalCode
        {
            get { return _billToPostalCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToPostalCode = value;
                    return _billToPostalCode;
                }, _billToPostalCode, BillToPostalCodeSelector);
            }
        }

        /// <summary>
        /// The country code to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToCountryCode
        {
            get { return _billToCountryCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToCountryCode = value;
                    return _billToCountryCode;
                }, _billToCountryCode, BillToCountryCodeSelector);
            }
        }

        /// <summary>
        /// The email address to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToEmail
        {
            get { return _billToEmail; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToEmail = value;
                    return _billToEmail;
                }, _billToEmail, BillToEmailSelector);
            }
        }

        /// <summary>
        /// The phone number to use for billing.  Generally copied from customer address.
        /// </summary
        [DataMember]
        public string BillToPhone
        {
            get { return _billToPhone; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToPhone = value;
                    return _billToPhone;
                }, _billToPhone, BillToPhoneSelector);
            }
        }

        /// <summary>
        /// The company name to use for billing.  Generally copied from customer address.
        /// </summary>
        [DataMember]
        public string BillToCompany
        {
            get { return _billToCompany; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _billToCompany = value;
                    return _billToCompany;
                }, _billToCompany, BillToCompanySelector);
            }
        }

        /// <summary>
        /// Indicates whether or not this invoice has been exported to an external system
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
        /// Indicates whether or not this invoice has been paid in full
        /// </summary>
        [DataMember]
        public bool Paid
        {
            get { return _paid; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _paid = value;
                    return _paid;
                }, _paid, PaidSelector);
            }
        }

        /// <summary>
        /// The total invoice amount
        /// </summary>
        [DataMember]
        public decimal Amount
        {
            get { return _amount; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _amount = value;
                    return _amount;
                }, _amount, AmountSelector);
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