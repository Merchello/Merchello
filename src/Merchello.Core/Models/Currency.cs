using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    internal class Currency : ICurrency
    {

        public Currency(string currencyCode, string symbol, string name)
        {
            Mandate.ParameterNotNullOrEmpty(currencyCode, "currencyCode");
            Mandate.ParameterNotNullOrEmpty(symbol, "symbol");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            CurrencyCode = currencyCode;
            Symbol = symbol;
            Name = name;
        }

        /// <summary>
        /// The ISO Currency Code
        /// </summary>
        [DataMember]
        public string CurrencyCode { get; private set; }

        /// <summary>
        /// The Currency Symbol
        /// </summary>
        [DataMember]
        public string Symbol { get; private set; }

        /// <summary>
        /// The Currency Name
        /// </summary>
        [DataMember]
        public string Name { get; private set; }
    }
}