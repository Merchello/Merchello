using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="IItemCache"/>
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
            container.AddItem(new ItemCacheLineItem(container.Key, lineItemType, name, sku, quantity, amount, extendedData));
        }

        /// <summary>
        /// Adds a line item to the collection
        /// </summary>
        public static void AddItem(this ILineItemContainer container, IItemCacheLineItem lineItem)
        {
            container.Items.Add(lineItem);
        }

        #endregion

        /// <summary>
        /// Converts a line item of one type to a line item of another type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lineItem"></param>
        /// <returns>Returns a line item of type T</returns>
        public static T ConvertToNewLineItem<T>(this ILineItem lineItem) where T : LineItemBase
        {
            var ctrArgs = new[]
                {
                    typeof (Guid), typeof (Guid), typeof (string), typeof (string), typeof (int), typeof (decimal), typeof (ExtendedDataCollection)
                };
            
            var ctrValues = new object[]
                {
                    Guid.Empty,
                    lineItem.LineItemTfKey,
                    lineItem.Sku,
                    lineItem.Name,
                    lineItem.Quantity,
                    lineItem.Amount,
                    lineItem.ExtendedData
                };

            var converted = ActivatorHelper.CreateInstance<LineItemBase>(typeof(T), ctrArgs, ctrValues);
            converted.Exported = lineItem.Exported;

            return converted as T;
        }
    }
}

