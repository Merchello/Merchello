using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using Merchello.Core.Formatters;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Logging;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="ILineItem"/>
    /// </summary>
    public static class LineItemExtensions
    {

        #region LineItemContainer
        
        /// <summary>
        /// Adds a <see cref="IProductVariant"/> line item to the collection
        /// </summary>
        public static void AddItem(this ILineItemContainer container, IProductVariant productVariant, int quantity)
        {
            var extendedData = new ExtendedDataCollection();
            
            container.AddItem(productVariant, quantity, extendedData);
        }


        /// <summary>
        /// Adds a <see cref="IProductVariant"/> line item to the collection
        /// </summary>
        public static void AddItem(this ILineItemContainer container, IProductVariant productVariant, int quantity, ExtendedDataCollection extendedData)
        {
            extendedData.AddProductVariantValues(productVariant);
            container.AddItem(LineItemType.Product, productVariant.Name, productVariant.Sku, quantity, productVariant.Price, extendedData);
        }


        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
        public static void AddItem(this ILineItemContainer container, LineItemType lineItemType, string name, string sku, int quantity, decimal amount)
        {
            container.AddItem(lineItemType, name, sku, quantity, amount, new ExtendedDataCollection());
        }

        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
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
        public static void AddItem(this ILineItemContainer container, ILineItem lineItem)
        {
            container.Items.Add(lineItem);
        }
       
        #endregion

        /// <summary>
        /// Converts a line item of one type to a line item of another type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lineItem"></param>
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
                    shipmentRateQuote.ShimpentLineItemName(),
                    shipmentRateQuote.ShipMethod.ServiceCode, // TODO this may not be unique (SKU) once multiple shipments are exposed
                    1,
                    shipmentRateQuote.Rate,
                    extendedData
                };

            var attempt = ActivatorHelper.CreateInstance<LineItemBase>(typeof (T), ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<ILineItem>("Failed instiating a line item from shipmentRateQuote", attempt.Exception);
                throw attempt.Exception;
            }
            return attempt.Result as T;
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

            var attempt = ActivatorHelper.CreateInstance<LineItemBase>(typeof (T), ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<ILineItem>("Failed instiating a line item from invoiceTaxResult", attempt.Exception);
                throw attempt.Exception;
            }

            return attempt.Result as T;
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
        /// True/false indicating whether or not this lineItem represents a line item that can be shipped (eg. a product)
        /// </summary>
        /// <param name="lineItem">The <see cref="ILineItem"/></param>
        public static bool IsShippable(this ILineItem lineItem)
        {
            return lineItem.LineItemType == LineItemType.Product &&
                   lineItem.ExtendedData.ContainsProductVariantKey() &&
                   lineItem.ExtendedData.GetShippableValue() &&
                   lineItem.ExtendedData.ContainsWarehouseCatalogKey();
        }


        #region Formatter

        /// <summary>
        /// Gets the 'Iteration token' used by the PatternReplaceFormatter to identify line item iterations
        /// </summary>
        internal static string GetFormatterIterationToken(this ILineItemContainer container)
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
        internal static IEnumerable<IReplaceablePattern> LineItemReplaceablePatterns(this ILineItemContainer container)
        {
            var patterns = new List<IReplaceablePattern>();

            var token = container.GetFormatterIterationToken();

            // TODO localization needed on pricing and datetime
            for (var i = 0; i < container.Items.Count; i++)
            {

                var sku = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "Sku", i), string.Format("{0}Item.Sku.{1}{2}", "{{", i, "}}"), container.Items[i].Sku);
                var unitPrice = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "UnitPrice", i), string.Format("{0}Item.UnitPrice.{1}{2}", "{{", i, "}}"), container.Items[i].Price.ToString("C"));
                var name = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "Name", i), string.Format("{0}Item.Name.{1}{2}", "{{", i, "}}"), container.Items[i].Name);
                var qty = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "Quantity", i), string.Format("{0}Item.Quantity.{1}{2}", "{{", i, "}}"), container.Items[i].Quantity.ToString(CultureInfo.InvariantCulture));
                var totalPrice = new ReplaceablePattern(string.Format("{0}.{1}.{2}", token, "TotalPrice", i), string.Format("{0}Item.TotalPrice.{1}{2}", "{{", i, "}}"), container.Items[i].TotalPrice.ToString("C"));

                patterns.Add(sku);
                patterns.Add(name);
                patterns.Add(unitPrice);
                patterns.Add(qty);
                patterns.Add(totalPrice);
            }

            return patterns;
        }

        #endregion
    }
}

