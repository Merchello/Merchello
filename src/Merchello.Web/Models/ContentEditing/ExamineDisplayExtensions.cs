namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using global::Examine;

    using Merchello.Core.Models;
    using Merchello.Examine;
    using Merchello.Web.Search;

    using Newtonsoft.Json;

    /// <summary>
    /// Extension methods to map examine (Lucene) documents to respective "Display" object classes
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ExamineDisplayExtensions
    {
        /// <summary>
        /// The to product display.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        internal static ProductDisplay ToProductDisplay(this SearchResult result, Func<Guid, IEnumerable<ProductVariantDisplay>> getProductVariants)
        {
            // this should be the master variant
            var productDisplay = new ProductDisplay(result.ToProductVariantDisplay());

            productDisplay.ProductVariants = getProductVariants(productDisplay.Key);
            productDisplay.ProductOptions = RawJsonFieldAsCollection<ProductOptionDisplay>(result, "productOptions");

            return productDisplay;
        }

        /// <summary>
        /// Converts Lucene cached ProductVariant into <see cref="ProductVariantDisplay"/>
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        internal static ProductVariantDisplay ToProductVariantDisplay(this SearchResult result)
        {
            var pvd = new ProductVariantDisplay()
            {
                Key = FieldAsGuid(result, "productVariantKey"),
                ProductKey = FieldAsGuid(result, "productKey"),
                Name = FieldAsString(result, "name"),
                Sku = FieldAsString(result, "sku"),
                Price = FieldAsDecimal(result, "price"),
                OnSale = FieldAsBoolean(result.Fields["onSale"]),
                SalePrice = FieldAsDecimal(result, "salePrice"),
                CostOfGoods = FieldAsDecimal(result, "costOfGoods"),
                Weight = FieldAsDecimal(result, "weight"),
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
                VersionKey = FieldAsGuid(result, "versionKey"),
                Attributes = RawJsonFieldAsCollection<ProductAttributeDisplay>(result, "attributes"),
                CatalogInventories = RawJsonFieldAsCollection<CatalogInventoryDisplay>(result, "catalogInventories")
            };

            return pvd;
        }

        /// <summary>
        /// Converts a Lucene cached invoice into <see cref="InvoiceDisplay"/>
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="getOrders">
        /// The get Orders.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        internal static InvoiceDisplay ToInvoiceDisplay(this SearchResult result, Func<Guid, IEnumerable<OrderDisplay>> getOrders)
        {
            var invoice = new InvoiceDisplay()
                {
                    Key = FieldAsGuid(result, "invoiceKey"),
                    InvoiceNumberPrefix = FieldAsString(result, "invoiceNumberPrefix"),
                    InvoiceNumber = FieldAsInteger(result, "invoiceNumber"),
                    InvoiceDate = FieldAsDateTime(result, "invoiceDate"),
                    InvoiceStatusKey = FieldAsGuid(result, "invoiceStatusKey"),
                    CustomerKey = FieldAsGuid(result, "customerKey"),
                    VersionKey = FieldAsGuid(result, "versionKey"),
                    BillToName = FieldAsString(result, "billToName"),
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

            invoice.Orders = getOrders(invoice.Key);

            return invoice;
        }

        /// <summary>
        /// Converts a Lucene cached order into <see cref="OrderDisplay"/>
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="OrderDisplay"/>.
        /// </returns>
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

        /// <summary>
        /// Converts a Lucene index result into a <see cref="CustomerDisplay"/>.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        internal static CustomerDisplay ToCustomerDisplay(this SearchResult result)
        {
            return new CustomerDisplay()
            {
                Key = FieldAsGuid(result, "customerKey"),
                LoginName = FieldAsString(result, "loginName"),
                FirstName = FieldAsString(result, "firstName"),
                LastName = FieldAsString(result, "lastName"),
                Email = FieldAsString(result, "email"),
                Notes = FieldAsString(result, "notes"),
                TaxExempt = FieldAsBoolean(result.Fields["taxExempt"]),
                ExtendedData =
                    RawJsonFieldAsCollection<KeyValuePair<string, string>>(result, "extendedData")
                        .AsExtendedDataCollection(),
                Addresses = RawJsonFieldAsCollection<CustomerAddress>(result, "addresses").Select(x => x.ToCustomerAddressDisplay()),
                LastActivityDate = FieldAsDateTime(result, "lastActivityDate")
            };
        }


        #region "Utility methods"

        /// <summary>
        /// Deserializes the a raw JSON field
        /// </summary>
        /// <typeparam name="T">
        /// The type to be deserialized
        /// </typeparam>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The collection of T
        /// </returns>
        private static IEnumerable<T> RawJsonFieldAsCollection<T>(SearchResult result, string alias)
        {
            return !result.Fields.ContainsKey(alias)
                ? new List<T>()
                : JsonConvert.DeserializeObject<IEnumerable<T>>(result.Fields[alias]);
        }

        /// <summary>
        /// Deserializes a the raw JSON field
        /// </summary>
        /// <typeparam name="T">
        /// The type to be deserialized
        /// </typeparam>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private static T JsonFieldAs<T>(SearchResult result, string alias)
        {
            return !result.Fields.ContainsKey(alias)
                       ? default(T)
                       : JsonConvert.DeserializeObject<T>(result.Fields[alias]);
        }

        /// <summary>
        /// Converts a field value to a Guid or Guid.Empty if not found
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        internal static Guid FieldAsGuid(SearchResult result, string alias)
        {
            if (!result.Fields.ContainsKey(alias)) return Guid.Empty;

            var value = result.Fields[alias];
            Guid converted;
            return Guid.TryParse(value, out converted) ? converted : Guid.Empty;
        }

        /// <summary>
        /// Converts a field value to a decimal or 0 if not found
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal FieldAsDecimal(SearchResult result, string alias)
        {
            if (!result.Fields.ContainsKey(alias)) return 0;
            string value = result.Fields[alias];

            decimal converted = decimal.TryParse(value, System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out converted) ? converted : 0;
            return converted;
        }

        /// <summary>
        /// Converts a field value to an integer or 0 if not found
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
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
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
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
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool FieldAsBoolean(string value)
        {
            return string.Equals("True", value);
        }

        /// <summary>
        /// Returns the field value as a string if available or string.Empty
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FieldAsString(SearchResult result, string alias)
        {
            return !result.Fields.ContainsKey(alias) ? string.Empty : result.Fields[alias];
        }

        #endregion
    }
}
