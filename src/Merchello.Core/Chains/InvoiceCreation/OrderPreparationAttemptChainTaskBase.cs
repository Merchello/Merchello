using Merchello.Core.Models;
using Merchello.Core.Sales;

namespace Merchello.Core.Chains.InvoiceCreation
{
    public abstract class OrderPreparationAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly SalePreparationBase _salePreparation;

        protected OrderPreparationAttemptChainTaskBase(SalePreparationBase salePreparation)
        {
            Mandate.ParameterNotNull(salePreparation, "checkout");

            _salePreparation = salePreparation;
        }

        /// <summary>
        /// Gets the <see cref="SalePreparationBase"/> object
        /// </summary>
        protected SalePreparationBase SalePreparation
        {
            get { return _salePreparation; }
        }
    }
}