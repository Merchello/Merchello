using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Plugin.Shipping.UPS.Models;
using Umbraco.Core;

namespace Merchello.Plugin.Shipping.UPS
{
    public class UpsShippingProcessor
    {
        private readonly UPSProcessorSettings _settings;

        public UpsShippingProcessor(UPSProcessorSettings settings)
        {
            _settings = settings;
        }

        private NameValueCollection GetInitialRequestForm(string currencyCode)
        {
            return new NameValueCollection()
            {
                { "x_ups_access_key", _settings.UpsAccessKey },
                { "x_ups_username", _settings.UpsUserName },
                { "x_ups_password", _settings.UpsPassword },
            };
        }

        /// <summary>
        /// Gets the Authorize.Net Url
        /// </summary>
        private string GetUpsUrl()
        {
            return _settings.UseSandbox
                ? "https://onlinetools.ups.com/ups.app/xml/Rate"
                : "https://onlinetools.ups.com/ups.app/xml/Rate";
        }



        /// <summary>
        /// The Authorize.Net API version
        /// </summary>
        public static string ApiVersion
        {
            get { return "3.1"; }
        }

        public Attempt<IShipmentRateQuote> CalculatePrice(IShipment shipment, IShipMethod shipMethod, decimal totalWeight, IShipProvince province)
        {
            // First sum up the total weight for the shipment.
            // We're assumning that a custom order line property 
            // was set on the order line prior when the product was added to the order line.

            var shippingPrice = 0M;
            try
            {
                var http = new UpsHttpRequestHandler();
                var rateXml = http.Post(RateRequest(shipment, totalWeight));

                var collection = UpsParseRates(rateXml);

                var firstCarrierRate = collection.FirstOrDefault(option => option.Service == shipMethod.Name);
                if (firstCarrierRate != null)
                    shippingPrice = firstCarrierRate.Rate;
            }
            catch (Exception ex)
            {
                return Attempt<IShipmentRateQuote>.Fail(
                        new Exception("An error occured during your request : " +
                                                     ex.Message +
                                                     " Please contact your administrator or try again."));
            }

            return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, shipMethod) { Rate = shippingPrice });
        }

        private IEnumerable<DeliveryOption> UpsParseRates(string response)
        {
            var optionCollection = new DeliveryOptionCollection();

            var sr = new StringReader(response);
            var tr = new XmlTextReader(sr);

            try
            {
                while (tr.Read())
                {
                    if ((tr.Name == "Error") && (tr.NodeType == XmlNodeType.Element))
                    {
                        string errorText = "";
                        while (tr.Read())
                        {
                            if ((tr.Name == "ErrorCode") && (tr.NodeType == XmlNodeType.Element))
                            {
                                errorText += "UPS Rating Error, Error Code: " + tr.ReadString() + ", ";
                            }
                            if ((tr.Name == "ErrorDescription") && (tr.NodeType == XmlNodeType.Element))
                            {
                                errorText += "Error Desc: " + tr.ReadString();
                            }
                        }
                        throw new ProviderException("UPS Error returned: " + errorText);
                    }
                    if ((tr.Name == "RatedShipment") && (tr.NodeType == XmlNodeType.Element))
                    {
                        string serviceCode = "";
                        string monetaryValue = "";
                        while (tr.Read())
                        {
                            if ((tr.Name == "Service") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "Code") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        serviceCode = tr.ReadString();
                                        tr.ReadEndElement();
                                    }
                                    if ((tr.Name == "Service") && (tr.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }
                                }
                            }
                            if (((tr.Name == "RatedShipment") && (tr.NodeType == XmlNodeType.EndElement)) || ((tr.Name == "RatedPackage") && (tr.NodeType == XmlNodeType.Element)))
                            {
                                break;
                            }
                            if ((tr.Name == "TotalCharges") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "MonetaryValue") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        monetaryValue = tr.ReadString();
                                        tr.ReadEndElement();
                                    }
                                    if ((tr.Name == "TotalCharges") && (tr.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        var service = GetServiceName(serviceCode);
                        var rate = Convert.ToDecimal(monetaryValue);

                        // tack on additional handling charge for each bag
                        if (!String.IsNullOrEmpty(_settings.UpsAdditionalHandlingCharge))
                        {
                            decimal additionalHandling = Convert.ToDecimal(_settings.UpsAdditionalHandlingCharge);
                        }

                        //Weed out unwanted or unkown service rates
                        if (service.ToUpper() != "UNKNOWN" && IncludedService(serviceCode))
                        {
                            var option = new DeliveryOption { Rate = rate, Service = service };
                            optionCollection.Add(option);
                        }
                    }
                }
                sr.Dispose();
                return optionCollection;
            }
            catch (SystemException ex)
            {
                throw new SystemException(ex.Message);
            }
            finally
            {
                sr.Close();
                tr.Close();
            }
        }

        private static string GetServiceName(string serviceId)
        {
            switch (serviceId)
            {
                case "01":
                    return "UPS NextDay Air";
                case "02":
                    return "UPS 2nd Day Air";
                case "03":
                    return "UPS Ground";
                case "07":
                    return "UPS Worldwide Express";
                case "08":
                    return "UPS Worldwide Expidited";
                case "11":
                    return "UPS Standard";
                case "12":
                    return "UPS 3 Day Select";
                case "13":
                    return "UPS Next Day Air Saver";
                case "14":
                    return "UPS Next Day Air Early A.M.";
                case "54":
                    return "UPS Worldwide Express Plus";
                case "59":
                    return "UPS 2nd Day Air A.M.";
                default:
                    return "Unknown";
            }
        }


        //Create rating request XML string
        private string RateRequest(IShipment shipment, decimal totalWeight)
        {
            var sb = new StringBuilder();

            //Build Access Request
            sb.Append("<?xml version='1.0'?>");
            sb.Append("<AccessRequest xml:lang='en-US'>");
            sb.AppendFormat("<AccessLicenseNumber>{0}</AccessLicenseNumber>", _settings.UpsAccessKey);
            sb.AppendFormat("<UserId>{0}</UserId>", _settings.UpsUserName);
            sb.AppendFormat("<Password>{0}</Password>", _settings.UpsPassword);
            sb.Append("</AccessRequest>");

            //Build Rate Request
            sb.Append("<?xml version='1.0'?>");
            sb.Append("<RatingServiceSelectionRequest xml:lang='en-US'>");
            sb.Append("<Request>");
            sb.Append("<TransactionReference>");
            sb.Append("<CustomerContext>Bare Bones Rate Request</CustomerContext>");
            sb.Append("<XpciVersion>1.0001</XpciVersion>");
            sb.Append("</TransactionReference>");
            sb.Append("<RequestAction>Rate</RequestAction>");
            sb.Append("<RequestOption>Shop</RequestOption>");
            sb.Append("</Request>");
            sb.Append("<PickupType>");
            sb.AppendFormat("<Code>{0}</Code>", _settings.UpsPickupTypeCode);
            sb.Append("</PickupType>");
            sb.Append("<CustomerClassification>");
            sb.AppendFormat("<Code>{0}</Code>", _settings.UpsCustomerClassification);
            sb.Append("</CustomerClassification>");
            sb.Append("<Shipment>");
            sb.Append("<Shipper>");
            sb.Append("<ShipperNumber>834010</ShipperNumber>"); // Added for negotiated rates
            sb.Append("<Address>");
            sb.AppendFormat("<PostalCode>{0}</PostalCode>", shipment.FromPostalCode);
            sb.AppendFormat("<CountryCode>{0}</CountryCode>", shipment.FromCountryCode);
            sb.Append("</Address>");
            sb.Append("</Shipper>");
            sb.Append("<ShipTo>");
            sb.Append("<Address>");
            //Required to get accurate residential delivery rates
            sb.Append("<ResidentialAddressIndicator/>");
            sb.AppendFormat("<PostalCode>{0}</PostalCode>", shipment.ToPostalCode);
            sb.AppendFormat("<CountryCode>{0}</CountryCode>", shipment.ToAddress1);
            sb.AppendFormat("<StateProvinceCode>{0}</StateProvinceCode>", shipment.ToRegion);
            sb.Append("</Address>");
            sb.Append("</ShipTo>");
            sb.Append("<ShipFrom>");
            sb.Append("<Address>");
            sb.AppendFormat("<PostalCode>{0}</PostalCode>", shipment.FromPostalCode);
            sb.AppendFormat("<CountryCode>{0}</CountryCode>", shipment.FromCountryCode);
            sb.AppendFormat("<StateProvinceCode>{0}</StateProvinceCode>", shipment.FromRegion);
            sb.Append("</Address>");
            sb.Append("</ShipFrom>");
            sb.Append("<Service>");
            sb.Append("<Code>03</Code>");
            sb.Append("</Service>");

            //Changed weight logic per JeffreyABecker suggestions

            sb.Append("<Package>");
            sb.Append("<PackagingType>");
            sb.AppendFormat("<Code>{0}</Code>", _settings.UpsPackagingTypeCode);
            sb.Append("</PackagingType>");
            sb.Append("<Dimensions>");
            sb.Append("</Dimensions>");
            sb.Append("<PackageWeight>");
            sb.Append(string.Format("<Weight>{0}</Weight>", (int)Math.Ceiling(totalWeight)));
            sb.Append("</PackageWeight>");
            sb.Append("<PackageServiceOptions>");
            sb.Append("<InsuredValue>");
            sb.Append("<CurrencyCode>USD</CurrencyCode>");
            //sb.AppendFormat("<MonetaryValue>{0:##.00}</MonetaryValue>", adjOrderAmount);
            sb.Append("</InsuredValue>");
            sb.Append("</PackageServiceOptions>");
            sb.Append("</Package>");

            sb.Append("</Shipment>");
            sb.Append("</RatingServiceSelectionRequest>");

            return sb.ToString();
        }

        private bool IncludedService(string serviceCode)
        {
            //if (_settings.UpsExcludeService.IndexOf(serviceCode, 0, StringComparison.Ordinal) > 0)
            //    return false;
            //else
            return true;
        }
    }
}
