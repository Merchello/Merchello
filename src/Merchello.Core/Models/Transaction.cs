using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public class Transaction : IdEntity, ITransaction
    {
        private readonly int _invoiceId;
        private readonly int _paymentId;
        private Guid _transactionTypeFieldKey;
        private string _description;
        private decimal _amount;
        private bool _exported;


        public Transaction(IPayment payment, IInvoice invoice, TransactionType transactionType)
            : this(payment, invoice, EnumTypeFieldConverter.Transaction().GetTypeField(transactionType).TypeKey)
        { }

        internal Transaction (IPayment payment, IInvoice invoice, Guid transactionTypeFieldKey)
            : this(payment.Id, invoice.Id, transactionTypeFieldKey)
        { }

        internal Transaction(int paymentId, int invoiceId, Guid transactionTypeFieldKey)
        {
            _paymentId = paymentId;
            _invoiceId = invoiceId;
            _transactionTypeFieldKey = transactionTypeFieldKey;
        }
        
        private static readonly PropertyInfo TransactionTypeFieldSelector = ExpressionHelper.GetPropertyInfo<Transaction, Guid>(x => x.TransactionTypeFieldKey);  
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<Transaction, string>(x => x.Description);  
        private static readonly PropertyInfo AmountSelector = ExpressionHelper.GetPropertyInfo<Transaction, decimal>(x => x.Amount);  
        private static readonly PropertyInfo ExportedSelector = ExpressionHelper.GetPropertyInfo<Transaction, bool>(x => x.Exported);  
        
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
        public Guid TransactionTypeFieldKey
        {
            get { return _transactionTypeFieldKey; }
            internal set 
            { 
                SetPropertyValueAndDetectChanges(o =>
                {
                    _transactionTypeFieldKey = value;
                    return _transactionTypeFieldKey;
                }, _transactionTypeFieldKey, TransactionTypeFieldSelector); 
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
        public TransactionType TransactionType
        {
            get { return EnumTypeFieldConverter.Transaction().GetTypeField(_transactionTypeFieldKey); }
            set
            {
                var reference = EnumTypeFieldConverter.Transaction().GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    TransactionTypeFieldKey = reference.TypeKey;
                }
            }
        }
    }

}