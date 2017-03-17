﻿namespace Merchello.Core
{
    using System;

    /// <summary>
    /// The price extensions.
    /// </summary>
    internal static class FormattingExtensions
    {
        /// <summary>
        /// The format as store date.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The store formatted date string.
        /// </returns>
        internal static string FormatAsStoreDate(this DateTime value)
        {
            return value.FormatAsStoreDate(MerchelloContext.Current);
        }

        /// <summary>
        /// The format as store date.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <returns>
        /// The store formatted date string.
        /// </returns>
        internal static string FormatAsStoreDate(this DateTime value, IMerchelloContext merchelloContext)
        {
            var dateFormat =
                merchelloContext.Services.StoreSettingService.GetByKey(Constants.StoreSetting.DateFormatKey);
            return dateFormat == null ? value.ToShortDateString() : value.ToString(dateFormat.Value);
        }

        /// <summary>
        /// Formats a price with a currency symbol.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="currencySymbol">
        /// The currency symbol.
        /// </param>
        /// <returns>
        /// The formatted price.
        /// </returns>
        internal static string FormatAsPrice(this decimal value, string currencySymbol)
        {
            return string.Format("{0}{1:0.00}", currencySymbol, value);
        }
    }
}