using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Strategies.Payment
{
    public interface IPaymentApplicationStrategyBase : IPaymentApplicationStrategy
    {
        /// <summary>
        /// The customer service
        /// </summary>
        ICustomerService CustomerService { get; }

        /// <summary>
        /// The invoice service
        /// </summary>
        IInvoiceService InvoiceService { get; }
        
    }
}
