namespace Merchello.Plugin.Taxation.Avalara
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Plugin.Taxation.Avalara.Models;
    using Merchello.Plugin.Taxation.Avalara.Models.Address;
    using Merchello.Plugin.Taxation.Avalara.Models.Tax;

    using Newtonsoft.Json;

    /// <summary>
    /// Mapping extensions to assist in mapping objects types
    /// </summary>
    public static class MappingExtensions
    {
        #region ExtendedData

        /// <summary>
        /// Serializes the <see cref="AvaTaxProviderSettings"/> and saves them in the extend data collection.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public static void SaveProviderSettings(this ExtendedDataCollection extendedData, AvaTaxProviderSettings settings)
        {
            extendedData.SetValue(AvaTaxProviderSettings.ExtendedDataKey, JsonConvert.SerializeObject(settings)); 
        }

        /// <summary>
        /// Deserializes ava tax provider settings from the gateway provider's extended data collection
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// The <see cref="AvaTaxProviderSettings"/>.
        /// </returns>
        public static AvaTaxProviderSettings GetAvaTaxProviderSettings(this ExtendedDataCollection extendedData)
        {
            return JsonConvert.DeserializeObject<AvaTaxProviderSettings>(extendedData.GetValue(AvaTaxProviderSettings.ExtendedDataKey));
        }

        #endregion

        #region TaxAddress

        /// <summary>
        /// Maps a <see cref="IAddress"/> to a <see cref="TaxAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="TaxAddress"/>.
        /// </returns>
        public static ITaxAddress ToTaxAddress(this IAddress address)
        {
            return address.ToValidatableAddress().ToTaxAddress();
        }

        /// <summary>
        /// Maps <see cref="IShipment"/> origin and destination addresses to an array of <see cref="TaxAddress"/> for the GetTax request.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="startAddressIndex">
        /// The start address index.
        /// </param>
        /// <returns>
        /// The <see cref="TaxAddress[]"/>.
        /// </returns>
        public static TaxAddress[] GetTaxAddressArray(this IShipment shipment, int startAddressIndex = 1)
        {
            var origin = shipment.GetOriginAddress().ToTaxAddress() as TaxAddress;

            origin.AddressCode = startAddressIndex.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');

            var destination = shipment.GetDestinationAddress().ToTaxAddress() as TaxAddress;

            destination.AddressCode = (startAddressIndex + 1).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0');

            return new[] { origin, destination };
        }

        #endregion

        #region TaxRequest

        /// <summary>
        /// Maps an <see cref="IInvoice"/> to tax request.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="defaultStoreAddress">
        /// The default store address.  This is required for items without shipping information.
        /// </param>
        /// <param name="estimateOnly">
        /// Indicates if the quote should be an estimate or recorded
        /// </param>
        /// <returns>
        /// The <see cref="TaxRequest"/>.
        /// </returns>
        public static TaxRequest AsTaxRequest(this IInvoice invoice, ITaxAddress defaultStoreAddress, bool estimateOnly = true)
        {
            var addresses = new List<TaxAddress>();
            var lines = new List<StatementLineItem>();

            defaultStoreAddress.AddressCode = "1";
            addresses.Add(defaultStoreAddress as TaxAddress);

            var shippingItems = invoice.ShippingLineItems().ToArray();
            if (shippingItems.Any())
            {
                var counter = 1;
                var lineNo = 1;

                foreach (var shipLine in shippingItems)
                {
                    var shipment = shipLine.ExtendedData.GetShipment<OrderLineItem>();
                    var shipmentAddresses = shipment.GetTaxAddressArray(counter + 1);

                    foreach (var line in shipment.Items)
                    {
                        lines.Add(
                                new StatementLineItem()
                                    {
                                        LineNo = lineNo.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                                        ItemCode = line.Sku,
                                        Amount = line.TotalPrice,
                                        Qty = line.Quantity,
                                        Description = line.Name,
                                        OriginCode = shipmentAddresses[0].AddressCode,
                                        DestinationCode = shipmentAddresses[1].AddressCode
                                    });
                        lineNo++;
                    }

                    lines.Add(new StatementLineItem()
                        {
                            LineNo = string.Format("{0}-FR", lineNo.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')),
                            Amount = shipLine.Price,
                            ItemCode = shipLine.Sku,
                            Qty = shipLine.Quantity,
                            Description = shipLine.Name,
                            OriginCode = shipmentAddresses[0].AddressCode,
                            DestinationCode = shipmentAddresses[1].AddressCode,
                            TaxCode = "FR020100" // TODO this should probably not be hard coded here
                        });

                    addresses.AddRange(shipmentAddresses);
                    counter++;
                }
            }

            // add items not included in the shipment
            var notShipped =
                invoice.Items.Where(
                    x =>
                    x.LineItemType != LineItemType.Shipping &&
                    x.LineItemType != LineItemType.Discount &&
                    !lines.Any(line => line.ItemCode.Contains(x.Sku)));

            // TODO add lines for non shippable - like downloadable and discounts


            var taxRequest = new TaxRequest(estimateOnly ? StatementType.SalesOrder : StatementType.SalesInvoice)
                {
                    DocCode = invoice.PrefixedInvoiceNumber(),
                    Addresses = addresses.ToArray(),
                    DocDate = invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                    Lines = lines
                };

            return taxRequest;
        }

        #endregion

        #region ValidatableAddress

        /// <summary>
        /// Maps a <see cref="IValidatableAddress"/> to an API usable query string.
        /// </summary>
        /// <param name="taxAddress">
        /// The tax address.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string AsApiQueryString(this IValidatableAddress taxAddress)
        {
            return string.Format(
                "{0}&{1}&{2}&{3}&{4}&{5}&{6}",
                GetQsValue("Line1", taxAddress.Line1),
                GetQsValue("Line2", taxAddress.Line2),
                GetQsValue("Line3", taxAddress.Line3),
                GetQsValue("City", taxAddress.City),
                GetQsValue("Region", taxAddress.Region),
                GetQsValue("PostalCode", taxAddress.PostalCode),
                GetQsValue("Country", taxAddress.Country));
        }

        /// <summary>
        /// Maps a <see cref="IAddress"/> to a <see cref="IValidatableAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="IValidatableAddress"/>.
        /// </returns>
        public static IValidatableAddress ToValidatableAddress(this IAddress address)
        {
            return new ValidatableAddress()
            {
                Line1 = address.Address1,
                Line2 = address.Address2,
                City = address.Locality,
                Region = address.Region,
                PostalCode = address.PostalCode,
                Country = address.CountryCode
            };
        }

        /// <summary>
        /// Maps a <see cref="IValidatableAddress"/> to an <see cref="IAddress"/>.
        /// </summary>
        /// <param name="validatableAddress">
        /// The verifiable address.
        /// </param>
        /// <returns>
        /// The <see cref="IAddress"/>.
        /// </returns>
        public static IAddress ToAddress(this IValidatableAddress validatableAddress)
        {
            return new Address()
            {
                Address1 = validatableAddress.Line1,
                Address2 = string.Format("{0} {1}", validatableAddress.Line2, validatableAddress.Line3).Trim(),
                Locality = validatableAddress.City,
                Region = validatableAddress.Region,
                PostalCode = validatableAddress.PostalCode,
                CountryCode = validatableAddress.Country
            };
        }

        /// <summary>
        /// Maps a <see cref="IValidatableAddress"/> to a <see cref="ITaxAddress"/>.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxAddress"/>.
        /// </returns>
        public static ITaxAddress ToTaxAddress(this IValidatableAddress address)
        {
            return AutoMapper.Mapper.Map<TaxAddress>(address);
        }

        /// <summary>
        /// Utility method to safely encode query string parameters.
        /// </summary>
        /// <param name="prop">
        /// The prop.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetQsValue(string prop, string value)
        {
            return string.Format("{0}={1}", prop, HttpUtility.UrlEncode(value));
        }

        #endregion
    }
}