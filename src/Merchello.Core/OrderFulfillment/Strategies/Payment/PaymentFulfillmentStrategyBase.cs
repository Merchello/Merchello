using System.Net.Configuration;
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

        protected PaymentFulfillmentStrategyBase(
            IPaymentService paymentService,
            IInvoiceService invoiceService,
            ITransactionService transactionService,
            TransactionType transactionType,
            bool raiseEvents = true)
        {
            Mandate.ParameterNotNull(paymentService, "paymentService");
            //Mandate.ParameterNotNull(invoiceService, "invoiceService");
            //Mandate.ParameterNotNull(transactionService, "transactionService");

            _paymentService = paymentService;
            _transactionService = transactionService;
            _invoiceService = invoiceService;
            _transactionType = transactionType;            
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

        //public abstract void PaymentInFull();
        //public abstract void PaymentPartial();
        //public abstract void PaymentNotApplied();

        public abstract void Process();
    }
}
