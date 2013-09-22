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
    public class PaymentNotAppliedStrategy : PaymentFulfillmentStrategyBase
    {
        private readonly IPayment _payment;
        private bool _raiseEvents;

        public PaymentNotAppliedStrategy(IPayment payment, bool raiseEvents = true)
            : base(TransactionType.Credit)
        {
            _payment = payment;
            _raiseEvents = raiseEvents;
        }

        public override void Process()
        {
            ((PaymentService)PaymentService).SaveNotApplied(_payment, _raiseEvents);
        }
    }
}
