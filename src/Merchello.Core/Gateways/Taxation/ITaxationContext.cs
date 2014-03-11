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
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        /// <remarks>
        /// 
        /// This assumes that the tax rate is assoicated with the invoice's billing address
        /// 
        /// </remarks>
        ITaxCalculationResult CalculateTaxesForInvoice(IInvoice invoice);

        /// <summary>
        /// Calculates taxes for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to tax</param>
        /// <param name="taxAddress">The address to base the taxation calculation - generally country and region</param>
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        ITaxCalculationResult CalculateTaxesForInvoice(IInvoice invoice, IAddress taxAddress);
    }
}