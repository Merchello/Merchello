using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    [Serializable]
    [DataContract(IsReference = true)]
    public partial class Transaction : IdEntity, ITransaction
    {
        private readonly IInvoice _invoice;
        private readonly IPayment _payment;
        private Guid _transactionTypeFieldKey;
        private string _description;
        private decimal _amount;
        private bool _exported;


        public Transaction(IPayment payment, IInvoice invoice, TransactionType transactionType)
            : this(payment, invoice, EnumeratedTypeFieldConverter.Transaction().GetTypeField(transactionType).TypeKey)
        { }

        internal Transaction (IPayment payment, IInvoice invoice, Guid transactionTypeFieldKey)
        {
            _payment = payment;
            _invoice = invoice;
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
            get { return _payment.Id; }
        }

        /// <summary>
        /// The invoice Id associated with the Transaction
        /// </summary>
        [DataMember]
        public int InvoiceId
        {
            get { return _invoice.Id; }
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
            get { return EnumeratedTypeFieldConverter.Transaction().GetTypeField(_transactionTypeFieldKey); }
            set
            {
                var reference = EnumeratedTypeFieldConverter.Transaction().GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    TransactionTypeFieldKey = reference.TypeKey;
                }
            }
        }
    }

}