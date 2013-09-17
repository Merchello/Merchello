using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Configuration;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public partial class Invoice : IdEntity, IInvoice
    {
        private string _invoiceNumber;
        private DateTime _invoiceDate;
        private ICustomer _customer;
        private Guid _customerKey;
        private IInvoiceStatus _invoiceStatus;
        private int _invoiceStatusId;
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
        private bool _shipped;
        private readonly decimal _amount;

        public Invoice (ICustomer customer, IInvoiceStatus invoiceStatus, decimal amount)
        {
            _customer = customer;
            _invoiceStatus = invoiceStatus;
            _amount = amount;
        }

        public Invoice(ICustomer customer, IAddress address, IInvoiceStatus invoiceStatus, decimal amount)
            : this(customer, invoiceStatus, amount)
        {
            _billToAddress1 = address.Address1;
            _billToAddress2 = address.Address2;
            _billToLocality = address.Locality;
            _billToRegion = address.Region;
            _billToPostalCode = address.PostalCode;
            _billToCountryCode = address.CountryCode;
            _billToPhone = address.Phone;
            _billToCompany = address.Company;
        }
        
        private static readonly PropertyInfo InvoiceNumberSelector = ExpressionHelper.GetPropertyInfo<Invoice, string>(x => x.InvoiceNumber);  
        private static readonly PropertyInfo InvoiceDateSelector = ExpressionHelper.GetPropertyInfo<Invoice, DateTime>(x => x.InvoiceDate);
        private static readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<Invoice, Guid>(x => x.CustomerKey);
        private static readonly PropertyInfo InvoiceStatusIdSelector = ExpressionHelper.GetPropertyInfo<Invoice, int>(x => x.InvoiceStatusId);
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
        private static readonly PropertyInfo ShippedSelector = ExpressionHelper.GetPropertyInfo<Invoice, bool>(x => x.Shipped);




        /// <summary>
        /// The customer key associated with the Invoice
        /// </summary>
        [IgnoreDataMember]
        public Guid CustomerKey
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
        /// The customer associated with the Customer
        /// </summary>
        [DataMember]
        public ICustomer Customer
        {
            get { return _customer;}
            internal set
            {
                _customer = value;
                CustomerKey = _customer.Key;
            }
        }

        /// <summary>
        /// The invoice number associated with the Invoice
        /// </summary>
        [DataMember]
        public string InvoiceNumber
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
        /// The invoice date
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
        /// The invoiceStatusId associated with the Invoice
        /// </summary>
        [DataMember]
        public int InvoiceStatusId
        {
            get { return _invoiceStatusId; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _invoiceStatusId = value;
                    return _invoiceStatusId;
                }, _invoiceStatusId, InvoiceStatusIdSelector);
            }
        }

        /// <summary>
        /// The invoice status associated with the Invoice
        /// </summary>
        [DataMember]
        public IInvoiceStatus InvoiceStatus 
        {
            get { return _invoiceStatus; }
            set
            {
                _invoiceStatus = value;
                InvoiceStatusId = _invoiceStatus.Id;
            } 
        }

        /// <summary>
        /// The billToName associated with the Invoice
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
        /// Address line 1 of the billing addres
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
        /// The second address line of the billing address
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
        /// The city or locality of the billing address
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
        /// The region, state or province of the billing address
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
        /// The postal code of the billing address
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
        /// The country code of the billing address
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
        /// The email address associated with the billing address
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
        /// The billing phone number
        /// </summary>
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
        /// The billing company name
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
        /// True/false indicating whether or not this invoice has been exported to an external system
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
        /// True/false indicating whether or not this invoice has been paid in full
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
        /// True/false indicating whether or not that ALL items invoices have been shipped and there will be no
        /// additional shipments
        /// </summary>
        [DataMember]
        public bool Shipped
        {
            get { return _shipped; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _shipped = value;
                    return _shipped;
                }, _shipped, ShippedSelector); 
            }
        }
    
        /// <summary>
        /// The invoice total charge amount
        /// </summary>
        [DataMember]
        public decimal Amount
        {
            get { return _amount; }
        }

    }

}