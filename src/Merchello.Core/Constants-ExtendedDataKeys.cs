using System.Collections.Generic;

namespace Merchello.Core
{
    public partial class Constants
    {
        /// <summary>
        /// The collection of Merchello "Reserved" ExtendedDataCollection keys
        /// </summary>
        public static IEnumerable<string> ReservedExtendedDataKeys
        {
            get { 
                return new[]
                {
                    "merchExtendedData", 
                    "merchLineItemCollection",
                    "merchLineItem",
                    "merchCurrencyCode",
                    "merchLineItemTaxAmount",
                    "merchName",
                    "merchSku",
                    "merchExported",
                    "merchPaymentMethod",
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
                    "merchShippable",
                    "merchDownload",
                    "merchDownloadMediaId",
                    "merchWarhouseCatalogKey",
                    "merchContainerKey",
                    "merchLineItemTfKey",
                    "merchQuantity",
                    "merchAmount",
                    "merchShipmentKey",
                    "merchShipMethodKey",
                    "merchWarehouseCatalogKey",
                    "merchShippingOriginAddress",
                    "merchShippingDestinationAddress",
                    "merchBillingAddress",
                    "merchBaseTax",
                    "merchProvinceTaxRate",
                    "merchSmtpProviderSettings"
                }; 
            }
        }

        public static class ExtendedDataKeys
        {
            // Common - serialized classes
            public static string ExtendedData = "merchExtendedData";
            public static string LineItemCollection = "merchLineItemCollection";
            public static string LineItem = "merchLineItem";
            public static string ProductVariant = "merchProductVariant";
            public static string CurrencyCode = "merchCurrencyCode";

            // Common            
            public static string Name = "merchName";
            public static string Sku = "merchSku";
            public static string Exported = "merchExported";

            // Payment
            public static string PaymentMethod = "merchPaymentMethod";

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
            public static string ShipmentKey = "merchShipmentKey";
            public static string ShipMethodKey = "merchShipMethodKey";
            public static string WarehouseCatalogKey = "merchWarehouseCatalogKey";
            public static string ShippingOriginAddress = "merchShippingOriginAddress";
            public static string ShippingDestinationAddress = "merchShippingDestinationAddress";
            public static string BillingAddress = "merchBillingAddress";

            // LineItem
            public static string ContainerKey = "merchContainerKey";
            public static string LineItemTfKey = "merchLineItemTfKey";
            public static string Quantity = "merchQuantity";
            public static string BaseTaxRate = "merchBaseTaxRate";
            public static string ProviceTaxRate = "merchProvinceTaxRate";
            public static string LineItemTaxAmount = "merchLineItemTaxAmount";

            // SMTP
            public static string SmtpProviderSettings = "merchSmtpProviderSettings";
            
        }
    }
}