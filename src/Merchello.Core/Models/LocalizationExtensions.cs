using System;
using System.Globalization;
using System.Linq;

namespace Merchello.Core.Models
{
    public static class LocalizationExtensions
    {
        ///// <summary>
        ///// Returns the most 'generic' <see cref="CultureInfo"/> based on a <see cref="RegionInfo"/>
        ///// </summary>
        //public static CultureInfo GetCultureInfo(this RegionInfo regionInfo)
        //{
            
        //    return CultureInfo.GetCultures(CultureTypes.AllCultures)
        //        .Where(ci => ci.Name.EndsWith(regionInfo.TwoLetterISORegionName))
        //        .FirstOrDefault(ci => !ci.EnglishName.Contains(" ("));
        //}

        ///// <summary>
        ///// Gets the most 'generic' <see cref="CultureInfo"/> based on the ISOCurrencySymbol (eg. USD) stored in merchCurrencyCode
        ///// </summary>
        ///// <param name="extendedData"></param>
        ///// <returns></returns>
        //public static CultureInfo GetCultureByCurrencyCode(this ExtendedDataCollection extendedData)
        //{
        //    return extendedData.GetCultureByCurrencyCode(MerchelloContext.Current);
        //}

        //internal static CultureInfo GetCultureByCurrencyCode(this ExtendedDataCollection extendedData, IMerchelloContext merchelloContext)
        //{
        //    return !extendedData.ContainsKey(Constants.ExtendedDataKeys.CurrencyCode) ?
        //        GetCultureInfoFromStoreCurrencySetting(merchelloContext) :
        //        GetCultureInfoFromIsoCurrencySymbol(extendedData.GetValue(Constants.ExtendedDataKeys.CurrencyCode));
        //}

        ///// <summary>
        ///// Gets the most 'generic' <see cref="CultureInfo"/> based on the ISOCurrencySymbol (eg. USD) saved in the StoreSettings - default currency
        ///// </summary>
        ///// <param name="isoCurrencySymbol">The ISOCurrencySymbol (eg. USD)</param>
        ///// <returns>The <see cref="CultureInfo"/> associated with the ISOCurrencySymbol</returns>
        //private static CultureInfo GetCultureInfoFromIsoCurrencySymbol(string isoCurrencySymbol)
        //{
        //    var regions = CultureInfo.GetCultures(CultureTypes.AllCultures)
        //        .Where(culture => new RegionInfo(culture.Name).ISOCurrencySymbol == isoCurrencySymbol);
        //    return regions
        //        .FirstOrDefault(ci => !ci.EnglishName.Contains(" ("));
        //}

        ///// <summary>
        ///// Gets the most 'generic' <see cref="CultureInfo"/> based on the ISOCurrencySymbol (eg. USD) saved in the StoreSettings - default currency
        ///// </summary>
        ///// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        ///// <returns>The <see cref="CultureInfo"/> associated with the ISOCurrencySymbol</returns>
        //private static CultureInfo GetCultureInfoFromStoreCurrencySetting(IMerchelloContext merchelloContext)
        //{
        //    var setting = merchelloContext.Services.StoreSettingService.GetByKey(Constants.StoreSettingKeys.CurrencyCodeKey);

        //    return GetCultureInfoFromIsoCurrencySymbol(setting.Value);
        //}


        //public static string FormatPrice(this ILineItem lineItem)
        //{
        //    return lineItem.FormatCurrency(MerchelloContext.Current);
        //}

        //public static string FormatPriceWithoutSymbol(this ILineItem lineItem)
        //{
        //    return lineItem.FormatCurrency(MerchelloContext.Current, false);
        //}

        //internal static string FormatCurrency(this ILineItem lineItem, IMerchelloContext merchelloContext, bool includeSymbol = true)
        //{
        //    var cultureInfo = lineItem.ExtendedData.GetCultureByCurrencyCode(merchelloContext);
        //    var nfi = cultureInfo.NumberFormat;
        //    if (!includeSymbol) nfi.CurrencySymbol = "";

        //    return FormatCurrency(lineItem.Price, nfi);
        //}

        ///// <summary>
        ///// Formats currency in a specific format
        ///// </summary>
        //private static string FormatCurrency(decimal amount, NumberFormatInfo numberFormatInfo)
        //{
        //    return string.Format(numberFormatInfo, "{0:c}", amount);
        //}
        
    }
}