using System;
using System.Linq;
using Examine;
using Examine.LuceneEngine;

namespace Merchello.Web.Models.ContentEditing
{
    /// <summary>
    /// Extension methods to map examine (lucene) documents to respective "Display" object classes
    /// </summary>
    public static class ExamineDisplayExtensions
    {



        public static ProductVariantDisplay ToProductVariantDisplay(this SearchResult result)
        {
            var pvd = new ProductVariantDisplay()
            {
                Id = FieldAsInteger(result.Fields["id"]),
                ProductKey = FieldAsGuid(result.Fields["productKey"]),
                Name = result.Fields["name"],
                Sku = result.Fields["sku"],
                Price = FieldAsDecimal(result.Fields["price"]),
                OnSale = FieldAsBoolean(result.Fields["onSale"]),
                SalePrice = FieldAsDecimal(result.Fields["salePrice"]),
                CostOfGoods = FieldAsDecimal(result.Fields["costOfGoods"]),
                Weight = FieldAsDecimal(result.Fields["weight"]),
                Length = FieldAsDecimal(result.Fields["length"]),
                Height = FieldAsDecimal(result.Fields["height"]),
                Width = FieldAsDecimal(result.Fields["width"]),
                Barcode = result.Fields["barcode"],
                Available = FieldAsBoolean(result.Fields["available"]),
                TrackInventory = FieldAsBoolean(result.Fields["trackInventory"]),
                OutOfStockPurchase = FieldAsBoolean(result.Fields["outOfStockPurchase"]),
                Taxable = FieldAsBoolean(result.Fields["taxable"]),
                Shippable = FieldAsBoolean(result.Fields["shippable"]),
                Download = FieldAsBoolean(result.Fields["download"]),
                DownloadMediaId = FieldAsInteger(result.Fields["downloadMediaId"])
            };

            return pvd;
        }

        #region "Utility methods"
        

       


        private static Guid FieldAsGuid(string value)
        {
            Guid converted;
            return Guid.TryParse(value, out converted) ? converted : Guid.Empty;
        }

        public static Decimal FieldAsDecimal(string value)
        {
            decimal converted;
            return decimal.TryParse(value, out converted) ? converted : 0;
        }

        public static int FieldAsInteger(string value)
        {
            int converted;
            return int.TryParse(value, out converted) ? converted : 0;
        }

        public static DateTime FieldAsDateTime(string value)
        {
            DateTime converted;
            return DateTime.TryParse(value, out converted) ? converted : DateTime.MinValue;
        }

        public static bool FieldAsBoolean(string value)
        {
            return string.Equals("True", value);
        }

        #endregion
    }
}