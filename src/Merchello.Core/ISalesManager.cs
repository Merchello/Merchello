using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core
{
    /// <summary>
    /// Defines the sales manager singleton
    /// </summary>
    public interface ISalesManager
    {
        IInvoice GenerateInvoice();

        Attempt<IPayment> CollectPayment(IInvoice invoice, IPaymentGatewayProvider paymentGatewayProvider);
    }
}