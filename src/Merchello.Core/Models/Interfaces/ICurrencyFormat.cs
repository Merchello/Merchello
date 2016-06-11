namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Defines a standard currency format
    /// </summary>
    public interface ICurrencyFormat
    {
        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        string Format { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        string Symbol { get; set; }
    }
}