using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class Payment : IdEntity, IPayment
    {
        
        private ICustomer _customer;
        private int _customerId;
        private Guid _providerKey;
        private Guid _paymentTypeFieldKey;
        private string _paymentMethodName;
        private string _referenceNumber;
        private decimal _amount;
        private bool _authorized;
        private bool _collected;
        private bool _exported;

        internal Payment(ICustomer customer, PaymentMethodType paymentMethodType, decimal amount)
            : this(customer, EnumTypeFieldConverter.PaymentMethod.GetTypeField(paymentMethodType).TypeKey, amount)
        { }

        internal Payment (ICustomer customer, Guid paymentTypeFieldKey, decimal amount)  
        {
            // customer can make a payment without an invoice (credit) so invoice can be null
            Mandate.ParameterNotNull(customer, "customer");
           
            _customer = customer;
            _customerId = customer.Id;            
            _amount = amount;
            _paymentTypeFieldKey = paymentTypeFieldKey;
        }        

        private static readonly PropertyInfo CustomerIdSelector = ExpressionHelper.GetPropertyInfo<Payment, int>(x => x.CustomerId); 
        private static readonly PropertyInfo ProviderKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid>(x => x.ProviderKey);  
        private static readonly PropertyInfo PaymentTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid>(x => x.PaymentTypeFieldKey);  
        private static readonly PropertyInfo PaymentMethodNameSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.PaymentMethodName);  
        private static readonly PropertyInfo ReferenceNumberSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.ReferenceNumber);  
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<Payment, decimal>(x => x.Amount);
        private static readonly PropertyInfo AuthorizedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Authorized);
        private static readonly PropertyInfo CollectedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Collected);
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Exported);        
    
        /// <summary>
        /// The customerKey associated with the Payment
        /// </summary>
        [IgnoreDataMember]
        public int CustomerId
        {
            get { return _customer.Id; }
            internal set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _customerId = value;
                    return _customerId;
                }, _customerId, CustomerIdSelector);
            }
        }
    
        /// <summary>
        /// The customer assoicated with the Payment
        /// </summary>
        [DataMember]
        public ICustomer Customer
        {
            get { return _customer;  }
            set
            {
                Mandate.ParameterNotNull(value, "value");
                _customer = value;
                CustomerId = _customer.Id;
            }
        }

        /// <summary>
        /// The provider key associated with the fulfillment provider for this payment
        /// </summary>
        [DataMember]
        public Guid ProviderKey
        {
            get { return _providerKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _providerKey = value;
                    return _providerKey;
                }, _providerKey, ProviderKeySelector); 
            }
        }
    
        /// <summary>
        /// The paymentTypeFieldKey associated with the Payment
        /// </summary>
        [DataMember]
        public Guid PaymentTypeFieldKey
        {
            get { return _paymentTypeFieldKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _paymentTypeFieldKey = value;
                    return _paymentTypeFieldKey;
                }, _paymentTypeFieldKey, PaymentTypeFieldKeySelector); 
            }
        }
    
        /// <summary>
        /// The paymentMethodName associated with the Payment
        /// </summary>
        [DataMember]
        public string PaymentMethodName
        {
            get { return _paymentMethodName; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _paymentMethodName = value;
                    return _paymentMethodName;
                }, _paymentMethodName, PaymentMethodNameSelector); 
            }
        }
    
        /// <summary>
        /// The referenceNumber associated with the Payment
        /// </summary>
        [DataMember]
        public string ReferenceNumber
        {
            get { return _referenceNumber; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _referenceNumber = value;
                    return _referenceNumber;
                }, _referenceNumber, ReferenceNumberSelector); 
            }
        }
    
        /// <summary>
        /// The amount associated with the Payment
        /// </summary>
        [DataMember]
        public decimal Amount
        {
            get { return _amount; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _amount = value;
                    return _amount;
                }, _amount, AmountSelector); 
            }
        }

        /// <summary>
        /// True/False indicating whether or not this payment has been authorized with the payment gateway provider
        /// </summary>
        [DataMember]
        public bool Authorized
        {
            get { return _authorized; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _authorized = value;
                    return _authorized;
                }, _authorized, AuthorizedSelector);
            }
        }

        /// <summary>
        /// True/False indicating whether or not this payment has been collected by the merchant
        /// </summary>
        [DataMember]
        public bool Collected
        {
            get { return _collected; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _collected = value;
                    return _collected;
                }, _collected, CollectedSelector);
            }
        }

        /// <summary>
        /// The exported associated with the Payment
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
    
        [DataMember]
        public PaymentMethodType PaymentMethodType
        {
            get { return EnumTypeFieldConverter.PaymentMethod.GetTypeField(_paymentTypeFieldKey); }
            set
            {
                var reference = EnumTypeFieldConverter.PaymentMethod.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    PaymentTypeFieldKey = reference.TypeKey;
                }
            }
        }
        
    }

}