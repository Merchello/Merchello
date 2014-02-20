using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Taxation
{
    /// <summary>
    /// Defines the Taxation context
    /// </summary>
    public interface ITaxationContext : IGatewayProviderTypedContextBase<TaxationGatewayProviderBase>
    {
        /// <summary>
        /// Calculates taxes for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to tax</param>
        /// <returns>The <see cref="IInvoiceTaxResult"/></returns>
        IInvoiceTaxResult CalculateTaxesForInvoice(IInvoice invoice);
    }
}