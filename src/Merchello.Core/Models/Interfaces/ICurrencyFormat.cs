namespace Merchello.Core.Models.Interfaces
{
    /// <summary>
    /// Defines a standard currency format
    /// </summary>
    public interface ICurrencyFormat
    {
        /// <summary>
        /// The currency format
        /// </summary>
        string Format { get; set; }

        /// <summary>
        /// The currency format
        /// </summary>
        string Symbol { get; set; }
    }
}