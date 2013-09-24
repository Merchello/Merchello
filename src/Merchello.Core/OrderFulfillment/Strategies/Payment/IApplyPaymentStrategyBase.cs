using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Services;

namespace Merchello.Core.OrderFulfillment.Strategies.Payment
{
    public interface IApplyPaymentStrategyBase : IApplyPaymentStrategy
    {
        /// <summary>
        /// The transaction service
        /// </summary>
        ITransactionService TransactionService { get; }

        /// <summary>
        /// The invoice service
        /// </summary>
        IInvoiceService InvoiceService { get; }
        
    }
}
