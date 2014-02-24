using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines the abstract GatewayTaxMethod
    /// </summary>
    public interface IGatewayTaxMethod
    {
        /// <summary>
        /// Gets the <see cref="Models.IGatewayTaxMethod"/>
        /// </summary>
        Models.IGatewayTaxMethod GatewayTaxMethod { get; }

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
        IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice);

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <param name="taxAddress">The <see cref="IAddress"/> to base taxation rates.  Either origin or destination address.</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress);

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="strategy">The strategy to use when calculating the tax amount</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        IInvoiceTaxResult CalculateTaxForInvoice(IInvoiceTaxationStrategy strategy);
    }
}