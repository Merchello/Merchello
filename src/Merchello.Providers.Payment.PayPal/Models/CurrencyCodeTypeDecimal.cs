namespace Merchello.Providers.Payment.PayPal.Models
{
    using global::PayPal.PayPalAPIInterfaceService.Model;

    /// <summary>
    /// A model for exposing the decimal places applied to a currency in a event.
    /// </summary>
    public class CurrencyCodeTypeDecimal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyCodeTypeDecimal"/> class.
        /// </summary>
        public CurrencyCodeTypeDecimal()
        {
            this.DecimalPlaces = 2;
        }

        /// <summary>
        /// Gets or sets the currency code type.
        /// </summary>
        public CurrencyCodeType CurrencyCodeType { get; set; }

        /// <summary>
        /// Gets or sets the decimal places.
        /// </summary>
        public int DecimalPlaces { get; set; } 
    }
}