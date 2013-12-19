using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Merchello.Core.Models.Interfaces;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

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
                options.AddRange(product.ProductOptions.Where(option => option.Choices.Any(choice => choice.Key == att.Key)));
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
                                          productAttributes.All(item => ((ProductAttributeCollection)variant.Attributes).Contains(item.Key));
                    });

        }

        /// <summary>
        /// Returns the <see cref="IProductVariant"/> of this <see cref="IProduct"/> that contains a matching collection of <see cref="IProductAttribute" />. 
        /// If not match is found, returns null.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="selectedChoiceKeys"></param>
        /// <returns><see cref="IProductVariant"/> or null if no <see cref="IProductVariant"/> is found with a matching collection of <see cref="IProductAttribute"/></returns>
        public static IProductVariant GetProductVariantForPurchase(this IProduct product, Guid[] selectedChoiceKeys)
        {
            return
                product.ProductVariants.FirstOrDefault(
                    variant => variant.Attributes.Count() == selectedChoiceKeys.Length &&
                               selectedChoiceKeys.All(key => ((ProductAttributeCollection)variant.Attributes).Contains(key)));
        }

        #endregion


        #region IProductVariant Collections

        /// <summary>
        /// Associates a product with a warehouse catalog
        /// </summary>
        /// <param name="product"></param>
        /// <param name="catalog"><see cref="IWarehouseCatalog"/></param>
        internal static void AddToWarehouseCatalog(this IProduct product, IWarehouseCatalog catalog)
        {
            ((Product)product).MasterVariant.AddToWarehouseCatalog(catalog);
        }

       

        /// <summary>
        /// Associates a product variant with a warehouse
        /// </summary>
        /// <param name="productVariant"></param>
        /// <param name="catalog"><see cref="IWarehouseCatalog"/></param>
        internal static void AddToWarehouseCatalog(this IProductVariant productVariant, IWarehouseCatalog catalog)
        {
            ((WarehouseInventoryCollection)productVariant.Warehouses).Add(new WarehouseInventory(catalog, productVariant.Key));
        }


        #endregion

        #region Examine Serialization

        /// <summary>
        /// Serializes <see cref="IProduct"/> object's variants
        /// </summary>
        /// <remarks>
        /// Intended to be used by the Merchello.Examine.Providers.MerchelloProductIndexer
        /// </remarks>
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
                
            doc.Root.Add(((Product)product).MasterVariant.SerializeToXml(product.ProductOptions).Root);
            
            foreach (var variant in product.ProductVariants)
            {
                doc.Root.Add(variant.SerializeToXml().Root);
            }
            return doc;
        }


        public static XDocument SerializeToXml(this IProductVariant productVariant, ProductOptionCollection productOptions = null)
        {
            string xml;
            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("productVariant");
                   // TODO construct the id
                    writer.WriteAttributeString("id", ((ProductVariant)productVariant).ExamineId.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("productKey", productVariant.ProductKey.ToString());
                    writer.WriteAttributeString("productVariantKey", productVariant.Key.ToString());
                    writer.WriteAttributeString("master", ((ProductVariant)productVariant).Master.ToString());
                    writer.WriteAttributeString("name", productVariant.Name);
                    writer.WriteAttributeString("sku", productVariant.Sku);
                    writer.WriteAttributeString("price", productVariant.Price.ToString(CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("manufacturer", productVariant.Manufacturer);
                    writer.WriteAttributeString("modelNumber", productVariant.ManufacturerModelNumber);
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
                    writer.WriteAttributeString("totalInventoryCount", productVariant.TotalInventoryCount.ToString());
                    writer.WriteAttributeString("attributes", GetAttributesJson(productVariant));
                    writer.WriteAttributeString("warehouses", GetWarehousesJson(productVariant));

                    if(productOptions != null) writer.WriteAttributeString("options", GetProductOptionsJson(productOptions));

                    writer.WriteAttributeString("createDate", productVariant.CreateDate.ToString("s"));
                    writer.WriteAttributeString("updateDate", productVariant.UpdateDate.ToString("s"));                    
                    writer.WriteAttributeString("allDocs", "1");
                                        
                    writer.WriteEndElement(); // product variant
                    writer.WriteEndDocument();

                    xml = sw.ToString();
                }
            }

            return XDocument.Parse(xml); 
        }

        private static string GetProductOptionsJson(IEnumerable<IProductOption> productOptions)
        {
            var json = "[{0}]";
            var options = "";

            foreach (var option in productOptions)
            {
                var optionChoices = new List<object>();                
                foreach (var choice in option.Choices)
                {
                    optionChoices.Add(
                            new
                            {
                                attributeKey = choice.Key,
                                optionKey = choice.OptionKey,
                                name = choice.Name,
                                sortOrder = choice.SortOrder
                            }
                        );
                }
                if (options.Length > 0) options += ",";
                options += JsonConvert.SerializeObject(
                        new
                        {
                            optionKey = option.Key,
                            name = option.Name,
                            required = option.Required,
                            sortOrder = option.SortOrder,
                            choices = optionChoices
                        }
                    );
            }
            json = string.Format(json, options);
            return json;
        }

        private static string GetWarehousesJson(IProductVariant productVariant)
        {
            var json = "[{0}]";
            var warehouses = "";

            foreach (var wh in productVariant.Warehouses)
            {
                if (warehouses.Length > 0) warehouses += ",";
                warehouses += JsonConvert.SerializeObject(
                new
                {   catalogKey = ((WarehouseInventory)wh).CatalogKey,
                    warehouseKey = wh.WarehouseKey,
                    productVariantKey = wh.ProductVariantKey,
                    count = wh.Count,
                    lowCount = wh.LowCount
                },
                Formatting.None);
            }
            json = string.Format(json, warehouses);
            return json;
        }

        private static string GetAttributesJson(IProductVariant productVariant)
        {
            var json = "[{0}]";
            var atts = "";

            foreach (var attribute in productVariant.Attributes)
            {
                if (atts.Length > 0) atts += ",";
                atts += JsonConvert.SerializeObject(
                new 
                { 
                    optionId = attribute.OptionKey,
                    name = attribute.Name,
                    sku = attribute.Sku,
                    sortOrder = attribute.SortOrder                    
                }, 
                Formatting.None);
            }
            json = string.Format(json, atts);
            return json;
        }

        #endregion
    }
}