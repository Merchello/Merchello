namespace Merchello.Core.Chains.InvoiceCreation.SalesPreparation
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    /// <summary>
    /// The invoice creation attempt chain task base.
    /// </summary>
    [Obsolete("Superseded by CheckoutManger.InvoiceCreationAttemptChainTaskBase")]
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
            Ensure.ParameterNotNull(salePreparation, "salePreparation");

            this._salePreparation = salePreparation;
        }

        /// <summary>
        /// Gets the <see cref="SalePreparationBase"/> object
        /// </summary>
        protected SalePreparationBase SalePreparation
        {
            get { return this._salePreparation; }
        }
    }
}