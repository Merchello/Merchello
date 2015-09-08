namespace Merchello.Core.Models
{
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The currency format.
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
        /// Gets or sets the format.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The create default.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
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
