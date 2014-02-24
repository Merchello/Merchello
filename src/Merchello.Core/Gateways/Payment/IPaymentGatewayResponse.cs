using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Payment
{
    public interface IPaymentGatewayResponse
    {
        IPayment Payment { get; set; }

        IEnumerable<IAppliedPayment> AppliedPayments { get; set; }

        //IEnumerable<IInvoice> GenerateOrdersForInvoices  
    }
}