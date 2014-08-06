namespace Merchello.Core.Gateways.Taxation
{
    using Merchello.Core.Models;

    /// <summary>
    /// Defines an invoice tax calculation result
    /// </summary>
    public interface ITaxCalculationResult
    {
        /// <summary>
        /// Gets the name of the Tax Method
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the tax used in the tax calculation
        /// </summary>
        decimal TaxRate { get; }

        /// <summary>
        /// Gets or sets the calculated tax amount
        /// </summary>
        decimal TaxAmount { get; set; }

        /// <summary>
        /// Gets the extended data collection to store any additional information returned by the Tax Provider.
        /// Ex. may include an itemization of Country, State, Local / Municipal taxes
        /// </summary>
        ExtendedDataCollection ExtendedData { get; }
    }
}