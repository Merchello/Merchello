namespace Merchello.Core.Gateways.Taxation
{
    using Models;

    /// <summary>
    /// Defines the Taxation context
    /// </summary>
    public interface ITaxationContext : IGatewayProviderTypedContextBase<TaxationGatewayProviderBase>
    {
        /// <summary>
        /// Calculates taxes for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">The <see cref="IInvoice"/> to tax</param>
        /// <param name="quoteOnly">A value indicating whether or not the taxes should be calculated as a quote</param>
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        /// <remarks>
        /// 
        /// This assumes that the tax rate is assoicated with the invoice's billing address
        /// 
        /// </remarks>
        ITaxCalculationResult CalculateTaxesForInvoice(IInvoice invoice, bool quoteOnly = false);

        /// <summary>
        /// Calculates taxes for the <see cref="IInvoice"/>
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/> to tax
        /// </param>
        /// <param name="taxAddress">
        /// The address to base the taxation calculation - generally country and region
        /// </param>
        /// <param name="quoteOnly">
        /// A value indicating whether or not the taxes should be calculated as a quote
        /// </param>
        /// <returns>
        /// The <see cref="ITaxCalculationResult"/>
        /// </returns>
        ITaxCalculationResult CalculateTaxesForInvoice(IInvoice invoice, IAddress taxAddress, bool quoteOnly = false);

        /// <summary>
        /// Gets the tax method for a given tax address
        /// </summary>
        /// <param name="taxAddress">
        /// The tax address
        /// </param>
        /// <returns>
        /// The <see cref="ITaxMethod"/>.
        /// </returns>
        ITaxMethod GetTaxMethodForTaxAddress(IAddress taxAddress);

        /// <summary>
        /// Gets the tax method for country code.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxMethod"/>.
        /// </returns>
        ITaxMethod GetTaxMethodForCountryCode(string countryCode);
    }
}