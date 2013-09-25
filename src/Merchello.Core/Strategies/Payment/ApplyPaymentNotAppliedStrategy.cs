using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.OrderFulfillment.Strategies.Payment
{
    /// <summary>
    /// Defines a payment without a transaction
    /// </summary>
    public class ApplyPaymentNotAppliedStrategy : ApplyPaymentStrategyBase
    {
        private readonly IPayment _payment;
        private readonly bool _raiseEvents;

        public ApplyPaymentNotAppliedStrategy(IPayment payment, bool raiseEvents = true)
            : this(new PaymentService(), new InvoiceService(), new TransactionService(),payment, raiseEvents)
        { }

        public ApplyPaymentNotAppliedStrategy(
            IPaymentService paymentService, 
            IInvoiceService invoiceService,
            ITransactionService transactionService, 
            IPayment payment, 
            bool raiseEvents = true)
            : base(paymentService, invoiceService, transactionService, TransactionType.Credit, raiseEvents)
        {
            Mandate.ParameterNotNull(payment, "payment");

            _payment = payment;
            _raiseEvents = raiseEvents;
        }

        public override void Process()
        {
            PaymentService.SaveNotApplied(_payment, _raiseEvents);
        }
    }
}
