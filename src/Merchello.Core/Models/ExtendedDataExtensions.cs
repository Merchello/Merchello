namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Xml;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Extension methods for <see cref="ExtendedDataCollection"/>
    /// </summary>
   [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    public static class ExtendedDataCollectionExtensions
    {
        #region ExtendedDataCollection

        /// <summary>
        /// The add extended data collection.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="extendedDataToSerialize">
        /// The extended data to serialize.
        /// </param>
        public static void AddExtendedDataCollection(this ExtendedDataCollection extendedData, ExtendedDataCollection extendedDataToSerialize)
        {
            extendedData.SetValue(Constants.ExtendedDataKeys.ExtendedData, extendedDataToSerialize.SerializeToXml());
        }

        /// <summary>
        /// True/false indicating whether or not this extended data collection contains a child serialized extended data collection
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// A value indicating whether or not this extended data collection contains a child serialized extended data collection
        /// </returns>
        public static bool ContainsExtendedDataSerialization(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.ExtendedData);
        }

        /// <summary>
        /// Gets a <see cref="ExtendedDataCollection"/> from the <see cref="ExtendedDataCollection"/>
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The <see cref="ExtendedDataCollection"/>.
        /// </returns>
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
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="lineItemCollection">
        /// The line Item Collection.
        /// </param>
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
                        ////writer.WriteStartElement(Constants);
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
        /// <param name="extendedData">The extended data collection</param>
        /// <returns><see cref="LineItemCollection"/></returns>
        public static LineItemCollection GetLineItemCollection<T>(this ExtendedDataCollection extendedData) where T : LineItemBase
        {
            if (!extendedData.ContainsKey(Constants.ExtendedDataKeys.LineItemCollection)) return null;

            var xdoc = XDocument.Parse(extendedData.GetValue(Constants.ExtendedDataKeys.LineItemCollection));
            var lineItemCollection = new LineItemCollection();
            foreach (var element in xdoc.Descendants(Constants.ExtendedDataKeys.LineItem))
            {
                
                var dictionary = GetLineItemXmlValues(element.ToString());

                var extendData = new ExtendedDataCollection(dictionary[Constants.ExtendedDataKeys.ExtendedData]);

                var ctrValues = new object[]
                    {                        
                        new Guid(dictionary[Constants.ExtendedDataKeys.LineItemTfKey]),
                        dictionary[Constants.ExtendedDataKeys.Name],
                        dictionary[Constants.ExtendedDataKeys.Sku],
                        int.Parse(dictionary[Constants.ExtendedDataKeys.Quantity]),
                        decimal.Parse(dictionary[Constants.ExtendedDataKeys.Price]),
                        new ExtendedDataCollection(dictionary[Constants.ExtendedDataKeys.ExtendedData])
                    };
                               
                var attempt = ActivatorHelper.CreateInstance<LineItemBase>(typeof(T).FullName, ctrValues);

                if (!attempt.Success)
                {
                    LogHelper.Error<LineItemCollection>("Failed to instantiate a LineItemCollection from ExtendedData", attempt.Exception);
                    throw attempt.Exception;
                }
                
                attempt.Result.ContainerKey = new Guid(dictionary[Constants.ExtendedDataKeys.ContainerKey]);

                lineItemCollection.Add(attempt.Result);
            }

            return lineItemCollection;
        }

        /// <summary>
        /// Helper method to parse Xml document
        /// </summary>
        /// <param name="lineItemXml">
        /// The line Item Xml.
        /// </param>
        /// <returns>
        /// A dictionary of line items
        /// </returns>
        private static IDictionary<string, string> GetLineItemXmlValues(string lineItemXml)
        {
            var xdoc = XDocument.Parse(lineItemXml);

            var dictionary = new Dictionary<string, string>
            {
                { Constants.ExtendedDataKeys.ContainerKey, GetXmlValue(xdoc,Constants.ExtendedDataKeys.ContainerKey) },
                { Constants.ExtendedDataKeys.LineItemTfKey, GetXmlValue(xdoc,Constants.ExtendedDataKeys.LineItemTfKey) },
                { Constants.ExtendedDataKeys.Sku, GetXmlValue(xdoc,Constants.ExtendedDataKeys.Sku) },
                { Constants.ExtendedDataKeys.Name, GetXmlValue(xdoc,Constants.ExtendedDataKeys.Name) },
                { Constants.ExtendedDataKeys.Quantity, GetXmlValue(xdoc,Constants.ExtendedDataKeys.Quantity) },
                { Constants.ExtendedDataKeys.Price, GetXmlValue(xdoc,Constants.ExtendedDataKeys.Price) },
                { Constants.ExtendedDataKeys.ExtendedData, GetXmlValue(xdoc,Constants.ExtendedDataKeys.ExtendedData) }
            };

            return dictionary;
        }

        #endregion

        #region Product / ProductVariant

        /// <summary>
        /// The add product variant values.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="productVariant">
        /// The product variant.
        /// </param>
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
        /// True/false indicating whether or not this extended data collection contains information 
        /// which could define a <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool DefinesProductVariant(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsProductVariantKey() && extendedData.ContainsProductKey();
        }

        /// <summary>
        /// True/false indicating whether or not the collection contains a ProductVariantKey
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the extended data collection contains a product variant key.
        /// </returns>
        public static bool ContainsProductVariantKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.ProductVariantKey);
        }       

        /// <summary>
        /// Return the ProductVariantKey
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The product variant key.
        /// </returns>
        public static Guid GetProductVariantKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.ProductVariantKey).AsGuid();
        }

        /// <summary>
        /// True/false indicating whether or not the collection contains a ProductKey
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the extended data collection contains a product key.
        /// </returns>
        public static bool ContainsProductKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.ProductKey);
        }

        /// <summary>
        /// Returns the ProductKey
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The product key.
        /// </returns>
        public static Guid GetProductKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.ProductKey).AsGuid();
        }

        /// <summary>
        /// Returns the "merchTaxable" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>A value indicating whether or not the line item is taxable</returns>
        public static bool GetTaxableValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Taxable).AsBool();
        }

        /// <summary>
        /// Returns the "merchTrackInventory" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>A value indicating whether or not the line item (product) should be included in inventory operations</returns>
        public static bool GetTrackInventoryValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.TrackInventory).AsBool();
        }

        /// <summary>
        /// Returns the "merchOutOfStockPurchase" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>A value indicating whether or not the item can be purchased even if it's out of stock</returns>
        public static bool GetOutOfStockPurchaseValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.OutOfStockPurchase).AsBool();
        }

        /// <summary>
        /// Returns the "merchShippable" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>A value indicating whether or not the item is shippable</returns>
        public static bool GetShippableValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Shippable).AsBool();
        }

        /// <summary>
        /// Returns the "merchDownload" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>A value indicating whether or not the item is downloadable</returns>
        public static bool GetDownloadValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Download).AsBool();
        }

        /// <summary>
        /// Returns the "merchTrackInventory" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The Umbraco media Id</returns>
        public static int GetDownloadMediaIdValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.DownloadMediaId).AsInt();
        }

        /// <summary>
        /// Returns the "merchPrice" value
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The price.
        /// </returns>
        public static decimal GetPriceValue(this ExtendedDataCollection extendedData)
        {
            decimal converted = decimal.TryParse(extendedData.GetValue(Constants.ExtendedDataKeys.Price), System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out converted) ? converted : 0;
            return converted;
        }

        /// <summary>
        /// Returns the "merchOnSale" value as a boolean
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the item is on sale.
        /// </returns>
        public static bool GetOnSaleValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.OnSale).AsBool();
        }

        /// <summary>
        /// Returns the "merchSalePrice" value
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The sales price
        /// </returns>
        public static decimal GetSalePriceValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.SalePrice).AsDecimal();
        }

        /// <summary>
        /// Returns the "merchManufacturer" value
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The manufacturer.
        /// </returns>
        public static string GetManufacturerValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Manufacturer);
        }

        /// <summary>
        /// Returns the "merchManufacturerModelNumber" value
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The manufacturers model number
        /// </returns>
        public static string GetManufacturerModelNumberValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.ManufacturerModelNumber);
        }

        /// <summary>
        /// returns the "merchWeight" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The weight</returns>
        public static decimal GetWeightValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Weight).AsDecimal();
        }

        /// <summary>
        /// returns the "merchHeight" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The height</returns>
        public static decimal GetHeightValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Height).AsDecimal();
        }

        /// <summary>
        /// returns the "merchWidth" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The width</returns>
        public static decimal GetWidthValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Width).AsDecimal();
        }

        /// <summary>
        /// returns the "merchLength" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The length</returns>
        public static decimal GetLengthValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Length).AsDecimal();
        }

        /// <summary>
        /// returns the "merchWeight" value
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The barcode</returns>
        public static string GetBarcodeValue(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.Barcode);
        }

        #endregion

        #region IAddress

        /// <summary>
        /// Adds an <see cref="IAddress"/> to extended data.  This is intended for shipment addresses
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="addressType">
        /// The Origin or Destination addresses
        /// </param>
        public static void AddAddress(this ExtendedDataCollection extendedData, IAddress address, AddressType addressType)
        {            
            extendedData.AddAddress(
                            address, 
                            addressType == AddressType.Shipping ? Constants.ExtendedDataKeys.ShippingDestinationAddress : Constants.ExtendedDataKeys.BillingAddress);
        }

        /// <summary>
        /// Adds an <see cref="IAddress"/> to extended data.  This is intended for shipment addresses
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="address">
        /// The <see cref="IAddress"/> to be added to extended data
        /// </param>
        /// <param name="dictionaryKey">
        /// The dictionary key used to reference the serialized <see cref="IAddress"/>
        /// </param>
        public static void AddAddress(this ExtendedDataCollection extendedData, IAddress address, string dictionaryKey)
        {
            var addressXml = SerializationHelper.SerializeToXml(address as Address);

            ////var addressJson = JsonConvert.SerializeObject(address);

            extendedData.SetValue(dictionaryKey, addressXml);
        }

        /// <summary>
        /// Gets an <see cref="IAddress"/> from the <see cref="ExtendedDataCollection"/>
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="addressType">
        /// The address Type.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress GetAddress(this ExtendedDataCollection extendedData, AddressType addressType)
        {
            return extendedData.GetAddress(addressType == AddressType.Shipping
                                               ? Constants.ExtendedDataKeys.ShippingDestinationAddress
                                               : Constants.ExtendedDataKeys.BillingAddress);
        }

        /// <summary>
        /// Gets an <see cref="IAddress"/> from the <see cref="ExtendedDataCollection"/>
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="dictionaryKey">
        /// The dictionary Key.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress GetAddress(this ExtendedDataCollection extendedData, string dictionaryKey)
        {
            if (!extendedData.ContainsKey(dictionaryKey)) return null;

            var attempt = SerializationHelper.DeserializeXml<Address>(extendedData.GetValue(dictionaryKey));

            return attempt.Success ? attempt.Result : null;
            ////return JsonConvert.DeserializeObject<IAddress>(extendedData.GetValue(dictionaryKey));
        }

        #endregion

        #region IShipment


        /// <summary>
        /// Adds a <see cref="IShipment"/> to the extended data collection
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        public static void AddShipment(this ExtendedDataCollection extendedData, IShipment shipment)
        {
            extendedData.AddAddress(shipment.GetOriginAddress(), Constants.ExtendedDataKeys.ShippingOriginAddress);
            extendedData.AddAddress(shipment.GetDestinationAddress(), Constants.ExtendedDataKeys.ShippingDestinationAddress);
            extendedData.SetValue(Constants.ExtendedDataKeys.ShipMethodKey, shipment.ShipMethodKey.ToString());
            extendedData.AddLineItemCollection(shipment.Items);
        }

        /// <summary>
        /// Gets (creates a new <see cref="IShipment"/>) from values saved in the <see cref="ExtendedDataCollection"/>
        /// </summary>
        /// <typeparam name="T">
        /// The type of line item
        /// </typeparam>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The <see cref="IShipment"/>.
        /// </returns>
        public static IShipment GetShipment<T>(this ExtendedDataCollection extendedData) where T : LineItemBase
        {
            var origin = extendedData.GetAddress(Constants.ExtendedDataKeys.ShippingOriginAddress);
            var destination = extendedData.GetAddress(Constants.ExtendedDataKeys.ShippingDestinationAddress);
            var lineItemCollection = extendedData.GetLineItemCollection<T>();

            if (origin == null) throw new NullReferenceException("ExtendedDataCollection does not contain an 'origin shipping address'");
            if (destination == null) throw new NullReferenceException("ExtendedDataCollection does not container a 'destination shipping address'");
            if (lineItemCollection == null) throw new NullReferenceException("ExtendedDataCollection does not contain a 'line item collection'");

            // TODO - this is a total hack since this value can be changed in the database
            var quoted = new ShipmentStatus()
            {
                Key = Constants.DefaultKeys.ShipmentStatus.Quoted,
                Alias = "quoted",
                Name = "Quoted",
                Active = true,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };


            return new Shipment(quoted, origin, destination, lineItemCollection)
                {
                    ShipMethodKey = extendedData.ContainsKey(Constants.ExtendedDataKeys.ShipMethodKey) ?
                        extendedData.GetShipMethodKey() :
                        Guid.Empty
                };
        }

        /// <summary>
        /// True/false indicating whether or not the collection contains a ShipmentKey
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the extended data collection contains a shipment key.
        /// </returns>
        public static bool ContainsShipmentKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.ShipmentKey);
        }

        /// <summary>
        /// Returns the merchShipmentKey value as a GUID
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The GUID based 'Key' of the <see cref="IShipment"/>
        /// </returns>
        public static Guid GetShipmentKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.ShipmentKey).AsGuid();
        }

        /// <summary>
        /// Returns the merchShipMethodKey value as a GUID
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The GUID based 'Key' of the <see cref="IShipMethod"/>
        /// </returns>
        public static Guid GetShipMethodKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.ShipMethodKey).AsGuid();
        }

        /// <summary>
        /// True/false indicating whether or not the collection contains a WarehouseCatalogKey
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the extended data collection contains a warehouse key.
        /// </returns>
        public static bool ContainsWarehouseCatalogKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Constants.ExtendedDataKeys.WarehouseCatalogKey);
        }

        /// <summary>
        /// Return the WarehouseCatalogKey
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The GUID warehouse catalog key.
        /// </returns>
        public static Guid GetWarehouseCatalogKey(this ExtendedDataCollection extendedData)
        {
            return extendedData.GetValue(Constants.ExtendedDataKeys.WarehouseCatalogKey).AsGuid();
        }

        #endregion

        #region IPaymentMethod

        /// <summary>
        /// Saves a <see cref="IPaymentMethod"/> to an extended data collection
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <param name="paymentMethod">
        /// The payment Method.
        /// </param>
        internal static void AddPaymentMethod(this ExtendedDataCollection extendedData, IPaymentMethod paymentMethod)
        {         
            extendedData.SetValue(Constants.ExtendedDataKeys.PaymentMethod, paymentMethod.Key.ToString());
        }

        /// <summary>
        /// Gets a <see cref="IPaymentMethod"/> from the <see cref="ExtendedDataCollection"/>
        /// </summary>
        /// <param name="extendedData">
        /// The extended Data.
        /// </param>
        /// <returns>
        /// The payment method key.
        /// </returns>
        internal static Guid GetPaymentMethodKey(this ExtendedDataCollection extendedData)
        {
            return !extendedData.ContainsKey(Constants.ExtendedDataKeys.PaymentMethod) ? Guid.Empty : 
                extendedData.GetValue(Constants.ExtendedDataKeys.PaymentMethod).AsGuid();
        }

        #endregion

        #region AutoMapper and Serialization

        /// <summary>
        /// Converts extended data into a more easily serializable collection for display classes (back office UI)
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>An <c>IEnumerable{object}</c></returns>        
        internal static IEnumerable<KeyValuePair<string, string>> AsEnumerable(this ExtendedDataCollection extendedData)
        {
            return extendedData.Select(item => new KeyValuePair<string, string>(item.Key, item.Value));
        }

        /// <summary>
        /// The as extended data collection.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="ExtendedDataCollection"/>.
        /// </returns>
        internal static ExtendedDataCollection AsExtendedDataCollection(this IEnumerable<KeyValuePair<string, string>> source)
        {
            var ed = new ExtendedDataCollection();
            foreach (var item in source.ToArray())
            {                
                ed.SetValue(item.Key, item.Value);
            }

            return ed;
        }

        /// <summary>
        /// Serializes the extended data collection to an JSON representation
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ExtendedDataAsJson(this IHasExtendedData entity)
        {
            return JsonConvert.SerializeObject(entity.ExtendedData.AsEnumerable());
        }

        #endregion

        #region Utility

        /// <summary>
        /// Gets the value as a GUID.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        private static Guid AsGuid(this string value)
        {
            Guid converted;
            return Guid.TryParse(value, out converted) ? converted : Guid.Empty;
        }

        /// <summary>
        /// Gets the.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool AsBool(this string value)
        {
            bool converted;
            return bool.TryParse(value, out converted) && converted;
        }

        /// <summary>
        /// The get decimal value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        private static decimal AsDecimal(this string value)
        {
            decimal converted;
            return decimal.TryParse(value, out converted) ? converted : 0;
        }

        /// <summary>
        /// The get integer value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int AsInt(this string value)
        {
            int converted;
            return int.TryParse(value, out converted) ? converted : 0;
        }

        /// <summary>
        /// The get xml value.
        /// </summary>
        /// <param name="xdoc">
        /// The xdoc.
        /// </param>
        /// <param name="elementName">
        /// The element name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws if the element name does not exist in the Xml document
        /// </exception>
        private static string GetXmlValue(XDocument xdoc, string elementName)
        {
            var element = xdoc.Descendants(elementName).FirstOrDefault();
            if (element == null) throw new NullReferenceException(elementName);

            return element.ToString().StartsWith("<" + Constants.ExtendedDataKeys.ExtendedData + ">") ? element.ToString() : element.Value;
        }

        #endregion
    }
}
