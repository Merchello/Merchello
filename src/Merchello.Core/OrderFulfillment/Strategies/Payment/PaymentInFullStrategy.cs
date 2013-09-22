using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Umbraco.Core.Auditing;

namespace Merchello.Core.OrderFulfillment.Strategies.Payment
{
    public class PaymentInFullStrategy : PaymentFulfillmentStrategyBase
    {
        private readonly IPayment _payment;
        private readonly IInvoice _invoice;
        private readonly string _description;
        
        public PaymentInFullStrategy(IPayment payment, IInvoice invoice, string description = "", bool raiseEvents = true)
            : base(TransactionType.Credit, raiseEvents)
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

            // save the payment if it is dirty or if it has not yet been payed
            if(_payment.IsDirty() || !_payment.HasIdentity) PaymentService.Save(_payment);

            // the transaction
            var transaction = TransactionService.CreateTransaction(_payment, _invoice, TransactionType.Credit, _payment.Amount);
            transaction.Description = _description;
            TransactionService.Save(transaction, RaiseEvents);

            _invoice.Paid = true;
            InvoiceService.Save(_invoice,RaiseEvents);
        }

    }
}
