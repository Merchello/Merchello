namespace Merchello.Web.Models.Reports
{
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Stores a currency value.
    /// </summary>
    public class ResultCurrencyValue
    {
        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        public CurrencyDisplay Currency { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public decimal Value { get; set; } 
    }
}