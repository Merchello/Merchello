namespace Merchello.Plugin.Taxation.Taxjar.Provider
{
    using Core.Models;

    /// <summary>
    /// Defines an TaxJar TaxationGatewayProvider.
    /// </summary>
    public interface ITaxJarTaxationGatewayProvider
    {
        /// <summary>
        /// Returns an <see cref="ITaxJarTaxationGatewayMethod"/> given the <see cref="ITaxMethod"/> (settings)
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxJarTaxationGatewayMethod"/>.
        /// </returns>
        ITaxJarTaxationGatewayMethod GetTaxJarTaxationGatewayMethod(ITaxMethod taxMethod);
    }
}