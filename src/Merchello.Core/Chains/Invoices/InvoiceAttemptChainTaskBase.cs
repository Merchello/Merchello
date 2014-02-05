using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.Invoices
{
    public abstract class InvoiceAttemptChainTaskBase : IAttemptChainTask<IInvoice>
    {
        public abstract Attempt<IInvoice> PerformTask(IInvoice arg);

    }
}