using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines an invoice taxation strategy base class
    /// </summary>
    public abstract class InvoiceTaxationStrategyBase : IInvoiceTaxationStrategy
    {
        private readonly IInvoice _invoice;
        private readonly IAddress _taxAddress;

        protected InvoiceTaxationStrategyBase(IInvoice invoice, IAddress taxAddress)
        {
            Mandate.ParameterNotNull(invoice, "invoice");
            Mandate.ParameterNotNull(taxAddress, "taxAddress");

            _invoice = invoice;
            _taxAddress = taxAddress;
        }

        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <returns>The <see cref="IInvoiceTaxResult"/></returns>
        public abstract Attempt<IInvoiceTaxResult> GetInvoiceTaxResult();


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