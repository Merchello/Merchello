using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Represents a Result
    /// </summary>
    public class PaymentResult : IPaymentResult
    {
        public PaymentResult(Attempt<IPayment> payment, IInvoice invoice, bool approveOrderCreation)
        {
            Payment = payment;
            Invoice = invoice;
            ApproveOrderCreation = approveOrderCreation;
        }

        /// <summary>
        /// Gets the Result
        /// </summary>
        public Attempt<IPayment> Payment { get; private set; }

        public IInvoice Invoice { get; private set; }

        /// <summary>
        /// True/false indicating whether or not an order should be generated
        /// as a result of this payment
        /// </summary>
        public bool ApproveOrderCreation { get; internal set; }
    }
}