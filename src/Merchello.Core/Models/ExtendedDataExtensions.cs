using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="ExtendedDataCollection"/>
    /// </summary>
    public static class ExtendedDataCollectionExtensions
    {

        #region Product / ProductVariant
        

        public static void AddProductVariantValues(this ExtendedDataCollection extendedData, IProductVariant productVariant)
        {
            extendedData.SetValue("merchProductKey", productVariant.ProductKey.ToString());
            extendedData.SetValue("merchProductVariantKey", productVariant.Key.ToString());
            extendedData.SetValue("merchCostOfGoods", productVariant.CostOfGoods.ToString());
            extendedData.SetValue("merchWeight", productVariant.Weight.ToString());
            extendedData.SetValue("merchWidth", productVariant.Width.ToString());
            extendedData.SetValue("merchHeight", productVariant.Height.ToString());
            extendedData.SetValue("merchLength", productVariant.Length.ToString());
            extendedData.SetValue("merchBarcode", productVariant.Barcode);
            extendedData.SetValue("merchTrackInventory", productVariant.TrackInventory.ToString());
            extendedData.SetValue("merchOutOfStockPurchase", productVariant.OutOfStockPurchase.ToString());
            extendedData.SetValue("merchTaxable", productVariant.Taxable.ToString());
            extendedData.SetValue("merchShippable", productVariant.Shippable.ToString());
            extendedData.SetValue("merchDownload", productVariant.Download.ToString());
            extendedData.SetValue("merchDownloadMediaId", productVariant.DownloadMediaId.ToString());
        }

        /// <summary>
        /// True/falce indicating whether or not this extended data collection contains information 
        /// which could define a <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static bool DefinesProductVariant(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsProductVariantKey() && extendedData.ContainsProductKey();
        }

        /// <summary>
        /// True/false indicating whether or not the colleciton contains a ProductVariantKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static bool ContainsProductVariantKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey("merchProductVariantKey");
        }       

        /// <summary>
        /// Return the ProductVariantKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static Guid GetProductVariantKey(this ExtendedDataCollection extendedData)
        {
            return GetGuidValue(extendedData.GetValue("merchProductVariantKey"));
        }

        /// <summary>
        /// True/false indicating whether or not the colleciton contains a ProductKey
        /// </summary>
        public static bool ContainsProductKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey("merchProductKey");
        }

        /// <summary>
        /// Returns the ProductKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static Guid GetProductKey(this ExtendedDataCollection extendedData)
        {
            return GetGuidValue(extendedData.GetValue("merchProductKey"));
        }

        /// <summary>
        /// Returns the "MerchTaxable" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetTaxableValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("merchTaxable"));
        }

        /// <summary>
        /// Returns the "MerchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetTrackInventoryValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("merchTrackInventory"));
        }

        /// <summary>
        /// Returns the "MerchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetOutOfStockPurchaseValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("merchOutOfStockPurchase"));
        }

        /// <summary>
        /// Returns the "MerchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetShippableValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("merchShippable"));
        }

        /// <summary>
        /// Returns the "MerchDownload" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetDownloadValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue("merchDownload"));
        }

        /// <summary>
        /// Returns the "MerchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static int GetDownloadMediaIdValue(this ExtendedDataCollection extendedData)
        {
            return GetIntegerValue(extendedData.GetValue("merchDownloadMediaId"));
        }

        /// <summary>
        /// returns the "MerchWeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetWeightValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue("MerchWeight"));
        }

        /// <summary>
        /// returns the "MerchHeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetHeightValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue("merchHeight"));
        }

        /// <summary>
        /// returns the "MerchWidth" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetWidthValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue("merchWidth"));
        }

        /// <summary>
        /// returns the "MerchLength" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetLengthValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue("merchLength"));
        }

        /// <summary>
        /// returns the "MerchWeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static string GetBarcodeValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue("merchBarcode");
        }

        #endregion

        #region IShipment

        /// <summary>
        /// True/false indicating whether or not the colleciton contains a WarehouseCatalogKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static bool ContainsWarehouseCatalogKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey("merchWarehouseCatalogKey");
        }

        /// <summary>
        /// Return the WarehouseCatalogKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static Guid GetWarehouseCatalogKey(this ExtendedDataCollection extendedData)
        {
            return GetGuidValue(extendedData.GetValue("merchWarehouseCatalogKey"));
        }

        #endregion


        #region Utility


        private static Guid GetGuidValue(string value)
        {
            Guid converted;
            return Guid.TryParse(value, out converted) ? converted : Guid.Empty;
        }

        private static bool GetBooleanValue(string value)
        {
            bool converted;
            return bool.TryParse(value, out converted) && converted;
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

        #endregion
    }
}