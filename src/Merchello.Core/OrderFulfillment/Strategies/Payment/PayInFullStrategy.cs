using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.OrderFulfillment.Strategies.Payment
{
    public class PayInFullStrategy : PaymentFulfillmentStrategyBase
    {
        private readonly IPayment _payment;
        private readonly IInvoice _invoice;
        private readonly string _description;


        public PayInFullStrategy(IPayment payment, IInvoice invoice, string description = "", bool raiseEvents = true)
            : this(new PaymentService(), new InvoiceService(), new TransactionService(), payment, invoice, description, raiseEvents)
        { }

        public PayInFullStrategy(IPaymentService paymentService, IInvoiceService invoiceService, ITransactionService transactionService, IPayment payment, IInvoice invoice, string description = "", bool raiseEvents = true)
            : base(paymentService, invoiceService, transactionService, TransactionType.Credit, raiseEvents)
        {
            Mandate.ParameterNotNull(payment, "payment");
            Mandate.ParameterNotNull(invoice, "invoice");

            _payment = payment;
            _invoice = invoice;
            _description = description;
        }

        public override void Process()
        {
            Mandate.ParameterCondition(_payment.Amount == _invoice.Amount, "Payment amount must equal the invoice amount to use the PaymentInFullStrategy");

            // the payment
            PaymentService.Save(_payment);


            // the transaction
            var transaction = TransactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, _payment.Amount);
            transaction.Description = _description;
            TransactionService.Save(transaction, RaiseEvents);

            _invoice.Paid = true;
            InvoiceService.Save(_invoice,RaiseEvents);
        }

    }
}
