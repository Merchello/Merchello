namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.WebPages;

    using Formatters;
    using Gateways.Shipping;
    using Gateways.Taxation;

    using Merchello.Core.Logging;

    using TypeFields;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Extension methods for <see cref="ILineItem"/>
    /// </summary>
    public static class LineItemExtensions
    {
        #region LineItemContainer
        
        /// <summary>
        /// Gets the tax line items.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The collection of tax line items.
        /// </returns>
        public static IEnumerable<ILineItem> TaxLineItems(this ILineItemContainer container)
        {
            return container.Items.Where(x => x.LineItemType == LineItemType.Tax);
        }

        /// <summary>
        /// Gets the product line items.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The collection of product line items.
        /// </returns>
        public static IEnumerable<ILineItem> ProductLineItems(this ILineItemContainer container)
        {
            return container.Items.Where(x => x.LineItemType == LineItemType.Product);
        }

        /// <summary>
        /// Gets the shipping line items.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The collection of shipping line item.
        /// </returns>
        public static IEnumerable<ILineItem> ShippingLineItems(this ILineItemContainer container)
        {
            return container.Items.Where(x => x.LineItemType == LineItemType.Shipping);
        }

        /// <summary>
        /// Gets the discount line items.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The collection of discount line items.
        /// </returns>
        public static IEnumerable<ILineItem> DiscountLineItems(this ILineItemContainer container)
        {
            return container.Items.Where(x => x.LineItemType == LineItemType.Discount);
        }

        /// <summary>
        /// Gets the custom line items.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The collection of customer line items.
        /// </returns>
        public static IEnumerable<ILineItem> CustomLineItems(this ILineItemContainer container)
        {
            return container.Items.Where(x => x.LineItemType == LineItemType.Custom);
        }

        /// <summary>
        /// Adds a <see cref="IProductVariant"/> line item to the collection
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        public static void AddItem(this ILineItemContainer container, IProductVariant productVariant, int quantity)
        {
            var extendedData = new ExtendedDataCollection();
            
            container.AddItem(productVariant, quantity, extendedData);
        }


        /// <summary>
        /// Adds a <see cref="IProductVariant"/> line item to the collection
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="productVariant">
        /// The product Variant.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        public static void AddItem(this ILineItemContainer container, IProductVariant productVariant, int quantity, ExtendedDataCollection extendedData)
        {
            extendedData.AddProductVariantValues(productVariant);
            container.AddItem(LineItemType.Product, productVariant.Name, productVariant.Sku, quantity, productVariant.Price, extendedData);
        }


        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="lineItemType">
        /// The line Item Type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        public static void AddItem(this ILineItemContainer container, LineItemType lineItemType, string name, string sku, int quantity, decimal amount)
        {
            container.AddItem(lineItemType, name, sku, quantity, amount, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="lineItemType">
        /// The line Item Type.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The sku.
        /// </param>
        /// <param name="quantity">
        /// The quantity.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        public static void AddItem(this ILineItemContainer container, LineItemType lineItemType, string name, string sku, int quantity, decimal amount, ExtendedDataCollection extendedData)
        {
            var lineItem = new ItemCacheLineItem(lineItemType, name, sku, quantity, amount, extendedData)
                {
                    ContainerKey = container.Key
                };
            
            container.AddItem(lineItem);
        }

        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="lineItem">
        /// The line Item.
        /// </param>
        public static void AddItem(this ILineItemContainer container, ILineItem lineItem)
        {
            lineItem.ContainerKey = container.Key;
            container.Items.Add(lineItem);
        }
       
        #endregion

        /// <summary>
        /// The allows validation.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool AllowsValidation(this ILineItem lineItem)
        {
            return lineItem.ExtendedData.GetAllowsValidationValue();
        }

        /// <summary>
        /// Converts a line item of one type to a line item of another type
        /// </summary>
        /// <typeparam name="T">The specific type of <see cref="ILineItem"/></typeparam>
        /// <param name="lineItem">The line item</param>
        /// <returns>A <see cref="LineItemBase"/> of type T</returns>
        public static T AsLineItemOf<T>(this ILineItem lineItem) where T : class, ILineItem
        {    
            var ctrValues = new object[]
                {                    
                    lineItem.LineItemTfKey,
                    lineItem.Name,
                    lineItem.Sku,
                    lineItem.Quantity,
                    lineItem.Price,
                    lineItem.ExtendedData
                };


            var attempt = ActivatorHelper.CreateInstance<LineItemBase>(typeof(T), ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<ILineItem>("Failed to convertion ILineItem", attempt.Exception);
                throw attempt.Exception;
            }

            attempt.Result.Exported = lineItem.Exported;

            return attempt.Result as T;
        }

        /// <summary>
        /// The as line item with key of.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T AsLineItemWithKeyOf<T>(this ILineItem lineItem) where T : class, ILineItem
        {
            var clone = lineItem.AsLineItemOf<T>();
            clone.Key = lineItem.Key;
            return clone;
        }


        /// <summary>
        /// Creates a line item of a particular type for a shipment rate quote
        /// </summary>
        /// <typeparam name="T">The type of the line item to create</typeparam>
        /// <param name="shipmentRateQuote">The <see cref="ShipmentRateQuote"/> to be translated to a line item</param>
        /// <returns>A <see cref="LineItemBase"/> of type T</returns>
        public static T AsLineItemOf<T>(this IShipmentRateQuote shipmentRateQuote) where T : LineItemBase
        {
            var extendedData = new ExtendedDataCollection();
            extendedData.AddShipment(shipmentRateQuote.Shipment);
            
            var ctrValues = new object[]
                {
                    EnumTypeFieldConverter.LineItemType.Shipping.TypeKey,
                    shipmentRateQuote.ShipmentLineItemName(),
                    shipmentRateQuote.ShipMethod.ServiceCode, // TODO this may not be unique (SKU) once multiple shipments are exposed
                    1,
                    shipmentRateQuote.Rate,
                    extendedData
                };

            var attempt = ActivatorHelper.CreateInstance<LineItemBase>(typeof(T), ctrValues);

            if (attempt.Success) return attempt.Result as T;

            MultiLogHelper.Error<ILineItem>("Failed instiating a line item from shipmentRateQuote", attempt.Exception);
            
            throw attempt.Exception;
        }

        /// <summary>
        /// Creates a line item of a particular type for a invoiceTaxResult
        /// </summary>
        /// <typeparam name="T">The type of the line item to be created</typeparam>
        /// <param name="taxCalculationResult">The <see cref="ITaxCalculationResult"/> to be converted to a line item</param>
        /// <returns>A <see cref="ILineItem"/> representing the <see cref="ITaxCalculationResult"/></returns>
        public static T AsLineItemOf<T>(this ITaxCalculationResult taxCalculationResult) where T : LineItemBase
        {
            var ctrValues = new object[]
            {
                EnumTypeFieldConverter.LineItemType.Tax.TypeKey,
                taxCalculationResult.Name,
                "Tax", // TODO this may not e unqiue (SKU),
                1,
                taxCalculationResult.TaxAmount,
                taxCalculationResult.ExtendedData
            };

            var attempt = ActivatorHelper.CreateInstance<LineItemBase>(typeof(T), ctrValues);

            if (attempt.Success) return attempt.Result as T;
            
            MultiLogHelper.Error<ILineItem>("Failed instiating a line item from invoiceTaxResult", attempt.Exception);
            
            throw attempt.Exception;
        }

        /// <summary>
        /// Returns a value indicating whether collection contains shippable items.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// A value indicating whether collection contains shippable items.
        /// </returns>
        public static bool HasShippableItems(this ILineItemContainer container)
        {
            return container.Items.Any(x => x.IsShippable());
        }

        /// <summary>
        /// Returns a collection of shippable line items
        /// </summary>
        /// <param name="container">The <see cref="ILineItemContainer"/></param>
        /// <returns>A collection of line items that can be shipped</returns>
        public static IEnumerable<ILineItem> ShippableItems(this ILineItemContainer container)
        {
            return container.Items.Where(x => x.IsShippable());
        }

        /// <summary>
        /// True/false indicating whether or not this lineItem represents a line item that can be shipped (a product)
        /// </summary>
        /// <param name="lineItem">
        /// The <see cref="ILineItem"/>
        /// </param>
        /// <returns>
        /// True or false indicating whether or not this line item represents a shippable line item
        /// </returns>
        public static bool IsShippable(this ILineItem lineItem)
        {
            return lineItem.LineItemType == LineItemType.Product &&
                   lineItem.ExtendedData.ContainsProductVariantKey() &&
                   lineItem.ExtendedData.GetShippableValue();
                   // &&
                   // lineItem.ExtendedData.ContainsWarehouseCatalogKey();
        }

        /// <summary>
        /// The get type field.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeField"/>.
        /// </returns>
        public static ITypeField GetTypeField(this ILineItem lineItem)
        {
            var type = EnumTypeFieldConverter.LineItemType.GetTypeField(lineItem.LineItemTfKey);
            var typeField = EnumTypeFieldConverter.LineItemType.Product;
            switch (type)
            {
                case LineItemType.Custom:
                    typeField =
                        EnumTypeFieldConverter.LineItemType.CustomTypeFields.FirstOrDefault(
                            x => x.TypeKey.Equals(lineItem.LineItemTfKey));
                    break;
                case LineItemType.Adjustment:
                    typeField = EnumTypeFieldConverter.LineItemType.Adjustment;
                    break;
                case LineItemType.Discount:
                    typeField = EnumTypeFieldConverter.LineItemType.Discount;
                    break;
                case LineItemType.Product:
                    typeField = EnumTypeFieldConverter.LineItemType.Product;
                    break;
                case LineItemType.Tax:
                    typeField = EnumTypeFieldConverter.LineItemType.Tax;
                    break;
                case LineItemType.Shipping:
                    typeField = EnumTypeFieldConverter.LineItemType.Shipping;
                    break;

            }

            return typeField;
        }


        /// <summary>
        /// Creates a new <see cref="ILineItemContainer"/> with filtered items.
        /// </summary>
        /// <param name="filteredItems">
        /// The line items.
        /// </param>
        /// <returns>
        /// The <see cref="ILineItemContainer"/>.
        /// </returns>
        public static ILineItemContainer CreateNewItemCacheLineItemContainer(IEnumerable<ILineItem> filteredItems)
        {
            var lineItems = filteredItems as ILineItem[] ?? filteredItems.ToArray();

            var result = new ItemCache(Guid.NewGuid(), ItemCacheType.Backoffice);
            if (!lineItems.Any()) return result;

            var itemCacheLineItems = lineItems.Select(x => x.AsLineItemOf<ItemCacheLineItem>());

            result.Items.Add(itemCacheLineItems);
            return result;
        }

        #region Formatter

        /// <summary>
        /// Gets the 'Iteration token' used by the PatternReplaceFormatter to identify line item iterations
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The iteration identifier
        /// </returns>
        [Obsolete]
        internal static string GetFormatterIterationIdentifier(this ILineItemContainer container)
        {
            if (container is IInvoice) return "Invoice.Items";
            if (container is IOrder) return "Order.Items";
            if (container is IItemCache) return "ItemCache.Items";
            if (container is IShipment) return "Shipment.Items";

            throw new InvalidOperationException("ILineItemContainer passed does not have a FormatterIterationToken");
        }

        /// <summary>
        /// Gets a collection of <see cref="IReplaceablePattern"/> for each line item in the <see cref="LineItemCollection"/>
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="currencySymbol">
        /// The currency Symbol.
        /// </param>
        /// <returns>
        /// A collection of replaceable patterns
        /// </returns>
        [Obsolete]
        internal static IEnumerable<IReplaceablePattern> LineItemReplaceablePatterns(this ILineItemContainer container, string currencySymbol)
        {
            var patterns = new List<IReplaceablePattern>();

            var token = container.GetFormatterIterationIdentifier();

            var iterateItems =
                container.Items.Where(
                    x => x.LineItemType == LineItemType.Product || x.LineItemType == LineItemType.Custom).ToArray();

            // TODO localization needed on pricing and datetime
            for (var i = 0; i < iterateItems.Count(); i++)
            {
                var sku = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "Sku", i), string.Format("{0}Item.Sku.{1}{2}", "{{", i, "}}"), iterateItems[i].LineItemType == LineItemType.Shipping ? string.Empty : iterateItems[i].Sku);
                var unitPrice = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "UnitPrice", i), string.Format("{0}Item.UnitPrice.{1}{2}", "{{", i, "}}"), iterateItems[i].Price.FormatAsPrice(currencySymbol));
                var name = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "Name", i), string.Format("{0}Item.Name.{1}{2}", "{{", i, "}}"), iterateItems[i].Name);
                var qty = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "Quantity", i), string.Format("{0}Item.Quantity.{1}{2}", "{{", i, "}}"), iterateItems[i].Quantity.ToString(CultureInfo.InvariantCulture));
                var totalPrice = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "TotalPrice", i), string.Format("{0}Item.TotalPrice.{1}{2}", "{{", i, "}}"), iterateItems[i].TotalPrice.FormatAsPrice(currencySymbol));

                patterns.Add(sku);
                patterns.Add(name);
                patterns.Add(unitPrice);
                patterns.Add(qty);
                patterns.Add(totalPrice);
            }

            //var shippingLineItems = container.ShippingLineItems().ToArray();

            //// TODO - needs work once we expose multiple shipments to do an array if multiple destinations are detected
            //if (shippingLineItems.Any())
            //{
            //    var shipment = shippingLineItems.First().ExtendedData.GetShipment<InvoiceLineItem>();
            //    patterns.Add(ReplaceablePattern.GetConfigurationReplaceablePattern("ShipToName", shipment.ToName));

            //}


            return patterns.Where(x => x != null);
        }

        #endregion
    }
}

