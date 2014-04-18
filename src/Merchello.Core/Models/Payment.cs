using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class Payment : Entity, IPayment
    {
        
        private Guid? _customerKey;
        private Guid? _paymentMethodKey;
        private Guid _paymentTypeFieldKey;
        private string _paymentMethodName;
        private string _referenceNumber;
        private decimal _amount;
        private bool _authorized;
        private bool _collected;
        private bool _exported;
        private bool _voided;
        private ExtendedDataCollection _extendedData;

        internal Payment(PaymentMethodType paymentMethodType, decimal amount)
            : this(paymentMethodType, amount, null, new ExtendedDataCollection())
        { }

        internal Payment(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey)
            : this(paymentMethodType, amount, paymentMethodKey, new ExtendedDataCollection())
        {}

        internal Payment(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey, ExtendedDataCollection extendedData)
            : this(EnumTypeFieldConverter.PaymentMethod.GetTypeField(paymentMethodType).TypeKey, amount, paymentMethodKey, extendedData)
        { }

        internal Payment(Guid paymentTypeFieldKey, decimal amount, Guid? paymentMethodKey, ExtendedDataCollection extendedData)  
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(paymentTypeFieldKey), "paymentTypeFieldKey");
            Mandate.ParameterNotNull(extendedData, "extendedData");
            

            _amount = amount;
            _paymentMethodKey = paymentMethodKey;
            _paymentTypeFieldKey = paymentTypeFieldKey;
            _extendedData = extendedData;
        }        

        private static readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid?>(x => x.CustomerKey); 
        private static readonly PropertyInfo PaymentMethodKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid?>(x => x.PaymentMethodKey);  
        private static readonly PropertyInfo PaymentTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid>(x => x.PaymentTypeFieldKey);  
        private static readonly PropertyInfo PaymentMethodNameSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.PaymentMethodName);  
        private static readonly PropertyInfo ReferenceNumberSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.ReferenceNumber);  
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<Payment, decimal>(x => x.Amount);
        private static readonly PropertyInfo AuthorizedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Authorized);
        private static readonly PropertyInfo CollectedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Collected);
        private static readonly PropertyInfo VoidedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Voided);
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Exported);
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);

        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }


        /// <summary>
        /// The customerKey associated with the Payment
        /// </summary>
        [IgnoreDataMember]
        public Guid? CustomerKey
        {
            get { return _customerKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _customerKey = value;
                    return _customerKey;
                }, _customerKey, CustomerKeySelector);
            }
        }
   

        /// <summary>
        /// The payment method key associated with the fulfillment provider for this payment
        /// </summary>
        [DataMember]
        public Guid? PaymentMethodKey
        {
            get { return _paymentMethodKey; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _paymentMethodKey = value;
                    return _paymentMethodKey;
                }, _paymentMethodKey, PaymentMethodKeySelector); 
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
        /// True/false indicating whether or not the payment has been voided
        /// </summary>
        [DataMember]
        public bool Voided
        {
            get { return _voided; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _voided = value;
                    return _voided;
                }, _voided, VoidedSelector);
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

        /// <summary>
        /// A collection to store custom/extended data for the payment
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get { return _extendedData; }
            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
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