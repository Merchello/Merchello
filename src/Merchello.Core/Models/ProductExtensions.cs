using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Merchello.Core.Models
{
    public static class ProductExtensions
    {

        #region IProduct Collections

        /// <summary>
        /// Returns a collection of ProductOption given as list of attributes (choices)
        /// </summary>
        /// <param name="product"></param>
        /// <param name="attributes">A collection of <see cref="IProductAttribute"/></param>
        /// <remarks>
        /// This is mainly used for suggesting sku defaults for ProductVariantes
        /// </remarks>
        public static IEnumerable<IProductOption> ProductOptionsForAttributes(this IProduct product, IEnumerable<IProductAttribute> attributes)
        {
            var options = new List<IProductOption>();
            foreach (var att in attributes)
            {
                options.AddRange(product.ProductOptions.Where(option => option.Choices.Any(choice => choice.Id == att.Id)));
            }
            return options;
        }


        /// <summary>
        /// Returns the "master" <see cref="IProductVariant"/> that defines this <see cref="IProduct" /> or null if this <see cref="IProduct" /> has options
        /// </summary>
        /// <returns><see cref="IProductVariant"/> or null if this <see cref="IProduct" /> has options</returns>
        public static IProductVariant GetProductVariantForPurchase(this IProduct product)
        {
            return product.ProductOptions.Any() ? null : ((Product)product).MasterVariant;
        }

        /// <summary>
        /// Returns the <see cref="IProductVariant"/> of this <see cref="IProduct"/> that contains a matching collection of <see cref="IProductAttribute" />. 
        /// If not match is found, returns null.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="selectedChoices">A collection of <see cref="IProductAttribute"/> which define the specific <see cref="IProductVariant"/> of the <see cref="IProduct"/></param>
        /// <returns><see cref="IProductVariant"/> or null if no <see cref="IProductVariant"/> is found with a matching collection of <see cref="IProductAttribute"/></returns>
        public static IProductVariant GetProductVariantForPurchase(this IProduct product, IEnumerable<IProductAttribute> selectedChoices)
        {
            return
                product.ProductVariants.FirstOrDefault(
                    variant =>
                    {
                        var productAttributes = selectedChoices as IProductAttribute[] ?? selectedChoices.ToArray();
                        return variant.Attributes.Count() == productAttributes.Count() &&
                                          productAttributes.All(item => ((ProductAttributeCollection)variant.Attributes).Contains(item.Id));
                    });

        }

        /// <summary>
        /// Returns the <see cref="IProductVariant"/> of this <see cref="IProduct"/> that contains a matching collection of <see cref="IProductAttribute" />. 
        /// If not match is found, returns null.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="selectedChoiceIds"></param>
        /// <returns><see cref="IProductVariant"/> or null if no <see cref="IProductVariant"/> is found with a matching collection of <see cref="IProductAttribute"/></returns>
        public static IProductVariant GetProductVariantForPurchase(this IProduct product, int[] selectedChoiceIds)
        {
            return
                product.ProductVariants.FirstOrDefault(
                    variant => variant.Attributes.Count() == selectedChoiceIds.Length &&
                               selectedChoiceIds.All(id => ((ProductAttributeCollection)variant.Attributes).Contains(id)));
        }

        #endregion

        #region XML Methods


        public static XDocument SerializeToXml(this IProduct product)
        {
            string xml;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("product");
                    writer.WriteAttributeString("key", product.Key.ToString());                                        
                    writer.WriteEndElement(); // product
                    writer.WriteEndDocument();
                    xml = sw.ToString();
                }
            }

            var doc = XDocument.Parse(xml);
            if (doc.Root == null) return XDocument.Parse("<product />");
                
            doc.Root.Add(((Product)product).MasterVariant.SerializeToXml().Root);
            
            foreach (var variant in product.ProductVariants)
            {
                doc.Root.Add(variant.SerializeToXml().Root);
            }
            return doc;
        }


        public static XDocument SerializeToXml(this IProductVariant productVariant)
        {
            string xml;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("productVariant");
                    writer.WriteAttributeString("id", productVariant.Id.ToString());
                    writer.WriteAttributeString("productKey", productVariant.ProductKey.ToString());
                    writer.WriteAttributeString("master", ((ProductVariant)productVariant).Master.ToString());
                    writer.WriteAttributeString("name", productVariant.Name);
                    writer.WriteAttributeString("sku", productVariant.Sku);
                    writer.WriteAttributeString("price", productVariant.Price.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("costOfGoods", productVariant.CostOfGoods.ToString());
                    writer.WriteAttributeString("salePrice", productVariant.SalePrice.ToString());
                    writer.WriteAttributeString("onSale", productVariant.OnSale.ToString());
                    writer.WriteAttributeString("weight", productVariant.Weight.ToString());
                    writer.WriteAttributeString("length", productVariant.Length.ToString());
                    writer.WriteAttributeString("width", productVariant.Width.ToString());
                    writer.WriteAttributeString("height", productVariant.Height.ToString());
                    writer.WriteAttributeString("barcode", productVariant.Barcode);
                    writer.WriteAttributeString("available", productVariant.Available.ToString());
                    writer.WriteAttributeString("trackInventory", productVariant.TrackInventory.ToString());
                    writer.WriteAttributeString("outOfStockPurchase", productVariant.OutOfStockPurchase.ToString());
                    writer.WriteAttributeString("taxable", productVariant.Taxable.ToString());
                    writer.WriteAttributeString("shippable", productVariant.Shippable.ToString());
                    writer.WriteAttributeString("download", productVariant.Download.ToString());
                    writer.WriteAttributeString("downloadMediaId", productVariant.DownloadMediaId.ToString());
                    writer.WriteAttributeString("createDate", productVariant.CreateDate.ToString());
                    writer.WriteAttributeString("updateDate", productVariant.UpdateDate.ToString());
                    writer.WriteAttributeString("allDocs", "1");

                    

                    writer.WriteStartElement("warehouses");
                    foreach (var warehouse in productVariant.Warehouses)
                    {
                        writer.WriteStartElement("warehouse");
                        writer.WriteAttributeString("warehouseId", warehouse.WarehouseId.ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("count", warehouse.Count.ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("lowCount", warehouse.LowCount.ToString(CultureInfo.InvariantCulture));
                        writer.WriteEndElement(); // warehouse
                    }
                    writer.WriteEndElement(); // warehouse Inventory

                    writer.WriteEndElement(); // product variant
                    writer.WriteEndDocument();

                    xml = sw.ToString();
                }
            }

            return XDocument.Parse(xml); 
        }

        private static XDocument GetAttributesXml(IProductVariant productVariant)
        {

            string xml;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartDocument();
                    // attributes
                    writer.WriteStartElement("attributes");
                    foreach (var attribute in productVariant.Attributes)
                    {
                        writer.WriteStartElement("attribute");
                        writer.WriteAttributeString("id", attribute.Id.ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("optionId",
                                                    attribute.OptionId.ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("name", attribute.Name);
                        writer.WriteAttributeString("sku", attribute.Sku);
                        writer.WriteAttributeString("sortOrder",
                                                    attribute.SortOrder.ToString(CultureInfo.InvariantCulture));
                        writer.WriteEndElement(); // end attribute
                    }
                    writer.WriteEndElement(); // product attributes
                    writer.WriteEndDocument();

                    xml = sw.ToString();
                }
            }
            return XDocument.Parse(xml);
        }

        #endregion



    }
}