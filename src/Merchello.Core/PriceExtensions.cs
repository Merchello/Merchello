namespace Merchello.Core
{
    /// <summary>
    /// The price extensions.
    /// </summary>
    internal static class PriceExtensions
    {
        /// <summary>
        /// Formats a price with a currency symbol.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="currencySymbol">
        /// The currency symbol.
        /// </param>
        /// <returns>
        /// The formatted price.
        /// </returns>
        public static string FormatAsPrice(this decimal value, string currencySymbol)
        {
            return string.Format("{0}{1:0.00}", currencySymbol, value);
        }
    }
}