using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    internal class AppliedPayment : IdEntity, IAppliedPayment
    {
        private readonly int _invoiceId;
        private readonly int _paymentId;
        private Guid _appliedPaymentTfKey;
        private string _description;
        private decimal _amount;
        private bool _exported;


        public AppliedPayment(IPayment payment, IInvoice invoice, AppliedPaymentType appliedPaymentType)
            : this(payment, invoice, EnumTypeFieldConverter.AppliedPayment.GetTypeField(appliedPaymentType).TypeKey)
        { }

        internal AppliedPayment (IPayment payment, IInvoice invoice, Guid appliedPaymentTfKey)
            : this(payment.Id, invoice.Id, appliedPaymentTfKey)
        { }

        internal AppliedPayment(int paymentId, int invoiceId, Guid appliedPaymentTfKey)
        {
            _paymentId = paymentId;
            _invoiceId = invoiceId;
            _appliedPaymentTfKey = appliedPaymentTfKey;
        }
        
        private static readonly PropertyInfo AppliedPaymentTypeFieldSelector = ExpressionHelper.GetPropertyInfo<AppliedPayment, Guid>(x => x.AppliedPaymentTfKey);  
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<AppliedPayment, string>(x => x.Description);  
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<AppliedPayment, decimal>(x => x.Amount);  
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<AppliedPayment, bool>(x => x.Exported);  
        
        /// <summary>
        /// The payment Id associated with the Transaction
        /// </summary>
        [DataMember]
        public int PaymentId
        {
            get { return _paymentId; }
        }

        /// <summary>
        /// The invoice Id associated with the Transaction
        /// </summary>
        [DataMember]
        public int InvoiceId
        {
            get { return _invoiceId; }
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