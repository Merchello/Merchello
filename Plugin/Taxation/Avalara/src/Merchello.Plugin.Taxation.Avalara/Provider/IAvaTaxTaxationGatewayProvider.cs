namespace Merchello.Plugin.Taxation.Avalara.Provider
{
    using Core.Models;

    /// <summary>
    /// Defines an AvaTax TaxationGatewayProvider.
    /// </summary>
    public interface IAvaTaxTaxationGatewayProvider
    {
        /// <summary>
        /// Returns an <see cref="IAvaTaxTaxationGatewayMethod"/> given the <see cref="ITaxMethod"/> (settings)
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        /// <returns>
        /// The <see cref="IAvaTaxTaxationGatewayMethod"/>.
        /// </returns>
        IAvaTaxTaxationGatewayMethod GetAvaTaxationGatewayMethod(ITaxMethod taxMethod);
    }
}