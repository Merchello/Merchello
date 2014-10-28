namespace Merchello.Plugin.Taxation.Taxjar.Services
{
    using Merchello.Plugin.Taxation.Taxjar.Models;

    public interface ITaxJarTaxService
    {
        /// <summary>
        /// Gets the <see cref="TaxResult"/> from the TaxJar API based on request parameters.
        /// </summary>
        /// <param name="request">
        /// The <see cref="TaxRequest"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TaxResult"/>.
        /// </returns>
        TaxResult GetTax(TaxRequest request);
    }
}
