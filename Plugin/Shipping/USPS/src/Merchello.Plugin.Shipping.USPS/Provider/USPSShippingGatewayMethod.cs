using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Plugin.Shipping.USPS.Models;
using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Merchello.Plugin.Shipping.USPS.Provider
{

    [GatewayMethodEditor("USPS Shipping Method Editor", "~/App_Plugins/Merchello.USPS/shippingmethod.html")]
    public class UspsShippingGatewayMethod : ShippingGatewayMethodBase, IUspsShippingGatewayMethod
    {
        private IShipMethod _shipMethod;
        private UspsProcessorSettings _processorSettings;
        private IRuntimeCacheProvider _runtimeCache;
        private IGatewayProviderSettings _gatewayProviderSettings;

        public UspsShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCache) : 
            base(gatewayResource, shipMethod, shipCountry)
        {
            _processorSettings = gatewayProviderSettings.ExtendedData.GetProcessorSettings();
            _shipMethod = shipMethod;
            _runtimeCache = runtimeCache;
            _gatewayProviderSettings = gatewayProviderSettings;

            // Express Mail Hold For Pickup
            ServiceLookup[2] = new[] { new UspsDeliveryOption(2, Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue, 108, 70) };

            // Express Mail Flat-Rate Envelope Hold For Pickup
            ServiceLookup[27] = new[] { new UspsDeliveryOption(27, 12.5M, 9.5M, 0.75M, Decimal.MaxValue, 70) };

            // Express Mail
            ServiceLookup[3] = new[] { new UspsDeliveryOption(3, Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue, 108, 70) };

            // Express Mail Flat-Rate Envelope
            ServiceLookup[13] = new[] { new UspsDeliveryOption(13, 12.5M, 9.5M, 0.75M, Decimal.MaxValue, 70) };

            // Priority Mail
            ServiceLookup[1] = new[] { new UspsDeliveryOption(1, Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue, 108, 70) };

            // Priority Mail Flat-Rate Envelope
            ServiceLookup[16] = new[] { new UspsDeliveryOption(16, 12.5M, 9.5M, 0.75M, Decimal.MaxValue, 70) };

            // Priority Mail Small Flat-Rate Box
            ServiceLookup[28] = new[] { new UspsDeliveryOption(28, 5.375M, 8.625M, 1.675M, Decimal.MaxValue, 70) };

            // Priority Mail Regular/Medium Flat-Rate Boxes
            ServiceLookup[17] = new[]
                                    {
                                        new UspsDeliveryOption(17, 11.875M, 13.625M, 3.375M, Decimal.MaxValue, 70),
                                        new UspsDeliveryOption(17, 11M, 8.5M, 5.5M, Decimal.MaxValue, 70)
                                    };

            // Priority Mail Large Flat-Rate Box
            ServiceLookup[22] = new[] { new UspsDeliveryOption(22, 12, 12, 5.5M, Decimal.MaxValue, 70) };

            // Parcel Post
            ServiceLookup[4] = new[] { new UspsDeliveryOption(4, Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue, 108, 70) };

            // Bound Printed Matter
            ServiceLookup[5] = new[] { new UspsDeliveryOption(5, Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue, 108, 70) };

            // Media Mail
            ServiceLookup[6] = new[] { new UspsDeliveryOption(6, Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue, 108, 70) };

            // Library Mail
            ServiceLookup[7] = new[] { new UspsDeliveryOption(7, Decimal.MaxValue, Decimal.MaxValue, Decimal.MaxValue, 108, 70) };
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {            
            decimal shippingPrice = 0M;
            try
            {
                var visitor = new UspsShipmentLineItemVisitor() { UseOnSalePriceIfOnSale = false };

                shipment.Items.Accept(visitor);

                var http = new HttpRequestHandler();

                var collection = GetCollectionFromCache(shipment);

                if (collection == null)
                {
                    var rateXml = http.Post(RateRequest(shipment, visitor));

                    collection = UspsParseRates(rateXml, shipment);

                    _runtimeCache.InsertCacheItem(MakeCacheKey(shipment), () => collection);      
                }
                //if (collection.Any(option => option.Service.Contains(shipment.ShipmentName) &&
                //                             option.Service.ToLower().Contains("medium") || option.Service.ToLower().Contains("large")))
                //{
                //    var packageSize = GetPackageSize(length, height, width);
                //    shippingPrice = packageSize == "REGULAR" ? collection.First(option => option.Service.Contains(shipment.ShipmentName) && 
                //        option.Service.ToLower().Contains("medium")).Rate : collection.First(option => option.Service.Contains(shipment.ShipmentName) && 
                //            option.Service.ToLower().Contains("large")).Rate;
                //}
                //else
                //{
                //    shippingPrice = collection.First(option => option.Service.Contains(shipment.ShipmentName)).Rate;
                //}
                var namedCollection = collection.Where(option => HttpUtility.HtmlDecode(option.Service).Contains(HttpUtility.HtmlDecode(_shipMethod.ServiceCode)));
                var deliveryOptions = namedCollection as IList<DeliveryOption> ?? namedCollection.ToList();
                if (deliveryOptions.Any(o => o.Service.Contains("medium") || o.Service.Contains("large")))
                {
                    var packageSize = GetPackageSize(1, 1, 1);
                    var firstOrDefault = deliveryOptions.FirstOrDefault(x => x.Service.Contains("medium"));
                    if (firstOrDefault != null)
                    {
                        var deliveryOption = deliveryOptions.FirstOrDefault(x => x.Service.Contains("large"));
                        if (deliveryOption != null)
                            shippingPrice = packageSize == "REGULAR" ? firstOrDefault.Rate : deliveryOption.Rate;
                    }
                }
                else
                {
                    var firstOrDefault = deliveryOptions.FirstOrDefault();
                    if (firstOrDefault != null) shippingPrice = firstOrDefault.Rate;
                }
            }
            catch (Exception ex)
            {
                return Attempt<IShipmentRateQuote>.Fail(
                           new Exception("An error occured during your request : " +
                                                        ex.Message +
                                                        " Please contact your administrator or try again."));
            }

            return Attempt<IShipmentRateQuote>.Succeed(new ShipmentRateQuote(shipment, _shipMethod) { Rate = shippingPrice });
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

        // Create rating request XML string
        private string RateRequest(IShipment shipment, UspsShipmentLineItemVisitor visitor)
        {
            // Changed weight logic per JeffreyABecker suggestions
            //int tOz = (int)Math.Ceiling(weight * 16.0m);
            //int packageWeightPounds = tOz / 16;
            //int packageWeightOunces = tOz % 16;

            var sb = new StringBuilder();
            sb.AppendFormat(shipment.ToCountryCode == "US"
                    ? "<RateV4Request USERID=\"{0}\" PASSWORD=\"{1}\">"
                    : "<IntlRateV2Response  USERID=\"{0}\" PASSWORD=\"{1}\">", _processorSettings.UspsUsername, _processorSettings.UspsPassword);
            sb.Append("<Revision/>");
            string packageStr = BuildRatePackage(shipment, visitor);
            sb.Append(packageStr);
            sb.Append("</RateV4Request>");
            return "API=RateV4&XML=" + sb;
        }

        private string BuildRatePackage(IShipment shipment, UspsShipmentLineItemVisitor visitor)
        {
            var count = 0;
            var sb = new StringBuilder();        
             
            var productWeight = (int)Math.Ceiling(visitor.TotalWeight);
            int tOz = (int)Math.Ceiling(productWeight * 16.0m);
            int packageWeightPounds = tOz / 16;
            int packageWeightOunces = tOz % 16;

            var productWidth = (int)Math.Ceiling(visitor.TotalWidth);
            var productLength = (int)Math.Ceiling(visitor.TotalLength);
            var productHeight = (int)Math.Ceiling(visitor.TotalHeight);
            string packageSize = GetPackageSize(productLength, productHeight, productWidth);
           
            sb.AppendFormat("<Package ID=\"{0}\">", count);
            sb.Append("<Service>ALL</Service>");
            sb.AppendFormat("<ZipOrigination>{0}</ZipOrigination>", shipment.FromPostalCode);
            sb.AppendFormat("<ZipDestination>{0}</ZipDestination>", shipment.ToPostalCode);
            sb.AppendFormat("<Pounds>{0}</Pounds>", packageWeightPounds);
            sb.AppendFormat("<Ounces>{0}</Ounces>", packageWeightOunces);
            sb.Append("<Container>Variable</Container>");
            sb.AppendFormat("<Size>{0}</Size>", packageSize);
            sb.AppendFormat("<Width>{0}</Width>", productWidth);
            sb.AppendFormat("<Length>{0}</Length>", productLength);
            sb.AppendFormat("<Height>{0}</Height>", productHeight);
            sb.Append("<Machinable>FALSE</Machinable>"); // Packages are not machinable
            sb.Append("</Package>");
                                               
            return sb.ToString();
        }

        private bool IsPackageTooLarge(int length, int height, int width)
        {
            int total = TotalPackageSize(length, height, width);
            return total > 130;
        }

        private int TotalPackageSize(int length, int height, int width)
        {
            int girth = height + height + width + width;
            int total = girth + length;
            return total;
        }

        private static bool IsPackageTooHeavy(int weight)
        {
            return weight > 70;
        }

        private static string GetPackageSize(int length, int height, int width)
        {
            int girth = height + height + width + width;
            int total = girth + length;

            if (total <= 84)
            {
                return "REGULAR";
            }

            if ((total > 84) && (total <= 108))
            {
                return "LARGE";
            }

            if ((total > 108) && (total <= 130))
            {
                return "OVERSIZE";
            }

            throw new Exception("Shipping Error: Package too large.");
        }

        private IEnumerable<DeliveryOption> UspsParseRates(string response, IShipment package)
        {
            var optionCollection = new List<DeliveryOption>();

            using (var reader = new XmlTextReader(new StringReader(response)))
            {
                while (reader.Read())
                {
                    if ((reader.Name == "Error") && (reader.NodeType == XmlNodeType.Element))
                    {
                        string errorText = "";

                        while (reader.Read())
                        {
                            if ((reader.Name == "HelpContext") && (reader.NodeType == XmlNodeType.Element))
                            {
                                errorText += "USPS Help Context: " + reader.ReadString() + ", ";
                            }

                            if ((reader.Name == "Description") && (reader.NodeType == XmlNodeType.Element))
                            {
                                errorText += "Error Desc: " + reader.ReadString();
                            }
                        }

                        throw new ProviderException("USPS Error returned: " + errorText);
                    }

                    if ((reader.Name == "Postage") && (reader.NodeType == XmlNodeType.Element))
                    {
                        int serviceCode;
                        Int32.TryParse(reader.GetAttribute("CLASSID"), out serviceCode);
                        string postalRate = "";
                        String mailService = "";

                        reader.Read();

                        if (reader.Name == "MailService" && reader.NodeType == XmlNodeType.Element)
                        {
                            mailService = reader.ReadString();
                            reader.ReadEndElement();
                        }

                        if (reader.Name == "Rate" && reader.NodeType == XmlNodeType.Element)
                        {
                            postalRate = reader.ReadString();
                            reader.ReadEndElement();
                        }

                        IEnumerable<UspsDeliveryOption> options = GetDeliveryOptions(serviceCode, package);

                        if (options == null)
                        {
                            continue;
                        }

                        decimal rate;

                        if (!Decimal.TryParse(postalRate, out rate))
                        {
                            continue;
                        }

                        if (!String.IsNullOrEmpty(_processorSettings.UspsAdditionalHandlingCharge))
                        {
                            decimal additionalHandling;

                            if (Decimal.TryParse(_processorSettings.UspsAdditionalHandlingCharge, out additionalHandling))
                            {
                                rate += additionalHandling;
                            }
                        }

                        // Weed out unavailable service rates
                        //if (options.Any(option => option.Fits(weight, width, length, height)))
                        //{
                        var deliveryOption = new DeliveryOption { Rate = rate, Service = mailService, ServiceCode = serviceCode.ToString() };
                        optionCollection.Add(deliveryOption);
                        //}
                    }
                }

                return optionCollection;
            }
        }

        private IEnumerable<UspsDeliveryOption> GetDeliveryOptions(int serviceCode, IShipment package)
        {
            IList<UspsDeliveryOption> options;
       
            if (!ServiceLookup.TryGetValue(serviceCode, out options))
            {
                return null;
            }

            return options;
        }

         private static readonly Dictionary<int, IList<UspsDeliveryOption>> ServiceLookup = new Dictionary<int, IList<UspsDeliveryOption>>();


        static UspsShippingGatewayMethod()
        {

        }

        internal class UspsDeliveryOption
        {
            public UspsDeliveryOption(int classId,
                                      Decimal widthLimit,
                                      Decimal heightLimit,
                                      Decimal lengthLimit,
                                      Decimal combinedLimit,
                                      Decimal weightLimit)
            {
                ClassId = classId;
                WidthLimit = widthLimit;
                HeightLimit = heightLimit;
                LengthLimit = lengthLimit;
                CombinedLimit = combinedLimit;
                WeightLimit = weightLimit;
            }

            public int ClassId { get; private set; }
            public Decimal WidthLimit { get; private set; }
            public Decimal HeightLimit { get; private set; }
            public Decimal LengthLimit { get; private set; }
            public Decimal CombinedLimit { get; private set; }
            public Decimal WeightLimit { get; private set; }

            public bool Fits(decimal weight, int width, int length, int height)
            {
                if (WeightLimit < weight)
                {
                    return false;
                }

                if (Math.Min(HeightLimit, WidthLimit) < Math.Min(width, height))
                {
                    return false;
                }

                if (LengthLimit < length)
                {
                    return false;
                }

                if (CombinedLimit < width + height)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
