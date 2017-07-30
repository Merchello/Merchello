namespace Merchello.Core.Models.Rdbms
{
    /// <summary>
    /// A dto used for querying distinct currency codes.
    /// </summary>
    internal sealed class DistinctCurrencyCodeDto
    {
        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        public string CurrencyCode { get; set; }
    }
}