namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    using Umbraco.Core;

    /// <summary>
    /// Represents a payment.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Payment : Entity, IPayment
    {
        #region Fields

        /// <summary>
        /// The customer key selector.
        /// </summary>
        private static readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid?>(x => x.CustomerKey);

        /// <summary>
        /// The payment method key selector.
        /// </summary>
        private static readonly PropertyInfo PaymentMethodKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid?>(x => x.PaymentMethodKey);

        /// <summary>
        /// The payment type field key selector.
        /// </summary>
        private static readonly PropertyInfo PaymentTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid>(x => x.PaymentTypeFieldKey);

        /// <summary>
        /// The payment method name selector.
        /// </summary>
        private static readonly PropertyInfo PaymentMethodNameSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.PaymentMethodName);

        /// <summary>
        /// The reference number selector.
        /// </summary>
        private static readonly PropertyInfo ReferenceNumberSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.ReferenceNumber);

        /// <summary>
        /// The amount selector.
        /// </summary>
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<Payment, decimal>(x => x.Amount);

        /// <summary>
        /// The authorized selector.
        /// </summary>
        private static readonly PropertyInfo AuthorizedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Authorized);

        /// <summary>
        /// The collected selector.
        /// </summary>
        private static readonly PropertyInfo CollectedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Collected);

        /// <summary>
        /// The voided selector.
        /// </summary>
        private static readonly PropertyInfo VoidedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Voided);

        /// <summary>
        /// The exported selector.
        /// </summary>
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Exported);

        /// <summary>
        /// The extended data changed selector.
        /// </summary>
        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);

        /// <summary>
        /// The customer key.
        /// </summary>
        private Guid? _customerKey;

        /// <summary>
        /// The payment method key.
        /// </summary>
        private Guid? _paymentMethodKey;

        /// <summary>
        /// The payment type field key.
        /// </summary>
        private Guid _paymentTypeFieldKey;

        /// <summary>
        /// The payment method name.
        /// </summary>
        private string _paymentMethodName;

        /// <summary>
        /// The reference number.
        /// </summary>
        private string _referenceNumber;

        /// <summary>
        /// The amount.
        /// </summary>
        private decimal _amount;

        /// <summary>
        /// The authorized.
        /// </summary>
        private bool _authorized;

        /// <summary>
        /// The collected.
        /// </summary>
        private bool _collected;

        /// <summary>
        /// The exported.
        /// </summary>
        private bool _exported;

        /// <summary>
        /// The voided.
        /// </summary>
        private bool _voided;

        /// <summary>
        /// The extended data.
        /// </summary>
        private ExtendedDataCollection _extendedData;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Payment"/> class.
        /// </summary>
        /// <param name="paymentMethodType">
        /// The payment method type.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        internal Payment(PaymentMethodType paymentMethodType, decimal amount)
            : this(paymentMethodType, amount, null, new ExtendedDataCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payment"/> class.
        /// </summary>
        /// <param name="paymentMethodType">
        /// The payment method type.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="paymentMethodKey">
        /// The payment method key.
        /// </param>
        internal Payment(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey)
            : this(paymentMethodType, amount, paymentMethodKey, new ExtendedDataCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payment"/> class.
        /// </summary>
        /// <param name="paymentMethodType">
        /// The payment method type.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="paymentMethodKey">
        /// The payment method key.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        internal Payment(PaymentMethodType paymentMethodType, decimal amount, Guid? paymentMethodKey, ExtendedDataCollection extendedData)
            : this(EnumTypeFieldConverter.PaymentMethod.GetTypeField(paymentMethodType).TypeKey, amount, paymentMethodKey, extendedData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payment"/> class.
        /// </summary>
        /// <param name="paymentTypeFieldKey">
        /// The payment type field key.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="paymentMethodKey">
        /// The payment method key.
        /// </param>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        internal Payment(Guid paymentTypeFieldKey, decimal amount, Guid? paymentMethodKey, ExtendedDataCollection extendedData)  
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(paymentTypeFieldKey), "paymentTypeFieldKey");
            Mandate.ParameterNotNull(extendedData, "extendedData");
            

            _amount = amount;
            _paymentMethodKey = paymentMethodKey;
            _paymentTypeFieldKey = paymentTypeFieldKey;
            _extendedData = extendedData;
        }        

        /// <summary>
        /// Gets or sets the customerKey associated with the Payment
        /// </summary>
        [IgnoreDataMember]
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
        /// Gets or sets the payment method key associated with the fulfillment provider for this payment
        /// </summary>
        [DataMember]
        public Guid? PaymentMethodKey
        {
            get
            {
                return _paymentMethodKey;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _paymentMethodKey = value;
                        return _paymentMethodKey;
                    }, 
                    _paymentMethodKey, 
                    PaymentMethodKeySelector); 
            }
        }
    
        /// <summary>
        /// Gets or sets the paymentTypeFieldKey associated with the Payment
        /// </summary>
        [DataMember]
        public Guid PaymentTypeFieldKey
        {
            get
            {
                return _paymentTypeFieldKey;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _paymentTypeFieldKey = value;
                        return _paymentTypeFieldKey;
                    }, 
                    _paymentTypeFieldKey, 
                    PaymentTypeFieldKeySelector); 
            }
        }
    
        /// <summary>
        /// Gets or sets the paymentMethodName associated with the Payment
        /// </summary>
        [DataMember]
        public string PaymentMethodName
        {
            get
            {
                return _paymentMethodName;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _paymentMethodName = value;
                        return _paymentMethodName;
                    }, 
                    _paymentMethodName, 
                    PaymentMethodNameSelector); 
            }
        }
    
        /// <summary>
        /// Gets or sets the referenceNumber associated with the Payment
        /// </summary>
        [DataMember]
        public string ReferenceNumber
        {
            get
            {
                return _referenceNumber;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _referenceNumber = value;
                        return _referenceNumber;
                    }, 
                    _referenceNumber, 
                    ReferenceNumberSelector); 
            }
        }
    
        /// <summary>
        /// Gets or sets the amount associated with the Payment
        /// </summary>
        [DataMember]
        public decimal Amount
        {
            get
            {
                return _amount;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _amount = value;
                        return _amount;
                    }, 
                    _amount, 
                    AmountSelector); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has been authorized with the payment gateway provider
        /// </summary>
        [DataMember]
        public bool Authorized
        {
            get
            {
                return _authorized;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _authorized = value;
                        return _authorized;
                    }, 
                    _authorized, 
                    AuthorizedSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this payment has been collected by the merchant
        /// </summary>
        [DataMember]
        public bool Collected
        {
            get
            {
                return _collected;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _collected = value;
                        return _collected;
                    }, 
                    _collected, 
                    CollectedSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the payment has been voided
        /// </summary>
        [DataMember]
        public bool Voided
        {
            get
            {
                return _voided;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _voided = value;
                        return _voided;
                    }, 
                    _voided, 
                    VoidedSelector);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the payment has been exported
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
        /// Gets the collection to store custom/extended data for the payment
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get
            {
                return _extendedData;
            }

            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// Gets or sets the payment method type.
        /// </summary>
        [DataMember]
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return EnumTypeFieldConverter.PaymentMethod.GetTypeField(_paymentTypeFieldKey);
            }

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

        /// <summary>
        /// The extended data changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }
    }

}