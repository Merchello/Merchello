using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines a taxation gateway provider
    /// </summary>
    public interface ITaxationGatewayProvider : IGateway
    {
        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice);

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <param name="strategy">The <see cref="IInvoiceTaxationStrategy"/> to use when in the tax computation</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IInvoiceTaxationStrategy strategy);
    }
}