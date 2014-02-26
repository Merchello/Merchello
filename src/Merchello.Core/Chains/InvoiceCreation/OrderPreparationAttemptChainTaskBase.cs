using Merchello.Core.Models;
using Merchello.Core.Sales;

namespace Merchello.Core.Chains.InvoiceCreation
{
    public abstract class OrderPreparationAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly SalesManagerBase _salesManager;

        protected OrderPreparationAttemptChainTaskBase(SalesManagerBase salesManager)
        {
            Mandate.ParameterNotNull(salesManager, "checkout");

            _salesManager = salesManager;
        }

        /// <summary>
        /// Gets the <see cref="SalesManagerBase"/> object
        /// </summary>
        protected SalesManagerBase SalesManager
        {
            get { return _salesManager; }
        }
    }
}