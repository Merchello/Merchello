namespace Merchello.Core.Models
{
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a currency format.
    /// </summary>
    public class CurrencyFormat : ICurrencyFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyFormat"/> class.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        public CurrencyFormat(string format, string symbol)
        {
            Format = format;
            Symbol = symbol;
        }

        /// <summary>
        /// Gets or sets the format that is used when formatting a decimal to a string representation.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the currency symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Helper that returns the default format.
        /// </summary>
        /// <param name="symbol">
        /// The currency symbol.
        /// </param>
        /// <returns>
        /// The <see cref="CurrencyFormat"/>.
        /// </returns>
        public static ICurrencyFormat CreateDefault(string symbol)
        {
            return new CurrencyFormat("{0}{1:0.00}", symbol);
        }
    }
}
