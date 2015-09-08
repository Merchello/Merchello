using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    public class CurrencyFormat : ICurrencyFormat
    {
        public CurrencyFormat(string format, string symbol)
        {
            Format = format;
            Symbol = symbol;
        }

        public string Format { get; set; }
        public string Symbol { get; set; }
    }
}
