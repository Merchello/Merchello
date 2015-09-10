namespace Merchello.Bazaar
{
    using System;
    using System.Linq;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// Extension methods for <see cref="ProductDisplay"/>.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// The theme partial view path.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ThemePartialViewPath(this IMasterModel model, string viewName)
        {
            return PathHelper.GetThemePartialViewPath(model, viewName);
        }

        /// <summary>
        /// Gets the theme view path.
        /// </summary>
        /// <param name="model">
        /// The <see cref="IMasterModel"/>.
        /// </param>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation of the view path and name.
        /// </returns>
        public static string ThemeViewPath(this IMasterModel model, string viewName)
        {
            const string Path = "{0}Views/{1}.cshtml";
            return string.Format(Path, PathHelper.GetThemePath(model.Theme), viewName);
        }

        /// <summary>
        /// The theme account path.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="viewName">
        /// The view name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ThemeAccountPath(this IMasterModel model, string viewName)
        {
            const string Path = "{0}Views/Account/{1}.cshtml";
            return string.Format(Path, PathHelper.GetThemePath(model.Theme), viewName);
        }

        /// <summary>
        /// Formats the price with the Merchello's setting currency symbol.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormattedPrice(this ProductModel model)
        {
            if (!model.ProductData.ProductVariants.Any())
            {
                return FormatPrice(model.ProductData.Price, model.Currency);
            }

            return FormattedPrice(model.ProductData, model.Currency);
        }


        /// <summary>
        /// The formatted price.
        /// </summary>
        /// <param name="display">
        /// The display.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormattedPrice(this ProductDisplay display, ICurrency currency)
        {
            if (!display.ProductVariants.Any()) return FormatPrice(display.Price, currency);

            var variants = display.ProductVariants.ToArray();
            var onsaleLow = variants.Any(x => x.OnSale) ? variants.Where(x => x.OnSale).Min(x => x.SalePrice) : 0;
            var low = variants.Any(x => !x.OnSale) ? variants.Where(x => !x.OnSale).Min(x => x.Price) : 0;
            var onSaleHigh = variants.Any(x => x.OnSale) ? variants.Where(x => x.OnSale).Max(x => x.SalePrice) : 0;
            var max = variants.Any(x => !x.OnSale) ? variants.Where(x => !x.OnSale).Max(x => x.Price) : 0;

            if (variants.Any(x => x.OnSale))
            {
                low = onsaleLow < low ? onsaleLow : low;
                max = max > onSaleHigh ? max : onSaleHigh;
            }

            if (low != max)
                return string.Format(
                    "{0} - {1}",
                    FormatPrice(low, currency),
                    FormatPrice(max, currency));

            return FormatPrice(display.Price, currency);

        }

        /// <summary>
        /// Formats the sale price with the Merchello's setting currency symbol.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormattedSalePrice(this ProductModel model)
        {
            return FormatPrice(model.ProductData.SalePrice, model.Currency);
        }

        /// <summary>
        /// The format unit price.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatUnitPrice(this BasketLineItem lineItem, ICurrency currency)
        {
            return FormatPrice(lineItem.UnitPrice, currency);
        }

        /// <summary>
        /// The format total price.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatTotalPrice(this BasketLineItem lineItem, ICurrency currency)
        {
            return FormatPrice(lineItem.TotalPrice, currency);
        }

        /// <summary>
        /// The format total price.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatTotalPrice(this BasketTableModel model)
        {
            return FormatPrice(model.TotalPrice, model.Currency);
        }

        /// <summary>
        /// Formats a price with the Merchello's setting currency symbol.
        /// </summary>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatPrice(decimal price, ICurrency currency)
        {
            var storeSettingService = MerchelloContext.Current.Services.StoreSettingService;

            // Try to get a currency format else use the pre defined one.
            var symbol = currency.Symbol;
            var format = storeSettingService.GetCurrencyFormat(currency);
           

            return string.Format(format.Format, symbol, price);
        }
    }
}