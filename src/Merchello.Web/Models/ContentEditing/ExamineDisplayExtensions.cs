using System;
using System.Collections.Generic;
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
            return DateTime.TryParse(value, out converted) ? converted : DateTime.MinValue;
        }

        /// <summary>
        /// Converts a field value to a boolean
        /// </summary>
        public static bool FieldAsBoolean(string value)
        {
            return string.Equals("True", value);
        }

        #endregion
    }
}