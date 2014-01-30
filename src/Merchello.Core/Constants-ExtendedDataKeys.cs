using System.Collections.Concurrent;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core
{
    public partial class Constants
    {
        /// <summary>
        /// The collection of Merchello "Reserved" ExtendedDataCollection keys
        /// </summary>
        public static IEnumerable<string> ReservedExtendedDataKeys
        {
            get { return new[]
                {
                    "merchExtendedData", 
                    "merchProductKey",
                    "merchProductVariantKey",
                    "merchCostOfGoods",
                    "merchWeight",
                    "merchHeight",
                    "merchWidth",
                    "merchLength",
                    "merchBarcode",
                    "merchPrice",
                    "merchOnSale",
                    "merchSalePrice",
                    "merchTrackInventory",
                    "merchOutOfStockPurchase",
                    "merchTaxable",
                    "merchShippalbe",
                    "merchDownload",
                    "merchDownloadMediaId",
                    "merchWarhouseCatalogKey"
                }; }
        }

        public static class ExtendedDataKeys
        {
            // ExtendedDataCollection
            public static string ExtendedData = "merchExtendedData";

            // ProductVariant
            public static string ProductKey = "merchProductKey";
            public static string ProductVariantKey = "merchProductVariantKey";
            public static string CostOfGoods = "merchCostOfGoods";
            public static string Weight = "merchWeight";
            public static string Height = "merchHeight";
            public static string Width = "MerchWidth";
            public static string Length = "merchLength";
            public static string Barcode = "merchBarcode";
            public static string Price = "merchPrice";
            public static string OnSale = "merchOnSale";
            public static string SalePrice = "merchSalePrice";
            public static string TrackInventory = "merchTrackInventory";
            public static string OutOfStockPurchase = "merchOutOfStockPurchase";
            public static string Taxable = "merchTaxable";
            public static string Shippable = "merchShippable";
            public static string Download = "merchDownload";
            public static string DownloadMediaId = "merchDownloadMediaId";

            // Shipment
            public static string WarehouseCatalogKey = "merchWarehouseCatalogKey";
        }
    }
}