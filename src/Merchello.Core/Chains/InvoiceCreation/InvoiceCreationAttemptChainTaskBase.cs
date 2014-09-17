namespace Merchello.Core.Chains.InvoiceCreation
{
    using Models;
    using Sales;

    /// <summary>
    /// The invoice creation attempt chain task base.
    /// </summary>
    public abstract class InvoiceCreationAttemptChainTaskBase : AttemptChainTaskBase<IInvoice>
    {
        /// <summary>
        /// The sale preparation.
        /// </summary>
        private readonly SalePreparationBase _salePreparation;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceCreationAttemptChainTaskBase"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        protected InvoiceCreationAttemptChainTaskBase(SalePreparationBase salePreparation)
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