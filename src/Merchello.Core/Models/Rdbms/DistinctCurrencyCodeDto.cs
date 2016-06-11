namespace Merchello.Core.Models.Rdbms
{
    using Umbraco.Core.Persistence;

    /// <summary>
    /// A dto used for querying distinct currency codes.
    /// </summary>
    internal sealed class DistinctCurrencyCodeDto
    {
        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [Column("currencyCode")]
        public string CurrencyCode { get; set; }
    }
}