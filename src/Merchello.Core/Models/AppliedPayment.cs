namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The applied payment.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class AppliedPayment : Entity, IAppliedPayment
    {
        /// <summary>
        /// The applied payment type field selector.
        /// </summary>
        private static readonly PropertyInfo AppliedPaymentTypeFieldSelector = ExpressionHelper.GetPropertyInfo<AppliedPayment, Guid>(x => x.AppliedPaymentTfKey);

        /// <summary>
        /// The description selector.
        /// </summary>
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<AppliedPayment, string>(x => x.Description);

        /// <summary>
        /// The amount selector.
        /// </summary>
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<AppliedPayment, decimal>(x => x.Amount);

        /// <summary>
        /// The exported selector.
        /// </summary>
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<AppliedPayment, bool>(x => x.Exported);

        /// <summary>
        /// The invoice key.
        /// </summary>
        private readonly Guid _invoiceKey;

        /// <summary>
        /// The payment key.
        /// </summary>
        private readonly Guid _paymentKey;

        /// <summary>
        /// The applied payment type field key.
        /// </summary>
        private Guid _appliedPaymentTfKey;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// The amount.
        /// </summary>
        private decimal _amount;

        /// <summary>
        /// The exported
        /// </summary>
        private bool _exported;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPayment"/> class.
        /// </summary>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="appliedPaymentType">
        /// The applied payment type.
        /// </param>
        public AppliedPayment(Guid paymentKey, Guid invoiceKey, AppliedPaymentType appliedPaymentType)
            : this(
                paymentKey,
                invoiceKey,
                EnumTypeFieldConverter.AppliedPayment.GetTypeField(appliedPaymentType).TypeKey)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPayment"/> class.
        /// </summary>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="appliedPaymentTfKey">
        /// The applied payment type field key.
        /// </param>
        internal AppliedPayment(Guid paymentKey, Guid invoiceKey, Guid appliedPaymentTfKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(paymentKey), "paymentKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
            Mandate.ParameterCondition(!Guid.Empty.Equals(appliedPaymentTfKey), "appliedPaymentTfKey");

            _paymentKey = paymentKey;
            _invoiceKey = invoiceKey;
            _appliedPaymentTfKey = appliedPaymentTfKey;
        }
        
        /// <summary>
        /// The payment key associated with the Transaction
        /// </summary>
        [DataMember]
        public Guid PaymentKey
        {
            get { return _paymentKey; }
        }

        /// <summary>
        /// The invoice key associated with the Transaction
        /// </summary>
        [DataMember]
        public Guid InvoiceKey
        {
            get { return _invoiceKey; }
        }
    
        /// <summary>
        /// The type associated with the Transaction
        /// </summary>
        [DataMember]
        public Guid AppliedPaymentTfKey
        {
            get { return _appliedPaymentTfKey; }
            internal set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _appliedPaymentTfKey = value;
                    return _appliedPaymentTfKey;
                }, _appliedPaymentTfKey, AppliedPaymentTypeFieldSelector); 
            }
        }
    
        /// <summary>
        /// The description associated with the Transaction
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return _description; }
            set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _description = value;
                    return _description;
                }, _description, DescriptionSelector); 
            }
        }
    
        /// <summary>
        /// The amount associated with the Transaction
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
        /// True/false indicating whether or not this transaction has been exported to a 3rd party system
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
        /// The transaction type associated with this transaction
        /// </summary>
        [DataMember]
        public AppliedPaymentType TransactionType
        {
            get { return EnumTypeFieldConverter.AppliedPayment.GetTypeField(_appliedPaymentTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.AppliedPayment.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    AppliedPaymentTfKey = reference.TypeKey;
                }
            }
        }
    }

}