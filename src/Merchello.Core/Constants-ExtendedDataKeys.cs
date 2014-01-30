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
                    "merchName",
                    "merchSku",
                    "merchExported",
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
                    "merchManufacturer",
                    "merchManufacturerModelNumber",
                    "merchSalePrice",
                    "merchTrackInventory",
                    "merchOutOfStockPurchase",
                    "merchTaxable",
                    "merchShippalbe",
                    "merchDownload",
                    "merchDownloadMediaId",
                    "merchWarhouseCatalogKey",
                    "merchContainerKey",
                    "merchLineItemTfKey",
                    "merchQuantity",
                    "merchAmount"
                }; }
        }

        public static class ExtendedDataKeys
        {
            // Common
            public static string ExtendedData = "merchExtendedData";
            public static string Name = "merchName";
            public static string Sku = "merchSku";
            public static string Exported = "merchExported";

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
            public static string Manufacturer = "merchManufacturer";
            public static string ManufacturerModelNumber = "merchManufacturerModelNumber";
            public static string SalePrice = "merchSalePrice";
            public static string TrackInventory = "merchTrackInventory";
            public static string OutOfStockPurchase = "merchOutOfStockPurchase";
            public static string Taxable = "merchTaxable";
            public static string Shippable = "merchShippable";
            public static string Download = "merchDownload";
            public static string DownloadMediaId = "merchDownloadMediaId";

            // Shipment
            public static string WarehouseCatalogKey = "merchWarehouseCatalogKey";

            // LineItem
            public static string ContainerKey = "merchContainerKey";
            public static string LineItemTfKey = "merchLineItemTfKey";
            public static string Quantity = "merchQuantity";
            public static string Amount = "merchAmount";
        }
    }
}