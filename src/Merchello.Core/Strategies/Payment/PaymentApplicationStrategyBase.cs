using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Strategies.Payment
{
    /// <summary>
    /// Defines a payment fulfillment strategy
    /// </summary>
    public abstract class PaymentApplicationStrategyBase : IPaymentApplicationStrategyBase
    {
        private readonly ICustomerService _customerService;
        private readonly ITransactionService _transactionService;
        private readonly IInvoiceService _invoiceService;


        // TODO : Figure out a way to require that sub classes are forced to implement
        // a constructor with this signature as it is used in the ServiceContext
        protected PaymentApplicationStrategyBase(
            ICustomerService customerService,
            IInvoiceService invoiceService,
            ITransactionService transactionService)
        {
            Mandate.ParameterNotNull(customerService, "customerService");
            Mandate.ParameterNotNull(invoiceService, "invoiceService");
            Mandate.ParameterNotNull(transactionService, "transactionService");

            _customerService = customerService;
            _transactionService = transactionService;
            _invoiceService = invoiceService;
        }

        #region Overrides IApplyPaymentStrategyBase

        /// <summary>
        /// The customer service
        /// </summary>
        public ICustomerService CustomerService {
            get { return _customerService; }
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

        #endregion

        #region Overrides IApplyPaymentStrategy


        /// <summary>
        /// Performs the actual work of the apply payment transaction
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/> to be applied in the transaction</param>
        /// <param name="invoice">The <see cref="IInvoice"/> to which the payment is to be applied</param>
        /// <param name="amount">The amount of the <see cref="IPayment"/> to be applied to the <see cref="IInvoice"/>.  This could
        /// be a partial payment
        /// </param>
        /// <param name="transactionType">The <see cref="TransactionType"/> of the resulting transaction created</param>
        /// <param name="transactionDescription">An optional description for the transaction</param>
        /// <param name="raiseEvents">True/False indicating whether or not any service providers required to make the transaction should raise events</param>
        public abstract void ApplyPayment(
            IPayment payment,
            IInvoice invoice,
            decimal amount,
            TransactionType transactionType = TransactionType.Credit,
            string transactionDescription = "",
            bool raiseEvents = true);


        public abstract void VoidPayment(IPayment payment, string transactionDescription = "", bool raiseEvents = true);


        #endregion

    }
}
