using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Payment
{
    /// <summary>
    /// Represents a Result
    /// </summary>
    public class PaymentResult : IPaymentResult
    {
        public PaymentResult(Attempt<IPayment> result, bool approveOrderCreation)
        {
            Result = result;
            ApproveOrderCreation = approveOrderCreation;
        }

        public Attempt<IPayment> Result { get; private set; }

        public bool ApproveOrderCreation { get; private set; }
    }
}