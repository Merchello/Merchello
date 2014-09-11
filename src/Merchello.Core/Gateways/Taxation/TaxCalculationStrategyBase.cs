namespace Merchello.Core.Gateways.Taxation
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines an invoice taxation strategy base class
    /// </summary>
    public abstract class TaxCalculationStrategyBase : ITaxCalculationStrategy
    {
        /// <summary>
        /// The invoice.
        /// </summary>
        private readonly IInvoice _invoice;

        /// <summary>
        /// The tax address.
        /// </summary>
        private readonly IAddress _taxAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxCalculationStrategyBase"/> class.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        protected TaxCalculationStrategyBase(IInvoice invoice, IAddress taxAddress)
        {
            Mandate.ParameterNotNull(invoice, "invoice");
            Mandate.ParameterNotNull(taxAddress, "taxAddress");

            _invoice = invoice;
            _taxAddress = taxAddress;
        }

        /// <summary>
        /// Gets the <see cref="IInvoice"/>
        /// </summary>
        protected IInvoice Invoice
        {
            get { return _invoice; }
        }

        /// <summary>
        /// Gets the tax address
        /// </summary>
        protected IAddress TaxAddress
        {
            get { return _taxAddress; }
        }

        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>
        /// </returns>
        public abstract Attempt<ITaxCalculationResult> CalculateTaxesForInvoice();
    }
}