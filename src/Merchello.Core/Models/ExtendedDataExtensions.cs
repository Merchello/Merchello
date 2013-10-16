using System;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="ExtendedDataCollection"/>
    /// </summary>
    public static class ExtendedDataCollectionExtensions
    {

        public static void AddProductVariantValues(this ExtendedDataCollection extendedData, IProductVariant productVariant)
        {
            extendedData.SetValue("MerchProductKey", productVariant.ProductKey.ToString());
            extendedData.SetValue("MerchProductVariantKey", productVariant.Key.ToString());
            extendedData.SetValue("MerchCostOfGoods", productVariant.CostOfGoods.ToString());
            extendedData.SetValue("MerchWeight", productVariant.Weight.ToString());
            extendedData.SetValue("MerchWidth", productVariant.Width.ToString());
            extendedData.SetValue("MerchHeight", productVariant.Height.ToString());
            extendedData.SetValue("MerchLength", productVariant.Length.ToString());
            extendedData.SetValue("MerchBarcode", productVariant.Barcode);
            extendedData.SetValue("MerchTrackInventory", productVariant.TrackInventory.ToString());
            extendedData.SetValue("MerchOutOfStockPurchase", productVariant.OutOfStockPurchase.ToString());
            extendedData.SetValue("MerchTaxable", productVariant.Taxable.ToString());
            extendedData.SetValue("MerchShippable", productVariant.Shippable.ToString());
            extendedData.SetValue("MerchDownload", productVariant.Download.ToString());
            extendedData.SetValue("MerchDownloadMediaId", productVariant.DownloadMediaId.ToString());
        }

        /// <summary>
        /// True/false indicating whether or not the colleciton contains a ProductVariantKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static bool ContainsProductVariantKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey("MerchProductVariantKey");
        }

        /// <summary>
        /// Return the ProductVariantKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static Guid GetProductVariantKey(this ExtendedDataCollection extendedData)
        {
            return GetGuidValue(extendedData.GetValue("MerchProductVariantKey"));
        }

        /// <summary>
        /// True/false indicating whether or not the colleciton contains a ProductKey
        /// </summary>
        public static bool ContainsProductKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey("MerchProductKey");
        }

        /// <summary>
        /// Returns the ProductKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static Guid GetProductKey(this ExtendedDataCollection extendedData)
        {
            return GetGuidValue(extendedData.GetValue("MerchProductKey"));
        }

        /// <summary>
        /// Returns the "MerchTaxable" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetTaxableValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("MerchTaxable"));
        }

        /// <summary>
        /// Returns the "MerchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetTrackInventoryValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("MerchTrackInventory"));
        }

        /// <summary>
        /// Returns the "MerchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetOutOfStockPurchaseValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("MerchOutOfStockPurchase"));
        }

        /// <summary>
        /// Returns the "MerchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetShippableValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("MerchShippable"));
        }

        /// <summary>
        /// Returns the "MerchDownload" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetDownloadValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("MerchDownload"));
        }


        /// <summary>
        /// Returns the "MerchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static int GetDownloadMediaIdValue(this ExtendedDataCollection extendedData)
        {
            return GetIntegerValue(extendedData.GetValue("MerchDownloadMediaId"));
        }

        /// <summary>
        /// returns teh "MerchWeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetWeightValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue("MerchWeight"));
        }

        /// <summary>
        /// returns teh "MerchHeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetHeightValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue("MerchHeight"));
        }

        /// <summary>
        /// returns teh "MerchWidth" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetWidthValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue("MerchWidth"));
        }

        /// <summary>
        /// returns teh "MerchLength" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetLengthValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue("MerchLength"));
        }

        /// <summary>
        /// returns teh "MerchWeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static string GetBarcodeValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue("MerchBarcode");
        }


        private static Guid GetGuidValue(string value)
        {
            Guid converted;
            return Guid.TryParse(value, out converted) ? converted : Guid.Empty;
        }

        private static bool GetBooleanValue(string value)
        {
            bool converted;
            return bool.TryParse(value, out converted) ? converted : false;
        }

        private static decimal GetDecimalValue(string value)
        {
            decimal converted;
            return decimal.TryParse(value, out converted) ? converted : 0;
        }

        private static int GetIntegerValue(string value)
        {
            int converted;
            return int.TryParse(value, out converted) ? converted : 0;
        }
    }
}