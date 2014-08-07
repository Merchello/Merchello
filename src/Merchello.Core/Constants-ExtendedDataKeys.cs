namespace Merchello.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Merchello ExtendedData constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// Gets the collection of Merchello "Reserved" ExtendedDataCollection keys
        /// </summary>
        public static IEnumerable<string> ReservedExtendedDataKeys
        {
            get 
            { 
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
                    "merchTaxTransactionResults",
                    "merchSmtpProviderSettings"
                }; 
            }
        }

        /// <summary>
        /// The extended data keys.
        /// </summary>
        public static class ExtendedDataKeys
        {
            //// Common - serialized classes --------------------------------------------------------

            /// <summary>
            /// Gets the extended data.
            /// </summary>
            public static string ExtendedData
            {
                get { return "merchExtendedData"; }
            }

            /// <summary>
            /// Gets the line item collection.
            /// </summary>
            public static string LineItemCollection
            {
                get { return "merchLineItemCollection";  }    
            }

            /// <summary>
            /// Gets the line item.
            /// </summary>
            public static string LineItem
            {
                get { return "merchLineItem"; }
            }

            /// <summary>
            /// Gets the product variant.
            /// </summary>
            public static string ProductVariant
            {
                get { return "merchProductVariant"; }
            }

            /// <summary>
            /// Gets the currency code.
            /// </summary>
            public static string CurrencyCode
            {
                get { return "merchCurrencyCode";  } 
            }

            //// Common -------------------------------------------------------------------------

            /// <summary>
            /// Gets the name reserved extended data key.
            /// </summary>
            public static string Name
            {
                get { return "merchName"; }
            }

            /// <summary>
            /// Gets the price reserved extended data key.
            /// </summary>
            public static string Price
            {
                get { return "merchPrice"; }
            }

            /// <summary>
            /// Gets the sku reserved extended data key.
            /// </summary>
            public static string Sku
            {
                get { return "merchSku"; }
            }

            /// <summary>
            /// Gets the exported reserved extended data key.
            /// </summary>
            public static string Exported
            {
                get { return "merchExported"; }
            }

            /// <summary>
            /// Gets the quantity reserved extended data key.
            /// </summary>
            public static string Quantity
            {
                get { return "merchQuantity"; }
            }

            //// Payment ------------------------------------------------------------------------

            /// <summary>
            /// Gets the payment method reserved extended data key.
            /// </summary>
            public static string PaymentMethod
            {
                get { return "merchPaymentMethod"; }
            }

            //// ProductVariant -----------------------------------------------------------------

            /// <summary>
            /// Gets the product key reserved extended data key.
            /// </summary>
            public static string ProductKey
            {
                get { return "merchProductKey"; }
            }

            /// <summary>
            /// Gets the product variant key.
            /// </summary>
            public static string ProductVariantKey
            {
                get { return "merchProductVariantKey"; }
            }

            /// <summary>
            /// Gets the cost of goods reserved extended data key.
            /// </summary>
            public static string CostOfGoods
            {
                get { return "merchCostOfGoods"; }
            }

            /// <summary>
            /// Gets the weight reserved extended data key.
            /// </summary>
            public static string Weight
            {
                get { return "merchWeight"; }    
            }

            /// <summary>
            /// Gets the height reserved extended data key.
            /// </summary>
            public static string Height
            {
                get { return "merchHeight"; }
            }


            /// <summary>
            /// Gets the width reserved extended data key.
            /// </summary>
            public static string Width
            {
                get { return "merchWidth"; }
            }

            /// <summary>
            /// Gets the length reserved extended data key.
            /// </summary>
            public static string Length
            {
                get { return "merchLength"; }
            }

            /// <summary>
            /// Gets the barcode reserved extended data key.
            /// </summary>
            public static string Barcode
            {
                get { return "merchBarcode"; }
            }

            /// <summary>
            /// Gets the on sale reserved extended data key.
            /// </summary>
            public static string OnSale
            {
                get { return "merchOnSale"; }
            }

            /// <summary>
            /// Gets the manufacturer reserved extended data key.
            /// </summary>
            public static string Manufacturer
            {
                get { return "merchManufacturer"; }
            }

            /// <summary>
            /// Gets the manufacturer model number reserved extended data key.
            /// </summary>
            public static string ManufacturerModelNumber
            {
                get { return "merchManufacturerModelNumber"; }
            }

            /// <summary>
            /// Gets the sale price reserved extended data key.
            /// </summary>
            public static string SalePrice
            {
                get { return "merchSalePrice"; }
            }

            /// <summary>
            /// Gets the track inventory reserved extended data key.
            /// </summary>
            public static string TrackInventory
            {
                get { return "merchTrackInventory"; }
            }

            /// <summary>
            /// Gets the out of stock purchase reserved extended data key.
            /// </summary>
            public static string OutOfStockPurchase
            {
                get { return "merchOutOfStockPurchase"; }
            }

            /// <summary>
            /// Gets the taxable reserved extended data key.
            /// </summary>
            public static string Taxable
            {
                get { return "merchTaxable"; }
            }

            /// <summary>
            /// Gets the shippable reserved extended data key.
            /// </summary>
            public static string Shippable
            {
                get { return "merchShippable"; }
            }

            /// <summary>
            /// Gets the download reserved extended data key.
            /// </summary>
            public static string Download
            {
                get { return "merchDownload"; }
            }

            /// <summary>
            /// Gets the download media id reserved extended data key.
            /// </summary>
            public static string DownloadMediaId
            {
                get { return "merchDownloadMediaId"; }
            }

            //// Shipment -----------------------------------------------------------------------

            /// <summary>
            /// Gets the shipment key reserved extended data key.
            /// </summary>
            public static string ShipmentKey
            {
                get { return "merchShipmentKey"; }
            }

            /// <summary>
            /// Gets the ship method key reserved extended data key.
            /// </summary>
            public static string ShipMethodKey
            {
                get { return "merchShipMethodKey"; }
            }

            /// <summary>
            /// Gets the warehouse catalog key reserved extended data key.
            /// </summary>
            public static string WarehouseCatalogKey
            {
                get { return "merchWarehouseCatalogKey"; }
            }

            /// <summary>
            /// Gets the shipping origin address reserved extended data key.
            /// </summary>
            public static string ShippingOriginAddress
            {
                get { return "merchShippingOriginAddress"; }
            }

            /// <summary>
            /// Gets the shipping destination address reserved extended data key.
            /// </summary>
            public static string ShippingDestinationAddress
            {
                get { return "merchShippingDestinationAddress"; }
            }

            /// <summary>
            /// Gets the billing address reserved extended data key.
            /// </summary>
            public static string BillingAddress
            {
                get { return "merchBillingAddress"; }
            }

            //// LineItem -----------------------------------------------------------------------

            /// <summary>
            /// Gets the container key reserved extended data key.
            /// </summary>
            public static string ContainerKey
            {
                get { return "merchContainerKey"; }
            }

            /// <summary>
            /// Gets the line item type field key reserved extended data key.
            /// </summary>
            public static string LineItemTfKey
            {
                get { return "merchLineItemTfKey"; }
            }

            /// <summary>
            /// Gets the base tax rate reserved extended data key.
            /// </summary>
            public static string BaseTaxRate
            {
                get { return "merchBaseTaxRate"; }
            }

            /// <summary>
            /// Gets the provice tax rate reserved extended data key.
            /// </summary>
            public static string ProviceTaxRate
            {
                get { return "merchProvinceTaxRate"; }
            }

            /// <summary>
            /// Gets the line item tax amount reserved extended data key.
            /// </summary>
            public static string LineItemTaxAmount
            {
                get { return "merchLineItemTaxAmount"; }
            }

            /// <summary>
            /// Gets the tax transaction results.
            /// </summary>
            public static string TaxTransactionResults
            {
                get { return "merchTaxTransactionResults"; }
            }

            //// SMTP ---------------------------------------------------------------------------

            /// <summary>
            /// Gets the smtp provider settings reserved extended data key.
            /// </summary>
            public static string SmtpProviderSettings
            {
                get { return "merchSmtpProviderSettings"; }
            }            
        }
    }
}