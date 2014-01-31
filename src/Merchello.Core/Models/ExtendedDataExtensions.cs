using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="ExtendedDataCollection"/>
    /// </summary>
    public static class ExtendedDataCollectionExtensions
    {

        #region ExtendedDataCollection

        public static void AddExtendedDataCollection(this ExtendedDataCollection extendedData, ExtendedDataCollection extendedDataToSerialize)
        {
            extendedData.SetValue(Constants.ExtendedDataKeys.ExtendedData, extendedDataToSerialize.SerializeToXml());
        }

        /// <summary>
        /// True/false indicating whether or not this extended data collection contains a child serialized extended data collection
        /// </summary>
        public static bool ContainsExtendedDataSerialization(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.ExtendedData);
        }

        /// <summary>
        /// Gets a <see cref="ExtendedDataCollection"/> from the <see cref="ExtendedDataCollection"/>
        /// </summary>
        public static ExtendedDataCollection GetExtendedDataCollection(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsExtendedDataSerialization()
                       ? new ExtendedDataCollection(extendedData.GetValue(Constants.ExtendedDataKeys.ExtendedData))
                       : null;
        }

        #endregion

        #region LineItemCollection

        /// <summary>
        /// Adds a <see cref="LineItemCollection"/> to the <see cref="ExtendedDataCollection"/>
        /// </summary>
        /// <param name="extendedData"></param>
        /// <param name="lineItemCollection"></param>
        public static void AddLineItemCollection(this ExtendedDataCollection extendedData, LineItemCollection lineItemCollection)
        {
         
            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings()
                    {
                        OmitXmlDeclaration = true
                    };
                
                using (var writer = XmlWriter.Create(sw, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(Constants.ExtendedDataKeys.LineItemCollection);

                    foreach (var lineItem in lineItemCollection)
                    {
                        //writer.WriteStartElement(Constants);
                        writer.WriteRaw(((LineItemBase)lineItem).SerializeToXml());
                    }

                    writer.WriteEndElement(); // ExtendedData
                    writer.WriteEndDocument();                  
                }
                extendedData.SetValue(Constants.ExtendedDataKeys.LineItemCollection, sw.ToString());
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="LineItemCollection"/> from a serialized collection in the ExtendedDataCollection
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ILineItem"/></typeparam>
        /// <param name="extendedData"></param>
        /// <returns><see cref="LineItemCollection"/></returns>
        public static LineItemCollection GetLineItemCollection<T>(this ExtendedDataCollection extendedData) where T : ILineItem
        {
            if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.LineItemCollection)) return null;

            var xdoc = XDocument.Parse(extendedData.GetValue(Constants.ExtendedDataKeys.LineItemCollection));
            var lineItemCollection = new LineItemCollection();
            foreach (var element in xdoc.Descendants(Constants.ExtendedDataKeys.LineItem))
            {

                var lineItem = new ItemCacheLineItem(element.ToString());

                lineItemCollection.Add(lineItem);
            }

            return lineItemCollection;
        }

        #endregion

        #region Product / ProductVariant


        public static void AddProductVariantValues(this ExtendedDataCollection extendedData, IProductVariant productVariant)
        {
            extendedData.SetValue(Constants.ExtendedDataKeys.ProductKey, productVariant.ProductKey.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.ProductVariantKey, productVariant.Key.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.CostOfGoods, productVariant.CostOfGoods.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Weight, productVariant.Weight.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Width, productVariant.Width.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Height, productVariant.Height.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Length, productVariant.Length.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Barcode, productVariant.Barcode);
            extendedData.SetValue(Constants.ExtendedDataKeys.Price, productVariant.Price.ToString(CultureInfo.InvariantCulture));
            extendedData.SetValue(Constants.ExtendedDataKeys.OnSale, productVariant.OnSale.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Manufacturer, productVariant.Manufacturer);
            extendedData.SetValue(Constants.ExtendedDataKeys.ManufacturerModelNumber, productVariant.ManufacturerModelNumber);
            extendedData.SetValue(Constants.ExtendedDataKeys.SalePrice, productVariant.SalePrice == null ? 0.ToString(CultureInfo.InvariantCulture) : productVariant.SalePrice.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.TrackInventory, productVariant.TrackInventory.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.OutOfStockPurchase, productVariant.OutOfStockPurchase.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Taxable, productVariant.Taxable.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Shippable, productVariant.Shippable.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.Download, productVariant.Download.ToString());
            extendedData.SetValue(Constants.ExtendedDataKeys.DownloadMediaId, productVariant.DownloadMediaId.ToString());
        }

        /// <summary>
        /// True/falce indicating whether or not this extended data collection contains information 
        /// which could define a <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static bool DefinesProductVariant(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsProductVariantKey() && extendedData.ContainsProductKey();
        }

        /// <summary>
        /// True/false indicating whether or not the colleciton contains a ProductVariantKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static bool ContainsProductVariantKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.ProductVariantKey);
        }       

        /// <summary>
        /// Return the ProductVariantKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static Guid GetProductVariantKey(this ExtendedDataCollection extendedData)
        {
            return GetGuidValue(extendedData.GetValue(Constants.ExtendedDataKeys.ProductVariantKey));
        }

        /// <summary>
        /// True/false indicating whether or not the colleciton contains a ProductKey
        /// </summary>
        public static bool ContainsProductKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.ProductKey);
        }

        /// <summary>
        /// Returns the ProductKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static Guid GetProductKey(this ExtendedDataCollection extendedData)
        {
            return GetGuidValue(extendedData.GetValue(Constants.ExtendedDataKeys.ProductKey));
        }

        /// <summary>
        /// Returns the "merchTaxable" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetTaxableValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue(Constants.ExtendedDataKeys.Taxable));
        }

        /// <summary>
        /// Returns the "merchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetTrackInventoryValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue(Constants.ExtendedDataKeys.TrackInventory));
        }

        /// <summary>
        /// Returns the "merchOutOfStockPurchase" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetOutOfStockPurchaseValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue(Constants.ExtendedDataKeys.OutOfStockPurchase));
        }

        /// <summary>
        /// Returns the "merchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetShippableValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue(Constants.ExtendedDataKeys.Shippable));
        }

        /// <summary>
        /// Returns the "merchDownload" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static bool GetDownloadValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue(Constants.ExtendedDataKeys.Download));
        }

        /// <summary>
        /// Returns the "merchTrackInventory" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>bool</returns>
        public static int GetDownloadMediaIdValue(this ExtendedDataCollection extendedData)
        {
            return GetIntegerValue(extendedData.GetValue(Constants.ExtendedDataKeys.DownloadMediaId));
        }

        /// <summary>
        /// Returns the "merchPrice" value
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static decimal GetPriceValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue(Constants.ExtendedDataKeys.Price));
        }

        /// <summary>
        /// Returns the "merchOnSale" value as a boolean
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static bool GetOnSaleValue(this ExtendedDataCollection extendedData)
        {
            return GetBooleanValue(extendedData.GetValue(Constants.ExtendedDataKeys.OnSale));
        }

        /// <summary>
        /// Returns the "merchSalePrice" value
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static decimal GetSalePriceValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue(Constants.ExtendedDataKeys.SalePrice));
        }

        /// <summary>
        /// Returns the "merchManufacturer" value
        /// </summary>
        public static string GetManufacturerValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Manufacturer);
        }

        /// <summary>
        /// Returns the "merchManufacturerModelNumber" value
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static string GetManufacturerModelNumberValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.ManufacturerModelNumber);
        }

        /// <summary>
        /// returns the "merchWeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetWeightValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue(Constants.ExtendedDataKeys.Weight));
        }

        /// <summary>
        /// returns the "merchHeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetHeightValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue(Constants.ExtendedDataKeys.Height));
        }

        /// <summary>
        /// returns the "merchWidth" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetWidthValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue(Constants.ExtendedDataKeys.Width));
        }

        /// <summary>
        /// returns the "merchLength" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static decimal GetLengthValue(this ExtendedDataCollection extendedData)
        {
            return GetDecimalValue(extendedData.GetValue(Constants.ExtendedDataKeys.Length));
        }

        /// <summary>
        /// returns the "merchWeight" value
        /// </summary>
        /// <param name="extendedData"><see cref="ExtendedDataCollection"/></param>
        /// <returns>decimal</returns>
        public static string GetBarcodeValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Barcode);
        }

        #endregion

        #region IShipment

        /// <summary>
        /// True/false indicating whether or not the colleciton contains a WarehouseCatalogKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static bool ContainsWarehouseCatalogKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.WarehouseCatalogKey);
        }

        /// <summary>
        /// Return the WarehouseCatalogKey
        /// </summary>
        /// <param name="extendedData"></param>
        /// <returns></returns>
        public static Guid GetWarehouseCatalogKey(this ExtendedDataCollection extendedData)
        {
            return GetGuidValue(extendedData.GetValue(Constants.ExtendedDataKeys.WarehouseCatalogKey));
        }

        #endregion


        #region Utility


        private static Guid GetGuidValue(string value)
        {
            Guid converted;
            return Guid.TryParse(value, out converted) ? converted : Guid.Empty;
        }

        private static bool GetBooleanValue(string value)
        {
            bool converted;
            return bool.TryParse(value, out converted) && converted;
        }

        private static decimal GetDecimalValue(string value)
        {
            decimal converted;
            return decimal.TryParse(value, out converted) ? converted : 0;
        }

        private static int GetIntegerValue(string value)
        {
            int converted;
            return int.TryParse(value, out converted) ? converted : 0;
        }

        #endregion
    }
}