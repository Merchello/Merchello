namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// Represents a payment.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Payment : Entity, IPayment
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
            Ensure.ParameterCondition(!Guid.Empty.Equals(paymentTypeFieldKey), "paymentTypeFieldKey");
            Ensure.ParameterNotNull(extendedData, "extendedData");
            
            _amount = amount;
            _paymentMethodKey = paymentMethodKey;
            _paymentTypeFieldKey = paymentTypeFieldKey;
            _extendedData = extendedData;
        }        

        /// <inheritdoc/>
        [IgnoreDataMember]
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
        public Guid? PaymentMethodKey
        {
            get
            {
                return _paymentMethodKey;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _paymentMethodKey, _ps.Value.PaymentMethodKeySelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid PaymentTypeFieldKey
        {
            get
            {
                return _paymentTypeFieldKey;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _paymentTypeFieldKey, _ps.Value.PaymentTypeFieldKeySelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string PaymentMethodName
        {
            get
            {
                return _paymentMethodName;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _paymentMethodName, _ps.Value.PaymentMethodNameSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string ReferenceNumber
        {
            get
            {
                return _referenceNumber;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _referenceNumber, _ps.Value.ReferenceNumberSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public decimal Amount
        {
            get
            {
                return _amount;
            }

            set 
            { 
                SetPropertyValueAndDetectChanges(value, ref _amount, _ps.Value.AmountSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Authorized
        {
            get
            {
                return _authorized;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _authorized, _ps.Value.AuthorizedSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Collected
        {
            get
            {
                return _collected;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _collected, _ps.Value.CollectedSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Voided
        {
            get
            {
                return _voided;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _voided, _ps.Value.VoidedSelector);
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

        /// <inheritdoc/>
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
        /// Handles the extended data collection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.ExtendedDataChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The customer key selector.
            /// </summary>
            public readonly PropertyInfo CustomerKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid?>(x => x.CustomerKey);

            /// <summary>
            /// The payment method key selector.
            /// </summary>
            public readonly PropertyInfo PaymentMethodKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid?>(x => x.PaymentMethodKey);

            /// <summary>
            /// The payment type field key selector.
            /// </summary>
            public readonly PropertyInfo PaymentTypeFieldKeySelector = ExpressionHelper.GetPropertyInfo<Payment, Guid>(x => x.PaymentTypeFieldKey);

            /// <summary>
            /// The payment method name selector.
            /// </summary>
            public readonly PropertyInfo PaymentMethodNameSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.PaymentMethodName);

            /// <summary>
            /// The reference number selector.
            /// </summary>
            public readonly PropertyInfo ReferenceNumberSelector = ExpressionHelper.GetPropertyInfo<Payment, string>(x => x.ReferenceNumber);

            /// <summary>
            /// The amount selector.
            /// </summary>
            public readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<Payment, decimal>(x => x.Amount);

            /// <summary>
            /// The authorized selector.
            /// </summary>
            public readonly PropertyInfo AuthorizedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Authorized);

            /// <summary>
            /// The collected selector.
            /// </summary>
            public readonly PropertyInfo CollectedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Collected);

            /// <summary>
            /// The voided selector.
            /// </summary>
            public readonly PropertyInfo VoidedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Voided);

            /// <summary>
            /// The exported selector.
            /// </summary>
            public readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Payment, bool>(x => x.Exported);

            /// <summary>
            /// The extended data changed selector.
            /// </summary>
            public readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);
        }
    }
}