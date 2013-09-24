using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.OrderFulfillment.Strategies.Payment
{
    public class PaymentApplicationStrategy : PaymentApplicationStrategyBase
    {
        public PaymentApplicationStrategy()
            : this(new InvoiceService(), new TransactionService())
        { }

        // This is the constructor referenced in the ServiceContext
        public PaymentApplicationStrategy(IInvoiceService invoiceService, ITransactionService transactionService)
            : base(invoiceService, transactionService)
        { }

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
        public override void ApplyPayment(
            IPayment payment, 
            IInvoice invoice, 
            decimal amount, 
            TransactionType transactionType = TransactionType.Credit,
            string transactionDescription = "", 
            bool raiseEvents = true)
        {   
            // the transaction
            var transaction = TransactionService.CreateTransaction(payment, invoice, TransactionType.Credit, amount);
            transaction.Description = transactionDescription;
            TransactionService.Save(transaction, raiseEvents);

            invoice.Paid = true;
            InvoiceService.Save(invoice, raiseEvents);
        }

        public override void VoidPayment(IPayment payment, string transactionDescription = "", bool raiseEvents = true)
        {
            throw new System.NotImplementedException();
        }
    }
}
