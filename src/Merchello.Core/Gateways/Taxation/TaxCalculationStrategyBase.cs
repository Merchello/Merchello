using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines an invoice taxation strategy base class
    /// </summary>
    public abstract class TaxCalculationStrategyBase : ITaxCalculationStrategy
    {
        private readonly IInvoice _invoice;
        private readonly IAddress _taxAddress;

        protected TaxCalculationStrategyBase(IInvoice invoice, IAddress taxAddress)
        {
            Mandate.ParameterNotNull(invoice, "invoice");
            Mandate.ParameterNotNull(taxAddress, "taxAddress");

            _invoice = invoice;
            _taxAddress = taxAddress;
        }

        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        public abstract Attempt<ITaxCalculationResult> CalculateTaxesForInvoice();


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
    }
}