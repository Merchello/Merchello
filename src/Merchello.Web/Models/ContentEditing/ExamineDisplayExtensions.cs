using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Examine;
using Merchello.Core.Models;
using Merchello.Examine;
using Newtonsoft.Json;

namespace Merchello.Web.Models.ContentEditing
{
    /// <summary>
    /// Extension methods to map examine (lucene) documents to respective "Display" object classes
    /// </summary>
    internal static class ExamineDisplayExtensions
    {
        
        internal static ProductDisplay ToProductDisplay(this SearchResult result)
        {
            // this should be the master variant
            var productDisplay = new ProductDisplay(result.ToProductVariantDisplay());

            var searcher = ExamineManager.Instance.SearchProviderCollection["MerchelloProductSearcher"];
            var criteria = ExamineManager.Instance.CreateSearchCriteria(IndexTypes.ProductVariant);
            criteria.Field("productKey", productDisplay.Key.ToString()).Not().Field("master", "True");

            var variants = searcher.Search(criteria);

            productDisplay.ProductVariants =  variants.Select(variant => variant.ToProductVariantDisplay()).ToList();
            productDisplay.ProductOptions = RawJsonFieldAsCollection<ProductOptionDisplay>(result, "productOptions");

            return productDisplay;
        }

        /// <summary>
        /// Converts Lucene cached ProductVariant into <see cref="ProductVariantDisplay"/>
        /// </summary>
        internal static ProductVariantDisplay ToProductVariantDisplay(this SearchResult result)
        {
            var pvd = new ProductVariantDisplay()
            {
                Key = FieldAsGuid(result, "productVariantKey"),
                ProductKey = FieldAsGuid(result, "productKey"),
                Name = result.Fields["name"],
                Sku = result.Fields["sku"],
                Price = FieldAsDecimal(result, "price"),
                OnSale = FieldAsBoolean(result.Fields["onSale"]),
                SalePrice = FieldAsDecimal(result, "salePrice"),
                CostOfGoods = FieldAsDecimal(result,"costOfGoods"),
                Weight = FieldAsDecimal(result,"weight"),
                Length = FieldAsDecimal(result, "length"),
                Height = FieldAsDecimal(result, "height"),
                Width = FieldAsDecimal(result, "width"),
                Barcode = result.Fields.ContainsKey("barcode") ? result.Fields["barcode"] : string.Empty,
                Available = FieldAsBoolean(result.Fields["available"]),
                TrackInventory = FieldAsBoolean(result.Fields["trackInventory"]),
                OutOfStockPurchase = FieldAsBoolean(result.Fields["outOfStockPurchase"]),
                Taxable = FieldAsBoolean(result.Fields["taxable"]),
                Shippable = FieldAsBoolean(result.Fields["shippable"]),
                Download = FieldAsBoolean(result.Fields["download"]),
                DownloadMediaId = FieldAsInteger(result, "downloadMediaId"),
                Attributes = RawJsonFieldAsCollection<ProductAttributeDisplay>(result, "attributes"),
                CatalogInventories = RawJsonFieldAsCollection<CatalogInventoryDisplay>(result, "catalogInventories")
            };

            return pvd;
        }

        /// <summary>
        /// Converts a Lucene cached invoice into <see cref="InvoiceDisplay"/>
        /// </summary>
        internal static InvoiceDisplay ToInvoiceDisplay(this SearchResult result)
        {
            var invoice = new InvoiceDisplay()
                {
                    Key = FieldAsGuid(result, "invoiceKey"),
                    InvoiceNumberPrefix = FieldAsString(result, "invoiceNumberPrefix"),
                    InvoiceNumber = FieldAsInteger(result, "invoiceNumber"),
                    InvoiceDate = FieldAsDateTime(result, "invoiceDate"),
                    InvoiceStatusKey = FieldAsGuid(result, "invoiceStatusKey"),
                    VersionKey = FieldAsGuid(result, "versionKey"),
                    BillToName = result.Fields["billToName"],
                    BillToAddress1 = FieldAsString(result, "billToAddress1"),
                    BillToAddress2 = FieldAsString(result, "billToAddress2"),
                    BillToLocality = FieldAsString(result, "billToLocality"),
                    BillToRegion = FieldAsString(result,"billoToRegion"),
                    BillToCountryCode = FieldAsString(result, "billToCountryCode"),
                    BillToPostalCode = FieldAsString(result, "billToPostalCode"),
                    BillToCompany = FieldAsString(result, "billToCompany"),
                    BillToPhone = FieldAsString(result,"billToPhone"),
                    BillToEmail = FieldAsString(result, "billToEmail"),
                    Exported = FieldAsBoolean(result.Fields["exported"]),
                    Archived = FieldAsBoolean(result.Fields["archived"]),
                    Total = FieldAsDecimal(result, "total"),
                    InvoiceStatus = JsonFieldAs<InvoiceStatusDisplay>(result, "invoiceStatus"),
                    Items = RawJsonFieldAsCollection<InvoiceLineItemDisplay>(result, "invoiceItems"),
                    
                };

            invoice.Orders = OrderQuery.GetByInvoiceKey(invoice.Key);

            return invoice;
        }

        /// <summary>
        /// Converts a Lucene cached order into <see cref="OrderDisplay"/>
        /// </summary>
        internal static OrderDisplay ToOrderDisplay(this SearchResult result)
        {
            return new OrderDisplay()
                {
                    Key = FieldAsGuid(result, "orderKey"),
                    InvoiceKey = FieldAsGuid(result, "invoiceKey"),
                    OrderNumberPrefix = FieldAsString(result, "orderNumberPrefix"),
                    OrderNumber = FieldAsInteger(result, "orderNumber"),
                    OrderDate = FieldAsDateTime(result, "orderDate"),
                    OrderStatusKey = FieldAsGuid(result, "orderStatusKey"),
                    VersionKey = FieldAsGuid(result, "versionKey"),
                    Exported = FieldAsBoolean(result.Fields["exported"]),
                    OrderStatus = JsonFieldAs<OrderStatusDisplay>(result, "orderStatus"),
                    Items = RawJsonFieldAsCollection<OrderLineItemDisplay>(result, "orderItems")
                };
        }


        #region "Utility methods"


        /// <summary>
        /// Deserializes the a raw JSON field
        /// </summary>
        private static IEnumerable<T> RawJsonFieldAsCollection<T>(SearchResult result, string alias)
        {
            return !result.Fields.ContainsKey(alias)
                ? new List<T>()
                : JsonConvert.DeserializeObject<IEnumerable<T>>(result.Fields[alias]);

        }
        /// <summary>
        /// Deserializes a the raw JSON field
        /// </summary>
        private static T JsonFieldAs<T>(SearchResult result, string alias)
        {
            return !result.Fields.ContainsKey(alias)
                       ? default(T)
                       : JsonConvert.DeserializeObject<T>(result.Fields[alias]);
        }

        /// <summary>
        /// Converts a field value to a Guid or Guid.Empty if not found
        /// </summary>
        private static Guid FieldAsGuid(SearchResult result, string alias)
        {
            if (!result.Fields.ContainsKey(alias)) return Guid.Empty;

            var value = result.Fields[alias];
            Guid converted;
            return Guid.TryParse(value, out converted) ? converted : Guid.Empty;
        }

        /// <summary>
        /// Converts a field value to a decimal or 0 if not found
        /// </summary>
        public static decimal FieldAsDecimal(SearchResult result, string alias)
        {
            if (!result.Fields.ContainsKey(alias)) return 0;
            var value = result.Fields[alias];

            decimal converted;
            return decimal.TryParse(value, out converted) ? converted : 0;
        }

        /// <summary>
        /// Converts a field value to an int or 0 if not found
        /// </summary>
        public static int FieldAsInteger(SearchResult result, string alias)
        {
            if (!result.Fields.ContainsKey(alias)) return 0;
            var value = result.Fields[alias];

            int converted;
            return int.TryParse(value, out converted) ? converted : 0;
        }

        /// <summary>
        /// Converts a field value to a DateTime or DateTime.MinValue if not found
        /// </summary>
        public static DateTime FieldAsDateTime(SearchResult result, string alias)
        {
            if (!result.Fields.ContainsKey(alias)) return DateTime.MinValue;
            var value = result.Fields[alias];
            
            DateTime converted;
            if (value.Length > 8) value = value.Substring(0, 8);

            // http://our.umbraco.org/forum/core/general/12331-Examine-date-fields-in-wrong-culture
            return DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out converted) 
                ? converted : DateTime.MinValue;
        }

        /// <summary>
        /// Converts a field value to a boolean
        /// </summary>
        public static bool FieldAsBoolean(string value)
        {
            return string.Equals("True", value);
        }

        /// <summary>
        /// Returns the field value as a string if available or string.Empty
        /// </summary>
        public static string FieldAsString(SearchResult result, string alias)
        {
            if (!result.Fields.ContainsKey(alias)) return string.Empty;
            return result.Fields[alias];
        }

        #endregion
    }
}