using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Merchello.Plugin.Shipping.USPS;
using UCommerce;
using UCommerce.EntitiesV2;
using UCommerce.Transactions.Shipping;

namespace CustomShippingProviders.USPS
{
    internal enum UspsPackageSize
    {
        Regular,
        Large,
    }

    public class UsPostalServiceShippingProvider
    {
        #region Variables
        private string _connectionUrl = "http://production.shippingapis.com/ShippingAPI.dll";
        private string _uspsUserName = "356MUSIC0921";
        private string _uspsPassword = "861KT46CI002";
        private string _uspsAdditionalHandlingCharge = "";
        private Int32[] _servicesDisabled;
        private string CarrierServicesOfferedDomestic = "";
        private string CarrierServicesOfferedInternational = "";

        private const int MAXPACKAGEWEIGHT = 70;
        private const string MEASUREWEIGHTSYSTEMKEYWORD = "ounce"; 
        private const string MEASUREDIMENSIONSYSTEMKEYWORD = "inches";

        private const string FromZip = "97330";
        private const string FromCountryCode = "US";
        private const string FromState = "OR";
        
        public Money CalculateShippingPrice(Shipment shipment)
        {
            // First sum up the total weight for the shipment.
            // We're assumning that a custom order line property 
            // was set on the order line prior when the product was added to the order line.            

            decimal shippingPrice = 0M;
            try
            {
                var http = new HttpRequestHandler();
                var rateXml = http.Post(RateRequest(shipment));

                var collection = UspsParseRates(rateXml, shipment);
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
                var namedCollection = collection.Where(option => option.Service.Contains(shipment.ShipmentName));
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
                //foreach (var option in namedCollection)
                //{
                    
                //    shippingPrice = shippingPrice != 0 && shippingPrice > option.Rate ? option.Rate : shippingPrice;
                //    var packageSize = GetPackageSize(length, height, width);
                //    if (packageSize == "REGULAR" && option.Service.ToLower().Contains("medium") || packageSize == "LARGE" && option.Service.ToLower().Contains("large"))
                //    {
                //        shippingPrice = option.Rate;
                //    }
                //}
            }
            catch (ApplicationException ex)
            {
                return new Money(0M, shipment.PurchaseOrder.BillingCurrency);
            }

            // To instantiate a new Money object we need the currency,
            // which is set on the purchase order. To get the currency
            // we move through Shipment -> OrderLines -> PurchasrOrder -> Currency
            return new Money(shippingPrice, shipment.PurchaseOrder.BillingCurrency);
        }

        public bool ValidateForShipping(Shipment shipment)
        {
            throw new NotImplementedException();
        }

        private static readonly Dictionary<int, IList<UspsDeliveryOption>> ServiceLookup
            = new Dictionary<int, IList<UspsDeliveryOption>>();

        static UsPostalServiceShippingProvider()
        {
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


        #endregion

        #region Provider specific behaviors
        //public override void Initialize(string name, NameValueCollection config)
        //{
        //    if (config == null)
        //    {
        //        throw new ArgumentNullException("config");
        //    }

        //    base.Initialize(name, config);

        //    _connectionUrl = config["connectionUrl"];

        //    if (String.IsNullOrEmpty(_connectionUrl))
        //    {
        //        throw new ProviderException("Empty or missing connectionUrl");
        //    }

        //    config.Remove("connectionUrl");

        //    _uspsUserName = config["uspsUserName"];

        //    if (String.IsNullOrEmpty(_uspsUserName))
        //    {
        //        throw new ProviderException("Empty or missing uspsUserName");
        //    }

        //    config.Remove("uspsUserName");

        //    _uspsPassword = config["uspsPassword"];

        //    if (String.IsNullOrEmpty(_uspsPassword))
        //    {
        //        throw new ProviderException("Empty or missing uspsPassword");
        //    }

        //    config.Remove("uspsPassword");

        //    _uspsAdditionalHandlingCharge = config["uspsAdditionalHandlingCharge"];

        //    if (String.IsNullOrEmpty(_uspsAdditionalHandlingCharge))
        //    {
        //        throw new ProviderException("Empty or missing uspsAdditionalHandlingCharge");
        //    }

        //    config.Remove("uspsAdditionalHandlingCharge");

        //    String servicesDisabled = config["servicesDisabled"];

        //    if (!String.IsNullOrEmpty(servicesDisabled))
        //    {
        //        String[] servicesDisabledValues = servicesDisabled.Split(',');
        //        _servicesDisabled = new Int32[servicesDisabledValues.Length];

        //        for (int i = 0; i < servicesDisabledValues.Length; i++)
        //        {
        //            Int32 serviceDisabled;
        //            if (!Int32.TryParse(servicesDisabledValues[i].Trim(), out serviceDisabled))
        //            {
        //                throw new ProviderException("servicesDisabled attribute value is badly formatted: " +
        //                                            "should be codes separated by commas.");
        //            }

        //            _servicesDisabled[i] = serviceDisabled;
        //        }

        //        Array.Sort(_servicesDisabled);
        //    }
        //    else
        //    {
        //        _servicesDisabled = new Int32[0];
        //    }

        //    config.Remove("servicesDisabled");

        //    // Throw an exception if unrecognized attributes remain
        //    if (config.Count > 0)
        //    {
        //        string attr = config.GetKey(0);

        //        if (!String.IsNullOrEmpty(attr))
        //        {
        //            throw new ProviderException("Unrecognized attribute: " + attr);
        //        }
        //    }
        //}
        #endregion

        #region Methods
        //public override DeliveryOptionCollection GetDeliveryOptions(PackageInfo package)
        //{
        //    HttpRequestHandler http = new HttpRequestHandler(_connectionUrl);
        //    string rateXml = http.Post(RateRequest(package));
        //    DeliveryOptionCollection collection = UspsParseRates(rateXml, package);
        //    return collection;
        //}

        //public override DeliveryOptionCollection GetDeliveryOptions(PackageInfo package, DeliveryRestrictions restrictions)
        //{
        //    // TODO: I need to put a little more thought into the restrictions
        //    if (restrictions == DeliveryRestrictions.Download)
        //    {
        //        throw new Exception("Shipping Error: This item is download only.");
        //    }

        //    _deliveryRestriction = restrictions;
        //    HttpRequestHandler http = new HttpRequestHandler(_connectionUrl);
        //    string rateXml = http.Post(RateRequest(package));
        //    DeliveryOptionCollection collection = UspsParseRates(rateXml, package);
        //    return collection;
        //}

        //public override DeliveryOptionCollection GetDeliveryOptions(PackageInfo package, decimal orderAmount, int orderQuantity)
        //{
        //    HttpRequestHandler http = new HttpRequestHandler(_connectionUrl);
        //    string rateXml = http.Post(RateRequest(package));
        //    DeliveryOptionCollection collection = UspsParseRates(rateXml, package);
        //    return collection;
        //}

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

        private DeliveryOptionCollection UspsParseRates(string response, Shipment package)
        {
            var optionCollection = new DeliveryOptionCollection();

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

                        if (!String.IsNullOrEmpty(_uspsAdditionalHandlingCharge))
                        {
                            decimal additionalHandling;

                            if (Decimal.TryParse(_uspsAdditionalHandlingCharge, out additionalHandling))
                            {
                                rate += additionalHandling;
                            }
                        }

                        // Weed out unavailable service rates
                        //if (options.Any(option => option.Fits(weight, width, length, height)))
                        //{
                            var deliveryOption = new DeliveryOption {Rate = rate, Service = mailService};
                            optionCollection.Add(deliveryOption);
                        //}
                    }
                }

                return optionCollection;
            }
        }

        private IEnumerable<UspsDeliveryOption> GetDeliveryOptions(int serviceCode, Shipment package)
        {
            IList<UspsDeliveryOption> options;

            if (!IsServiceEnabled(serviceCode))
            {
                return null;
            }

            if (!ServiceLookup.TryGetValue(serviceCode, out options))
            {
                return null;
            }

            //if (IsMediaService(serviceCode))
            //{
            //    String isMediaValue;
            //    Boolean isMedia;

            //    if (package.Args == null
            //        || !package.Args.TryGetValue(PackageInfoArgs.IsMedia, out isMediaValue)
            //        || !Boolean.TryParse(isMediaValue, out isMedia)
            //        || !isMedia)
            //    {
            //        return null;
            //    }
            //}

            return options;
        }

        private bool IsServiceEnabled(int serviceCode)
        {
            if (_servicesDisabled == null)
            {
                return true;
            }
            return Array.BinarySearch(_servicesDisabled, serviceCode) < 0;
        }

        private static bool IsMediaService(int serviceCode)
        {
            return serviceCode == 6;
        }

        // Create rating request XML string
        private string RateRequest(Shipment shipment)
        {
            // Changed weight logic per JeffreyABecker suggestions
            //int tOz = (int)Math.Ceiling(weight * 16.0m);
            //int packageWeightPounds = tOz / 16;
            //int packageWeightOunces = tOz % 16;

            var sb = new StringBuilder();
            int countryId = 0;
            if (!int.TryParse(shipment.ShipmentAddress.Country.Name, out countryId))
            {
                countryId = shipment.ShipmentAddress.Country.CountryId;
            }
            sb.AppendFormat(
                Country.Get(countryId).TwoLetterISORegionName == "US"
                    ? "<RateV4Request USERID=\"{0}\" PASSWORD=\"{1}\">"
                    : "<IntlRateV2Response  USERID=\"{0}\" PASSWORD=\"{1}\">", _uspsUserName, _uspsPassword);
            sb.Append("<Revision/>");
            string packageStr = BuildRatePackage(shipment);
            sb.Append(packageStr);
            sb.Append("</RateV4Request>");
            return "API=RateV4&XML=" + sb;
        }

        private string BuildRatePackage(Shipment shipment)
        {
            var count = 0;
            var sb = new StringBuilder();
            foreach (var order in shipment.PurchaseOrder.OrderLines)
            {
                var weight = 0M;
                var height = 0M;
                var length = 0M;
                var width = 0M;
                bool freeShipping = false;

                try
                {
                    if (order.OrderProperties != null)
                    {
                        var property = order.OrderProperties.FirstOrDefault(x => x.Key == "FreeShipping");
                        if (property != null)
                            bool.TryParse(property.Value, out freeShipping);

                        property = order.OrderProperties.FirstOrDefault(x => x.Key == "PackageWeight");
                        if (property != null)
                            weight = Convert.ToDecimal(property.Value);

                        property = order.OrderProperties.FirstOrDefault(x => x.Key == "PackageHeight");
                        if (property != null)
                            height = Convert.ToDecimal(property.Value);

                        property = order.OrderProperties.FirstOrDefault(x => x.Key == "PackageLength");
                        if (property != null)
                            length = Convert.ToDecimal(property.Value);

                        property = order.OrderProperties.FirstOrDefault(x => x.Key == "PackageWidth");
                        if (property != null)
                            width = Convert.ToDecimal(property.Value);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                if (!freeShipping)
                {
                    var productWeight = (int) Math.Ceiling(weight);
                    int tOz = (int) Math.Ceiling(productWeight*16.0m);
                    int packageWeightPounds = tOz/16;
                    int packageWeightOunces = tOz%16;
                    var productHeight = (int) Math.Ceiling(height);
                    var productLength = (int) Math.Ceiling(length);
                    var productWidth = (int) Math.Ceiling(width);
                    string packageSize = GetPackageSize(productLength, productHeight, productWidth);
                    for (var i = 0; i < order.Quantity; i++)
                    {
                        sb.AppendFormat("<Package ID=\"{0}\">", count);
                        sb.Append("<Service>ALL</Service>");
                        sb.AppendFormat("<ZipOrigination>{0}</ZipOrigination>", FromZip);
                        sb.AppendFormat("<ZipDestination>{0}</ZipDestination>", shipment.ShipmentAddress.PostalCode);
                        sb.AppendFormat("<Pounds>{0}</Pounds>", packageWeightPounds);
                        sb.AppendFormat("<Ounces>{0}</Ounces>", packageWeightOunces);
                        sb.Append("<Container>Variable</Container>");
                        sb.AppendFormat("<Size>{0}</Size>", packageSize);
                        sb.AppendFormat("<Width>{0}</Width>", width);
                        sb.AppendFormat("<Length>{0}</Length>", length);
                        sb.AppendFormat("<Height>{0}</Height>", height);
                        sb.Append("<Machinable>FALSE</Machinable>"); // Packages are not machinable
                        sb.Append("</Package>");

                        count++;
                    }
                }
            }
            return sb.ToString();
            //if (!IsPackageTooHeavy(packageWeightPounds)
            //    && !IsPackageTooLarge(length, height, width))
            //{
            //    // Rate single package
            //    string packageSize = GetPackageSize(length, height, width);

            //    var sb = new StringBuilder();
            //    sb.Append("<Package ID=\"0\">");
            //    sb.Append("<Service>ALL</Service>");
            //    sb.AppendFormat("<ZipOrigination>{0}</ZipOrigination>", FromZip);
            //    sb.AppendFormat("<ZipDestination>{0}</ZipDestination>", shipment.ShipmentAddress.PostalCode);
            //    sb.AppendFormat("<Pounds>{0}</Pounds>", packageWeightPounds);
            //    sb.AppendFormat("<Ounces>{0}</Ounces>", packageWeightOunces);
            //    sb.Append("<Container>Variable</Container>");
            //    sb.AppendFormat("<Size>{0}</Size>", packageSize);
            //    sb.Append("<Machinable>FALSE</Machinable>"); // Packages are not machinable
            //    sb.Append("</Package>");

            //    return sb.ToString();
            //}
            //else
            //{
            //    // Rate multiple packages
            //    int totalPackages = TotalPackageSize(length, height, width) / 108;
            //    int tempPackageWeightPounds = packageWeightPounds / totalPackages;
            //    int temppackageWeightOunces = packageWeightOunces / totalPackages;
            //    int tempPackageHeight = height / totalPackages;
            //    int tempPackageWidth = width / totalPackages;
            //    string packageSize = GetPackageSize(length, tempPackageHeight, tempPackageWidth);

            //    var sb = new StringBuilder();

            //    for (int i = 0; i <= totalPackages; i++)
            //    {
            //        sb.AppendFormat("<Package ID=\"{0}\">", i);
            //        sb.Append("<Service>ALL</Service>");
            //        sb.AppendFormat("<ZipOrigination>{0}</ZipOrigination>", FromZip);
            //        sb.AppendFormat("<ZipDestination>{0}</ZipDestination>", shipment.ShipmentAddress.PostalCode);
            //        sb.AppendFormat("<Pounds>{0}</Pounds>", tempPackageWeightPounds);
            //        sb.AppendFormat("<Ounces>{0}</Ounces>", temppackageWeightOunces);
            //        sb.AppendFormat("<Size>{0}</Size>", packageSize);
            //        sb.Append("<Machinable>FALSE</Machinable>"); // Packages are not machinable
            //        sb.Append("</Package>");
            //    }

            //    return sb.ToString();
            //}
        }
        #endregion
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
