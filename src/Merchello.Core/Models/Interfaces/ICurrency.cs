using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    public interface ICurrency
    {
        /// <summary>
        /// The ISO Currency Code
        /// </summary>
        [DataMember]
        string CurrencyCode { get; }

        /// <summary>
        /// The Currency Symbol
        /// </summary>
        [DataMember]
        string Symbol { get; }

        /// <summary>
        /// The Currency Name
        /// </summary>
        [DataMember]
        string Name { get; }
    }
}