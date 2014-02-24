using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Represents the abstract GatewayTaxMethod
    /// </summary>
    public abstract class GatewayTaxMethodBase : IGatewayTaxMethod
    {
        private readonly Models.IGatewayTaxMethod _gatewayTaxMethod;

        protected GatewayTaxMethodBase(Models.IGatewayTaxMethod gatewayTaxMethod)
        {
            Mandate.ParameterNotNull(gatewayTaxMethod, "taxMethod");

            _gatewayTaxMethod = gatewayTaxMethod;
        }

        /// <summary>
        /// Gets the <see cref="Models.IGatewayTaxMethod"/>
        /// </summary>
        public Models.IGatewayTaxMethod GatewayTaxMethod
        {
            get { return _gatewayTaxMethod; }
        }

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        /// <remarks>
        /// 
        /// Assumes the billing address of the invoice will be used for the taxation address
        /// 
        /// </remarks>
        public virtual IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice)
        {
            return CalculateTaxForInvoice(invoice, invoice.GetBillingAddress());
        }

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <param name="taxAddress">The <see cref="IAddress"/> to base taxation rates.  Either origin or destination address.</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        public abstract IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress);

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="strategy">The strategy to use when calculating the tax amount</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        public virtual IInvoiceTaxResult CalculateTaxForInvoice(IInvoiceTaxationStrategy strategy)
        {
            var attempt = strategy.CalculateTaxesForInvoice();

            if (!attempt.Success) throw attempt.Exception;

            return attempt.Result;
        }
    }
}