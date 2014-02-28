using Merchello.Core.Models;
using Merchello.Core.Sales;

namespace Merchello.Core.Chains.InvoiceCreation
{
    public abstract class SalePreparationAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        private readonly SalePreparationBase _salePreparation;

        protected SalePreparationAttemptChainTaskBase(SalePreparationBase salePreparation)
        {
            Mandate.ParameterNotNull(salePreparation, "salePreparation");

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