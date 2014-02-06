using System;
using Merchello.Core.Gateways.Shipping;
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
            var lineItem = new ItemCacheLineItem(lineItemType, name, sku, quantity, amount, extendedData)
                {
                    ContainerKey = container.Key
                };
            
            container.AddItem(lineItem);
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
        /// <returns>A <see cref="LineItemBase"/> of type T</returns>
        public static T ConvertToNewLineItemOf<T>(this ILineItem lineItem) where T : LineItemBase
        {    
            var ctrValues = new object[]
                {                    
                    lineItem.LineItemTfKey,
                    lineItem.Name,
                    lineItem.Sku,
                    lineItem.Quantity,
                    lineItem.Amount,
                    lineItem.ExtendedData
                };

            var converted = ActivatorHelper.CreateInstance<LineItemBase>(typeof(T), LineItemConstructorArgs, ctrValues);
            converted.Exported = lineItem.Exported;

            return converted as T;
        }


        /// <summary>
        /// Creates a line item of a particular type for a shipment rate quote
        /// </summary>
        /// <typeparam name="T">The type of the line item to create</typeparam>
        /// <param name="shipmentRateQuote">The <see cref="ShipmentRateQuote"/> to be translated to a line item</param>
        /// <returns>A <see cref="LineItemBase"/> of type T</returns>
        public static ILineItem AsLineItemOf<T>(this IShipmentRateQuote shipmentRateQuote) where T : LineItemBase
        {
            var extendedData = new ExtendedDataCollection();
            extendedData.AddShipment(shipmentRateQuote.Shipment);

            var ctrValues = new object[]
                {
                    EnumTypeFieldConverter.LineItemType.Shipping.TypeKey,
                    shipmentRateQuote.ShipMethod.Name,
                    shipmentRateQuote.ShipMethod.ServiceCode, // TODO this may not be unique once multiple shipments are exposed
                    1,
                    shipmentRateQuote.Rate,
                    extendedData
                };

            return ActivatorHelper.CreateInstance<LineItemBase>(typeof (T), LineItemConstructorArgs, ctrValues);
        }


        /// <summary>
        /// LineItemBase constructor argument types
        /// </summary>
        private static Type[] LineItemConstructorArgs
        {
            get
            {
                return new[] { typeof(Guid), typeof(string), typeof(string), typeof(int), typeof(decimal), typeof(ExtendedDataCollection) };
            }
        }
    }
}

