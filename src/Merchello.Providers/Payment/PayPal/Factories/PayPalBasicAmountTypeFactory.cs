namespace Merchello.Providers.Payment.PayPal.Factories
{    
    using Merchello.Core.Events;
    using Merchello.Providers.Payment.PayPal.Models;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// A factory for building BasicAmountType.
    /// </summary>
    public class PayPalBasicAmountTypeFactory
    {
        /// <summary>
        /// The <see cref="CurrencyCodeType"/>.
        /// </summary>
        private readonly CurrencyCodeType _currencyCodeType;

        /// <summary>
        /// The number of decimal places.
        /// </summary>
        private int _decimalPlaces = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalBasicAmountTypeFactory"/> class.
        /// </summary>
        /// <param name="currencyCodeType">
        /// The currency code type.
        /// </param>
        public PayPalBasicAmountTypeFactory(CurrencyCodeType currencyCodeType)
        {
            _currencyCodeType = currencyCodeType;

            Initialize();
        }

        /// <summary>
        /// Occurs before formatting prices.
        /// </summary>
        public static event TypedEventHandler<PayPalBasicAmountTypeFactory, ObjectEventArgs<CurrencyCodeTypeDecimal>> FormattingPrice;

        /// <summary>
        /// Builds <see cref="BasicAmountType"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="BasicAmountType"/>.
        /// </returns>
        public BasicAmountType Build(decimal value)
        {
            return new BasicAmountType(_currencyCodeType, PriceToString(value, _decimalPlaces));
        }

        /// <summary>
        /// Formats a price.
        /// </summary>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="decimals">
        /// The decimals.
        /// </param>
        /// <returns>
        /// Returns the price as a string value.
        /// </returns>
        private string PriceToString(decimal price, int decimals)
        {
            var priceFormat = decimals == 0 ? "0" : "0." + new string('0', decimals);
            return price.ToString(priceFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the number of decimal places for a particular currency.
        /// </summary>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetCurrencyDecimals(CurrencyCodeType currency)
        {
            var currencyDecimals = new CurrencyCodeTypeDecimal
            {
                CurrencyCodeType = currency,
                DecimalPlaces = 2
            };

            FormattingPrice.RaiseEvent(new ObjectEventArgs<CurrencyCodeTypeDecimal>(currencyDecimals), this);

            return currencyDecimals.DecimalPlaces;
        }

        /// <summary>
        /// Initializes the factory.
        /// </summary>
        private void Initialize()
        {
            _decimalPlaces = GetCurrencyDecimals(_currencyCodeType);
        }
    }
}