using Merchello.Core.Models;

namespace Merchello.Core.Chains.Invoices
{
    public class InvoiceAttemptChainTaskHandler : AttemptChainTaskHandler<IInvoice>
    {
        public InvoiceAttemptChainTaskHandler(InvoiceAttemptChainTaskBase task) 
            : base(task)
        { }
    }
}