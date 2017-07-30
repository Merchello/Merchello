namespace Merchello.Core.Models
{
    using System;
    


    /// <summary>
    /// Represents currency.
    /// </summary>
    public interface ICurrency
    {
        /// <summary>
        /// Gets the ISO Currency Code
        /// </summary>
        
        string CurrencyCode { get; }

        /// <summary>
        /// Gets the Currency Symbol
        /// </summary>
        
        string Symbol { get; }

        /// <summary>
        /// Gets the Currency Name
        /// </summary>
        
        string Name { get; }
    }
}