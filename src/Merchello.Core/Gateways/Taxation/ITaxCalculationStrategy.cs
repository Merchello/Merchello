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
        /// <returns>The <see cref="ITaxCalculationResult"/></returns>
        Attempt<ITaxCalculationResult> CalculateTaxesForInvoice();
    }
}
