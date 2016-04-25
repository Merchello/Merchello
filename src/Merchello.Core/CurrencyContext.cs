namespace Merchello.Core
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;

    using Umbraco.Core;

    /// <summary>
    /// The currency context.
    /// </summary>
    public class CurrencyContext
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static CurrencyContext _instance;

        /// <summary>
        /// The <see cref="IStoreSettingService"/>.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// The store currency.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private ICurrency _storeCurrency;

        /// <summary>
        /// The currency format.
        /// </summary>
        // ReSharper disable InconsistentNaming
        private ICurrencyFormat _currencyFormat;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyContext"/> class.
        /// </summary>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        internal CurrencyContext(IStoreSettingService storeSettingService)
        {
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");

            this._storeSettingService = storeSettingService;
        }

        /// <summary>
        /// Gets a value indicating whether the context singleton is setup.
        /// </summary>
        public static bool HasCurrent
        {
            get
            {
                return Current != null;
            }
        }

        /// <summary>
        /// Gets or sets the singleton.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public static CurrencyContext Current
        {
            get
            {
                return _instance;
            }

            internal set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Gets the store currency.
        /// </summary>
        /// <remarks>
        /// This assumes that all stores will use the same currency
        /// </remarks>
        public ICurrency StoreCurrency
        {
            get
            {
                if (this._storeCurrency != null) return this._storeCurrency;
                var storeSetting = this._storeSettingService.GetByKey(Core.Constants.StoreSettingKeys.CurrencyCodeKey);
                this._storeCurrency = this._storeSettingService.GetCurrencyByCode(storeSetting.Value);
                return this._storeCurrency;
            }
        }

        /// <summary>
        /// Gets the store currency format.
        /// </summary>
        private ICurrencyFormat StoreCurrencyFormat
        {
            get
            {
                if (this._currencyFormat != null) return this._currencyFormat;
                this._currencyFormat = this._storeSettingService.GetCurrencyFormat(this.StoreCurrency);
                return this._currencyFormat;
            }
        }

        /// <summary>
        /// Formats the currency based with symbol and configured culture.
        /// </summary>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <returns>
        /// The formatted amount.
        /// </returns>
        /// <remarks>
        /// Overrides for currency format are made in the Merchello.config
        /// </remarks>
        public string FormatCurrency(decimal amount)
        {
            return string.Format(this.StoreCurrencyFormat.Format, this._storeCurrency.Symbol, amount);
        }

        /// <summary>
        /// Clears the static field values.
        /// </summary>
        internal void ResetCurrency()
        {
            this._storeCurrency = null;
            this._currencyFormat = null;
        }
    }
}