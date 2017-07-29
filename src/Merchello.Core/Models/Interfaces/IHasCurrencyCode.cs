namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents an entity that has a currency code.
    /// </summary>
    public interface IHasCurrencyCode
    {
        /// <summary>
        /// Gets the currency code.
        /// </summary>
        string CurrencyCode { get; } 
    }
}