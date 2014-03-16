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