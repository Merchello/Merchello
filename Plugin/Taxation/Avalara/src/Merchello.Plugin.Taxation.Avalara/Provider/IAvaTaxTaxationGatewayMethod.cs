namespace Merchello.Plugin.Taxation.Avalara.Provider
{
    using Core.Gateways.Taxation;
    using Core.Models;

    /// <summary>
    /// Defines an AvaTax TaxationGatewayMethod.
    /// </summary>
    public interface IAvaTaxTaxationGatewayMethod : ITaxationGatewayMethod
    {
        /// <summary>
        /// The calculate tax for invoice.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <param name="quoteOnly">
        /// A value indicating whether or not this is a tax quote or a formal tax submission
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>.
        /// </returns>
        ITaxCalculationResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress, bool quoteOnly);
    }
}