using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Plugin.Shipping.UPS.Models;
using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Merchello.Plugin.Shipping.UPS.Provider
{
    [GatewayMethodEditor("UPS Shipping Method Editor", "~/App_Plugins/Merchello.UPS/shippingmethod.html")]
    public class UPSShippingGatewayMethod : ShippingGatewayMethodBase, IUPSShippingGatewayMethod
    {
        private UPSProcessorSettings _processorSettings;
        private UPSType _upsType;
        private IShipMethod _shipMethod;
        private IGatewayProviderSettings _gatewayProviderSettings;
        private IRuntimeCacheProvider _runtimeCache;

        public UPSShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCache)
            : base(gatewayResource, shipMethod, shipCountry)
        {
            _processorSettings = gatewayProviderSettings.ExtendedData.GetProcessorSettings();
            _gatewayProviderSettings = gatewayProviderSettings;
            _shipMethod = shipMethod;
            _runtimeCache = runtimeCache;
            _shipMethod = shipMethod;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            try
            {
                // TODO this should be made configurable
                var visitor = new UPSShipmentLineItemVisitor() { UseOnSalePriceIfOnSale = false };

                shipment.Items.Accept(visitor);

                var province = ShipMethod.Provinces.FirstOrDefault(x => x.Code == shipment.ToRegion);
                                                  
                var collection = GetCollectionFromCache(shipment);

                if (collection == null)
                {
                    try
                    {
                        var http = new UpsHttpRequestHandler();
                        var rateXml = http.Post(RateRequest(shipment, visitor));

                        collection = UpsParseRates(rateXml);

                        _runtimeCache.InsertCacheItem(MakeCacheKey(shipment), () => collection);  
                    }
                    catch (Exception ex)
                    {
                        return Attempt<IShipmentRateQuote>.Fail(
                            new Exception("An error occured during your request : " +
                                          ex.Message +
                                          " Please contact your administrator or try again."));
                    }

                }
                var shippingPrice = 0M;

                var firstCarrierRate = collection.FirstOrDefault(option => option.Service == _shipMethod.ServiceCode);
                if (firstCarrierRate != null)
                    shippingPrice = firstCarrierRate.Rate;
                
                return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, _shipMethod) { Rate = shippingPrice });


            }
            catch (Exception ex)
            {
                return Attempt<IShipmentRateQuote>.Fail(
                           new Exception("An error occured during your request : " +
                                                        ex.Message +
                                                        " Please contact your administrator or try again."));
            }
        }

        protected IEnumerable<DeliveryOption> GetCollectionFromCache(IShipment shipment)
        {
            return _runtimeCache.GetCacheItem(MakeCacheKey(shipment)) as IEnumerable<DeliveryOption>;
        }

        /// <summary>
        /// Generates a unique cache key for runtime caching of the <see cref="FedExShippingGatewayMethod"/>
        /// </summary>
        /// <param name="shipment"> The Shipment</param>
        /// <returns>The unique CacheKey string</returns>
        /// <remarks>
        /// 
        /// CacheKey is assumed to be unique per customer and globally for CheckoutBase.  Therefore this will NOT be unique if 
        /// to different checkouts are happening for the same customer at the same time - we consider that an extreme edge case.
        /// 
        /// </remarks>
        private string MakeCacheKey(IShipment shipment)
        {
            return string.Format("merchello.shippingquotecollection.{0}.{1}.{2}", _shipMethod.ProviderKey, _gatewayProviderSettings.Key, shipment.VersionKey);
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

                        var service = serviceCode;
                        var rate = Convert.ToDecimal(monetaryValue);

                        // tack on additional handling charge for each bag
                        if (!String.IsNullOrEmpty(_processorSettings.UpsAdditionalHandlingCharge))
                        {
                            decimal additionalHandling = Convert.ToDecimal(_processorSettings.UpsAdditionalHandlingCharge);
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
        private string RateRequest(IShipment shipment, UPSShipmentLineItemVisitor visitor)
        {
            var sb = new StringBuilder();

            //Build Access Request
            sb.Append("<?xml version='1.0'?>");
            sb.Append("<AccessRequest xml:lang='en-US'>");
            sb.AppendFormat("<AccessLicenseNumber>{0}</AccessLicenseNumber>", _processorSettings.UpsAccessKey);
            sb.AppendFormat("<UserId>{0}</UserId>", _processorSettings.UpsUserName);
            sb.AppendFormat("<Password>{0}</Password>", _processorSettings.UpsPassword);
            sb.Append("</AccessRequest>");

            //Build Rate Request
            sb.Append("<?xml version='1.0'?>");
                sb.Append("<RatingServiceSelectionRequest xml:lang='en-US'>");
                    sb.Append("<Request>");
                        sb.Append("<TransactionReference>");
                            sb.Append("<CustomerContext>Bare Bones Rate Request</CustomerContext>");
                            sb.Append("<XpciVersion>1</XpciVersion>");
                        sb.Append("</TransactionReference>");
                        sb.Append("<RequestAction>Rate</RequestAction>");
                        sb.Append("<RequestOption>Shop</RequestOption>");
                    sb.Append("</Request>");
                    sb.Append("<PickupType>");
                        sb.AppendFormat("<Code>{0}</Code>", _processorSettings.UpsPickupTypeCode);
                    sb.Append("</PickupType>");
                    sb.Append("<CustomerClassification>");
                        sb.AppendFormat("<Code>{0}</Code>", _processorSettings.UpsCustomerClassification);
                    sb.Append("</CustomerClassification>");
                    sb.Append("<Shipment>");
                        sb.Append("<Description>Rate Quote</Description>");
                        sb.Append("<Shipper>");
                            sb.AppendFormat("<Name>{0}</Name>", shipment.FromName);
                            sb.Append("<ShipperNumber>834010</ShipperNumber>"); // Added for negotiated rates
                            sb.Append("<Address>");
                                sb.AppendFormat("<AddressLine1>{0}</AddressLine1>", shipment.FromAddress1);
                                sb.AppendFormat("<StateProvinceCode>{0}</StateProvinceCode>", shipment.FromRegion);
                                sb.AppendFormat("<PostalCode>{0}</PostalCode>", shipment.FromPostalCode);
                                sb.AppendFormat("<CountryCode>{0}</CountryCode>", shipment.FromCountryCode);
                            sb.Append("</Address>");
                        sb.Append("</Shipper>");
                        sb.Append("<ShipTo>");
                            sb.AppendFormat("<Name>{0}</Name>", shipment.ToName);
                            sb.Append("<Address>");
                        //Required to get accurate residential delivery rates
                            sb.Append("<ResidentialAddressIndicator/>"); 
                                sb.AppendFormat("<AddressLine1>{0}</AddressLine1>", shipment.ToAddress1);
                                sb.AppendFormat("<StateProvinceCode>{0}</StateProvinceCode>", shipment.ToRegion);
                                sb.AppendFormat("<PostalCode>{0}</PostalCode>", shipment.ToPostalCode);
                                sb.AppendFormat("<CountryCode>{0}</CountryCode>", shipment.ToCountryCode);
                            sb.Append("</Address>");
                        sb.Append("</ShipTo>");
                        sb.Append("<ShipFrom>");
                            sb.Append("<Address>");
                                sb.AppendFormat("<AddressLine1>{0}</AddressLine1>", shipment.FromAddress1);
                                sb.AppendFormat("<StateProvinceCode>{0}</StateProvinceCode>", shipment.FromRegion);
                                sb.AppendFormat("<PostalCode>{0}</PostalCode>", shipment.FromPostalCode);
                                sb.AppendFormat("<CountryCode>{0}</CountryCode>", shipment.FromCountryCode);
                            sb.Append("</Address>");
                        sb.Append("</ShipFrom>");
                        sb.Append("<Service>");
                            sb.Append("<Code>03</Code>");
                        sb.Append("</Service>");

                        sb.Append("<Package>");
                            sb.Append("<PackagingType>");
                                sb.AppendFormat("<Code>{0}</Code>", _processorSettings.UpsPackagingTypeCode);
                            sb.Append("</PackagingType>");
                                sb.Append("<UnitOfMeasurement>");
                                    sb.Append("<Code>IN</Code>");
                                sb.Append("</UnitOfMeasurement>");
                                sb.AppendFormat("<Length>{0}</Length>", visitor.TotalLength);
                                sb.AppendFormat("<Width>{0}</Width>", visitor.TotalWidth);
                                sb.AppendFormat("<Height>{0}</Height>", visitor.TotalHeight);
                            sb.Append("<PackageWeight>");
                                sb.Append("<UnitOfMeasurement>");
                                    sb.Append("<Code>LBS</Code>");
                                sb.Append("</UnitOfMeasurement>");
                                sb.Append(string.Format("<Weight>{0}</Weight>", (int)Math.Ceiling(visitor.TotalWeight)));
                            sb.Append("</PackageWeight>");
                        sb.Append("<PackageServiceOptions>");
                            sb.Append("<InsuredValue>");
                                sb.Append("<CurrencyCode>USD</CurrencyCode>");
                                sb.Append("<MonetaryValue>0.00</MonetaryValue>");
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

        public enum UPSType
        {
            UPSNextDayAir,
            UPS2ndDayAir,
            UPSGround,
            UPSWorldwideExpress,
            UPSWorldwideExpidited,
            UPSStandard,
            UPS3DaySelect,
            UPSNextDayAirSaver,
            UPSNextDayAirEarlyAM,
            UPSWorldwideExpressPlus,
            UPS2ndDayAirAM,
        }        
    }
}
