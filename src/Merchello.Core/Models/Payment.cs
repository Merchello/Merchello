using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public partial class Payment : IdEntity, IPayment
    {
        
        private ICustomer _customer;
        private Guid _customerKey;
        private string _gatewayAlias;
        private Guid _paymentTypeFieldKey;
        private string _paymentMethodName;
        private string _referenceNumber;
        private decimal _amount;
        private bool _exported;

        public Payment(ICustomer customer, PaymentMethodType paymentMethodType, decimal amount)
            : this(customer, EnumeratedTypeFieldConverter.PaymentMethod().GetTypeField(paymentMethodType).TypeKey, amount)
        { }

        internal Payment (ICustomer customer, Guid paymentTypeFieldKey, decimal amount)  
        {
            // customer can make a payment without an invoice (credit) so invoice can be null
            Mandate.ParameterNotNull(customer, "customer");
           
            _customer = customer;
            _customerKey = customer.Key;            
            _amount = amount;
            _paymentTypeFieldKey = paymentTypeFieldKey;
        }
        

        private static readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid>(x => x.CustomerKey); 
        private static readonly PropertyInfo GatewayAliasSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.GatewayAlias);  
        private static readonly PropertyInfo PaymentTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid>(x => x.PaymentTypeFieldKey);  
        private static readonly PropertyInfo PaymentMethodNameSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.PaymentMethodName);  
        private static readonly PropertyInfo ReferenceNumberSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.ReferenceNumber);  
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<Payment, decimal>(x => x.Amount);  
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Exported);  
        
    
        /// <summary>
        /// The customerKey associated with the Payment
        /// </summary>
        [IgnoreDataMember]
        public Guid CustomerKey
        {
            get { return _customer.Key; }
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
                CustomerKey = _customer.Key;
            }
        }

        /// <summary>
        /// The gatewayAlias associated with the Payment
        /// </summary>
        [DataMember]
        public string GatewayAlias
        {
            get { return _gatewayAlias; }
                set 
                { 
                    SetPropertyValueAndDetectChanges(o =>
                    {
                        _gatewayAlias = value;
                        return _gatewayAlias;
                    }, _gatewayAlias, GatewayAliasSelector); 
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
            get { return EnumeratedTypeFieldConverter.PaymentMethod().GetTypeField(_paymentTypeFieldKey); }
            set
            {
                var reference = EnumeratedTypeFieldConverter.PaymentMethod().GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    PaymentTypeFieldKey = reference.TypeKey;
                }
            }
        }
        
    }

}