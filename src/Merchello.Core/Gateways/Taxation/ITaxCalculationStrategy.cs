namespace Merchello.Core.Gateways.Taxation
{
    using Merchello.Core.Strategies;

    using Umbraco.Core;

    /// <summary>
    /// Defines a taxation strategy
    /// </summary>
    public interface ITaxCalculationStrategy : IStrategy
    {
        /// <summary>
        /// Computes the invoice tax result
        /// </summary>
        /// <param name="estimateOnly">
        /// An optional parameter indicating that the tax calculation should be an estimate.
        /// This is useful for some 3rd party tax APIs
        /// </param>
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        Attempt<ITaxCalculationResult> CalculateTaxesForInvoice(bool estimateOnly);
    }
}
