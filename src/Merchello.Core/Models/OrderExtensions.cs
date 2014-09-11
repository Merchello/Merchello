using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Extension methods for <see cref="IOrder"/>
    /// </summary>
    public static class OrderExtensions
    {
        /// <summary>
        /// Returns a constructed order number (including it's invoice number prefix - if any)
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/></param>
        /// <returns>The prefixed order number</returns>
        public static string PrefixedOrderNumber(this IOrder order)
        {
            return string.IsNullOrEmpty(order.OrderNumberPrefix)
                ? order.OrderNumber.ToString(CultureInfo.InvariantCulture)
                : string.Format("{0}-{1}", order.OrderNumberPrefix, order.OrderNumber);
        }

#region Fulfillment


        /// <summary>
        /// Gets a collection of unfulfilled (unshipped) line items
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/></param>        
        /// <returns>A collection of <see cref="IOrderLineItem"/></returns>
        public static IEnumerable<IOrderLineItem> UnfulfilledItems(this IOrder order)
        {
            return order.UnfulfilledItems(MerchelloContext.Current);
        }

        /// <summary>
        /// Gets a collection of unfulfilled (unshipped) line items
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A collection of <see cref="IOrderLineItem"/></returns>
        public static IEnumerable<IOrderLineItem> UnfulfilledItems(this IOrder order, IMerchelloContext merchelloContext)
        {
            return order.UnfulfilledItems(merchelloContext, order.Items.Select(x => x as OrderLineItem));
        }

        /// <summary>
        /// Gets a collection of unfulfilled (unshipped) line items
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/></param>
        /// <param name="items">A collection of <see cref="IOrderLineItem"/></param>
        /// <returns>The collection of <see cref="IOrderLineItem"/></returns>
        public static IEnumerable<IOrderLineItem> UnfulfilledItems(this IOrder order, IEnumerable<IOrderLineItem> items)
        {
            return order.UnfulfilledItems(MerchelloContext.Current, items);
        }

        /// <summary>
        /// Gets a collection of unfulfilled (unshipped) line items
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <param name="items">A collection of <see cref="IOrderLineItem"/></param>
        /// <returns>The collection of <see cref="IOrderLineItem"/></returns>
        public static IEnumerable<IOrderLineItem> UnfulfilledItems(this IOrder order, IMerchelloContext merchelloContext, IEnumerable<IOrderLineItem> items)
        {

            if (Constants.DefaultKeys.OrderStatus.Fulfilled == order.OrderStatus.Key) return new List<IOrderLineItem>();

            var shippableItems = items.Where(x => x.IsShippable() && x.ShipmentKey == null).ToArray();

            var inventoryItems = shippableItems.Where(x => x.ExtendedData.GetTrackInventoryValue()).ToArray();

            // get the variants to check the inventory
            var variants = merchelloContext.Services.ProductVariantService.GetByKeys(inventoryItems.Select(x => x.ExtendedData.GetProductVariantKey())).ToArray();

            foreach (var item in inventoryItems)
            {
                var variant = variants.FirstOrDefault(x => x.Key == item.ExtendedData.GetProductVariantKey());
                if (variant == null) continue;

                // TODO refactor back ordering.
                //// check inventory
                //var inventory = variant.CatalogInventories.FirstOrDefault(x => x.CatalogKey == item.ExtendedData.GetWarehouseCatalogKey());
                //if (inventory != null)
                //    item.BackOrder = inventory.Count < item.Quantity;
            }

            return shippableItems;
        }

        /// <summary>
        /// Gets a collection of items that have inventory requirements
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/></param>
        /// <returns>A collection of <see cref="IOrderLineItem"/></returns>
        public static IEnumerable<IOrderLineItem> InventoryTrackedItems(this IOrder order)
        {
            return order.Items.Where(x => x.ExtendedData.GetTrackInventoryValue() && x.ExtendedData.ContainsWarehouseCatalogKey()).Select(x => (OrderLineItem)x);
        }



#endregion

        #region Examine

        /// <summary>
        /// Serializes <see cref="IOrder"/> object
        /// </summary>
        /// <remarks>
        /// Intended to be used by the Merchello.Examine.Providers.MerchelloOrderIndexer
        /// </remarks>
        internal static XDocument SerializeToXml(this IOrder order)
        {
            string xml;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("order");
                    writer.WriteAttributeString("id", ((Order)order).ExamineId.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("orderKey", order.Key.ToString());
                    writer.WriteAttributeString("invoiceKey", order.InvoiceKey.ToString());
                    writer.WriteAttributeString("orderNumberPrefix", order.OrderNumberPrefix);
                    writer.WriteAttributeString("orderNumber", order.OrderNumber.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("prefixedOrderNumber", order.PrefixedOrderNumber());
                    writer.WriteAttributeString("orderDate", order.OrderDate.ToString("s"));
                    writer.WriteAttributeString("orderStatusKey", order.OrderStatusKey.ToString());
                    writer.WriteAttributeString("versionKey", order.VersionKey.ToString());
                    writer.WriteAttributeString("exported", order.Exported.ToString());
                    writer.WriteAttributeString("orderStatus", GetOrderStatusJson(order.OrderStatus));
                    writer.WriteAttributeString("orderItems", GetGenericItemsCollection(order.Items));
                    writer.WriteAttributeString("createDate", order.CreateDate.ToString("s"));
                    writer.WriteAttributeString("updateDate", order.UpdateDate.ToString("s"));
                    writer.WriteAttributeString("allDocs", "1");
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    xml = sw.ToString();
                }
            }

            return XDocument.Parse(xml);
            
        }

        private static string GetGenericItemsCollection(IEnumerable<ILineItem> items)
        {
            return JsonConvert.SerializeObject(
                items.Select(x =>
                    new
                    {
                        key = x.Key,
                        containerKey = x.ContainerKey,
                        name = x.Name,
                        shipmentKey = ((OrderLineItem)x).ShipmentKey,
                        lineItemTfKey = x.LineItemTfKey,
                        lineItemType = x.LineItemType.ToString(),
                        sku = x.Sku,
                        price = x.Price,
                        quantity = x.Quantity,
                        exported = x.Exported,
                        backOrder = ((OrderLineItem)x).BackOrder
                    }
                ), Newtonsoft.Json.Formatting.None);
        }

        private static string GetOrderStatusJson(IOrderStatus orderStatus)
        {
            return JsonConvert.SerializeObject(
                    new
                    {
                        key = orderStatus.Key,
                        name = orderStatus.Name,
                        alias = orderStatus.Alias,
                        reportable = orderStatus.Reportable,
                        active = orderStatus.Active,
                        sortOrder = orderStatus.SortOrder
                    }, Newtonsoft.Json.Formatting.None);
        }

        #endregion
    }
}