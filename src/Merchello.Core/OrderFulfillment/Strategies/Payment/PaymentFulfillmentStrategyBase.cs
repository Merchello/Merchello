using Merchello.Core.Services;

namespace Merchello.Core.OrderFulfillment.Strategies.Payment
{
    /// <summary>
    /// Defines a payment fulfillment strategy
    /// </summary>
    public abstract class PaymentFulfillmentStrategyBase : IPaymentFulfillmnetStrategy
    {
        private readonly ITransactionService _transactionService;
        private readonly IPaymentService _paymentService;
        private readonly IInvoiceService _invoiceService;
        private readonly TransactionType _transactionType;
        private readonly bool _raiseEvents;

        protected PaymentFulfillmentStrategyBase(TransactionType transactionType, bool raiseEvents = true)
            : this(transactionType, MerchelloContext.Current.Services.PaymentService, raiseEvents)
        { }

        internal PaymentFulfillmentStrategyBase(TransactionType transactionType, IPaymentService paymentService, bool raiseEvents = true)
            : this(transactionType, paymentService, ((ServiceContext) MerchelloContext.Current.Services).TransactionService, MerchelloContext.Current.Services.InvoiceService, raiseEvents)
        {
        }

        internal PaymentFulfillmentStrategyBase(TransactionType transactionType, IPaymentService paymentService, ITransactionService transactionService, IInvoiceService invoiceService, bool raiseEvents = true)
        {            
            Mandate.ParameterNotNull(paymentService, "paymentService");
            Mandate.ParameterNotNull(transactionService, "transactionService");
            Mandate.ParameterNotNull(invoiceService, "invoiceService");

            _paymentService = paymentService;
            _transactionService = transactionService;
            _transactionType = transactionType;
            _invoiceService = invoiceService;
            _raiseEvents = raiseEvents;
        }

        /// <summary>
        /// The payment service
        /// </summary>
        public IPaymentService PaymentService
        {
            get { return _paymentService; }
        }

        /// <summary>
        /// The transaction service
        /// </summary>
        public ITransactionService TransactionService
        {
            get { return _transactionService; }
        }

        /// <summary>
        /// The invoice service
        /// </summary>
        public IInvoiceService InvoiceService
        {
            get { return _invoiceService; }
        }

        /// <summary>
        /// The transaction type
        /// </summary>
        public TransactionType TransactionType
        {
            get { return _transactionType; }    
        }

        /// <summary>
        /// True/false indicating whether or not to raise events in providers
        /// </summary>
        public bool RaiseEvents
        {
            get { return _raiseEvents; }
        }

        public abstract void Process();
    }
}
