namespace Merchello.Core.Models
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines currency.
    /// </summary>
    public interface ICurrency
    {
        /// <summary>
        /// Gets the ISO Currency Code
        /// </summary>
        [DataMember]
        string CurrencyCode { get; }

        /// <summary>
        /// Gets the Currency Symbol
        /// </summary>
        [DataMember]
        string Symbol { get; }

        /// <summary>
        /// Gets the Currency Name
        /// </summary>
        [DataMember]
        string Name { get; }
    }
}