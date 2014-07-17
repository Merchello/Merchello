using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Cache;
using RateAvailableServiceWebServiceClient.RateServiceWebReference;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Notification = RateAvailableServiceWebServiceClient.RateServiceWebReference.Notification;

namespace Merchello.Plugin.Shipping.FedEx.Provider
{
    [GatewayMethodEditor("FedEx Shipping Method Editor", "~/App_Plugins/Merchello.FedEx/shippingmethod.html")]
    public class FedExShippingGatewayMethod : ShippingGatewayMethodBase, IFedExShippingGatewayMethod
    {
        private FedExShippingProcessor _processor;
        private FedExType _fedExType;
        private IShipMethod _shipMethod;
        private IRuntimeCacheProvider _runtimeCache;
        private IGatewayProviderSettings _gatewayProviderSettings;
        public FedExShippingGatewayMethod(IGatewayResource gatewayResource, IShipMethod shipMethod, IShipCountry shipCountry, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayResource, shipMethod, shipCountry)
        {
            _processor = new FedExShippingProcessor(gatewayProviderSettings.ExtendedData.GetProcessorSettings());
            _gatewayProviderSettings = gatewayProviderSettings;
            _shipMethod = shipMethod;
            _runtimeCache = runtimeCacheProvider;
        }

        public override Attempt<IShipmentRateQuote> QuoteShipment(IShipment shipment)
        {
            try
            {
                // TODO this should be made configurable
                var visitor = new FedExShipmentLineItemVisitor() { UseOnSalePriceIfOnSale = false };

                shipment.Items.Accept(visitor);

                var province = ShipMethod.Provinces.FirstOrDefault(x => x.Code == shipment.ToRegion);

                var shippingPrice = 0M;
                try
                {
                    var service = new RateService { Url = "https://wsbeta.fedex.com:443/web-services/rate" };

                    var collection = GetCollectionFromCache(shipment);
                    if (collection == null)
                    {
                        var reply = service.getRates(RateRequest(shipment, visitor.TotalWeight, visitor.Quantity));



                        if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING)
                        {
                            collection = BuildDeliveryOptions(reply, shipment);

                            _runtimeCache.InsertCacheItem(MakeCacheKey(shipment), () => collection);       
                        }
                    }

                    var firstCarrierRate = collection.FirstOrDefault(option => option.Service.Contains(_shipMethod.ServiceCode));
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

        private RateRequest RateRequest(IShipment shipment, decimal totalWeight, int quantity)
        {
            // Build the RateRequest
            var request = new RateRequest
            {
                WebAuthenticationDetail =
                    new WebAuthenticationDetail
                    {
                        UserCredential =
                            new WebAuthenticationCredential
                            {
                                Key = "fhbqZrPFKWRe2Ndf",
                                Password = "yWBFMQJBIHDcJRHnYQ06KQZLH"
                            }
                    }
            };

            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.WebAuthenticationDetail.UserCredential.Key = getProperty("key");
                request.WebAuthenticationDetail.UserCredential.Password = getProperty("password");
            }

            request.ClientDetail = new ClientDetail { AccountNumber = "510087569", MeterNumber = "118628262" };
            if (usePropertyFile()) //Set values from a file for testing purposes
            {
                request.ClientDetail.AccountNumber = getProperty("accountnumber");
                request.ClientDetail.MeterNumber = getProperty("meternumber");
            }

            request.TransactionDetail = new TransactionDetail
            {
                CustomerTransactionId = "***Rate Available Services v14 Request using VC#***"
            };

            request.Version = new VersionId();

            request.ReturnTransitAndCommit = true;
            request.ReturnTransitAndCommitSpecified = true;

            SetShipmentDetails(request, shipment, totalWeight, quantity);

            return request;
        }

        private static void SetShipmentDetails(RateRequest request, IShipment shipment, decimal totalWeight, int quantity)
        {
            request.RequestedShipment = new RequestedShipment
            {
                ShipTimestamp = DateTime.Now,
                ShipTimestampSpecified = true,
                DropoffType = DropoffType.REGULAR_PICKUP,
                DropoffTypeSpecified = true,
                PackagingType = PackagingType.YOUR_PACKAGING,
                PackagingTypeSpecified = true
            };

            SetOrigin(request, shipment);

            SetDestination(request, shipment);

            SetPackageLineItems(request, shipment, totalWeight, quantity);

            request.RequestedShipment.RateRequestTypes = new RateRequestType[2];
            request.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
            request.RequestedShipment.RateRequestTypes[1] = RateRequestType.LIST;

            request.RequestedShipment.PackageCount = quantity.ToString();
            //set to true to request COD shipment
            var isCodShipment = false;
            if (isCodShipment)
                SetCOD(request);
        }

        private static void SetOrigin(RateRequest request, IShipment shipment)
        {
            request.RequestedShipment.Shipper = new Party
            {
                Address =
                    new RateAvailableServiceWebServiceClient.RateServiceWebReference.Address
                    {
                        StreetLines = new[] { shipment.FromAddress1, shipment.FromAddress2 },
                        StateOrProvinceCode = "WA",//shipment.FromRegion,
                        PostalCode = "98225",//shipment.FromPostalCode,
                        CountryCode = shipment.FromCountryCode
                    }
            };
        }

        private static void SetDestination(RateRequest request, IShipment shipment)
        {
            request.RequestedShipment.Recipient = new Party
            {
                Address =
                    new RateAvailableServiceWebServiceClient.RateServiceWebReference.Address
                    {
                        StreetLines = new[] { shipment.ToAddress1, shipment.ToAddress2 },
                        StateOrProvinceCode = shipment.ToRegion,
                        PostalCode = shipment.ToPostalCode,
                        CountryCode = shipment.ToCountryCode
                    }
            };
        }

        private static void SetPackageLineItems(RateRequest request, IShipment shipment, decimal totalWeight, int quantity)
        {
            var count = 1;

            for (var i = 0; i < quantity; i++)
            {
                request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[1];
                request.RequestedShipment.RequestedPackageLineItems[0] = new RequestedPackageLineItem
                {
                    SequenceNumber = count.ToString(),
                    GroupPackageCount = "1",
                    Weight =
                        new Weight
                        {
                            Units = WeightUnits.LB,
                            UnitsSpecified = true,
                            Value = totalWeight,
                            ValueSpecified = true
                        },
                    Dimensions = new Dimensions
                    {
                        Units = LinearUnits.IN,
                        UnitsSpecified = true
                    }
                };
                count++;

            }
        }

        private static void SetCOD(RateRequest request)
        {
            // To get all COD rates, set both COD details at both package and shipment level
            // Set COD at Package level for Ground Services
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested = new PackageSpecialServicesRequested
            {
                SpecialServiceTypes = new PackageSpecialServiceType[1]
            };
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.SpecialServiceTypes[0] = PackageSpecialServiceType.COD;
            //
            request.RequestedShipment.RequestedPackageLineItems[0].SpecialServicesRequested.CodDetail = new CodDetail
            {
                CollectionType = CodCollectionType.GUARANTEED_FUNDS,
                CodCollectionAmount =
                    new RateAvailableServiceWebServiceClient.RateServiceWebReference.Money
                    {
                        Amount = 250,
                        AmountSpecified = true,
                        Currency = "USD"
                    }
            };
            // Set COD at Shipment level for Express Services
            request.RequestedShipment.SpecialServicesRequested = new ShipmentSpecialServicesRequested
            {
                SpecialServiceTypes = new ShipmentSpecialServiceType[1]
            }; // Special service requested
            request.RequestedShipment.SpecialServicesRequested.SpecialServiceTypes[0] = ShipmentSpecialServiceType.COD;
            //
            request.RequestedShipment.SpecialServicesRequested.CodDetail = new CodDetail
            {
                CodCollectionAmount =
                    new RateAvailableServiceWebServiceClient.RateServiceWebReference.Money
                    {
                        Amount = 150,
                        AmountSpecified = true,
                        Currency = "USD"
                    },
                CollectionType = CodCollectionType.GUARANTEED_FUNDS
            };
        }

        private static IEnumerable<DeliveryOption> BuildDeliveryOptions(RateReply rateReply, IShipment shipment)
        {
            var optionCollection = new DeliveryOptionCollection();
            foreach (var rateReplyDetail in rateReply.RateReplyDetails)
            {
                var service = rateReplyDetail.ServiceType.ToString();

                optionCollection.AddRange(rateReplyDetail.RatedShipmentDetails.Select(shipmentDetail => shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount).Select(rate => new DeliveryOption { Rate = rate, Service = service }));
            }

            return optionCollection;
        }

        private static void ShowRateReply(RateReply reply)
        {
            Console.WriteLine("RateReply details:");
            for (int i = 0; i < reply.RateReplyDetails.Length; i++)
            {
                RateReplyDetail rateReplyDetail = reply.RateReplyDetails[i];
                Console.WriteLine("Rate Reply Detail for Service {0} ", i + 1);
                if (rateReplyDetail.ServiceTypeSpecified)
                    Console.WriteLine("Service Type: {0}", rateReplyDetail.ServiceType);
                if (rateReplyDetail.PackagingTypeSpecified)
                    Console.WriteLine("Packaging Type: {0}", rateReplyDetail.PackagingType);

                for (int j = 0; j < rateReplyDetail.RatedShipmentDetails.Length; j++)
                {
                    RatedShipmentDetail shipmentDetail = rateReplyDetail.RatedShipmentDetails[j];
                    Console.WriteLine("---Rated Shipment Detail for Rate Type {0}---", j + 1);
                    ShowShipmentRateDetails(shipmentDetail);
                    ShowPackageRateDetails(shipmentDetail.RatedPackages);
                }
                ShowDeliveryDetails(rateReplyDetail);
                Console.WriteLine("**********************************************************");
            }
        }

        private static void ShowShipmentRateDetails(RatedShipmentDetail shipmentDetail)
        {
            if (shipmentDetail == null) return;
            if (shipmentDetail.ShipmentRateDetail == null) return;
            ShipmentRateDetail rateDetail = shipmentDetail.ShipmentRateDetail;
            Console.WriteLine("--- Shipment Rate Detail ---");
            //
            Console.WriteLine("RateType: {0} ", rateDetail.RateType);
            if (rateDetail.TotalBillingWeight != null) Console.WriteLine("Total Billing Weight: {0} {1}", rateDetail.TotalBillingWeight.Value, shipmentDetail.ShipmentRateDetail.TotalBillingWeight.Units);
            if (rateDetail.TotalBaseCharge != null) Console.WriteLine("Total Base Charge: {0} {1}", rateDetail.TotalBaseCharge.Amount, rateDetail.TotalBaseCharge.Currency);
            if (rateDetail.TotalFreightDiscounts != null) Console.WriteLine("Total Freight Discounts: {0} {1}", rateDetail.TotalFreightDiscounts.Amount, rateDetail.TotalFreightDiscounts.Currency);
            if (rateDetail.TotalSurcharges != null) Console.WriteLine("Total Surcharges: {0} {1}", rateDetail.TotalSurcharges.Amount, rateDetail.TotalSurcharges.Currency);
            if (rateDetail.Surcharges != null)
            {
                // Individual surcharge for each package
                foreach (Surcharge surcharge in rateDetail.Surcharges)
                    Console.WriteLine(" {0} surcharge {1} {2}", surcharge.SurchargeType, surcharge.Amount.Amount, surcharge.Amount.Currency);
            }
            if (rateDetail.TotalNetCharge != null) Console.WriteLine("Total Net Charge: {0} {1}", rateDetail.TotalNetCharge.Amount, rateDetail.TotalNetCharge.Currency);
        }

        private static void ShowPackageRateDetails(RatedPackageDetail[] ratedPackages)
        {
            if (ratedPackages == null) return;
            Console.WriteLine("--- Rated Package Detail ---");
            for (int i = 0; i < ratedPackages.Length; i++)
            {
                RatedPackageDetail ratedPackage = ratedPackages[i];
                Console.WriteLine("Package {0}", i + 1);
                if (ratedPackage.PackageRateDetail != null)
                {
                    Console.WriteLine("Billing weight {0} {1}", ratedPackage.PackageRateDetail.BillingWeight.Value, ratedPackage.PackageRateDetail.BillingWeight.Units);
                    Console.WriteLine("Base charge {0} {1}", ratedPackage.PackageRateDetail.BaseCharge.Amount, ratedPackage.PackageRateDetail.BaseCharge.Currency);
                    if (ratedPackage.PackageRateDetail.TotalSurcharges != null) Console.WriteLine("Total Surcharges: {0} {1}", ratedPackage.PackageRateDetail.TotalSurcharges.Amount, ratedPackage.PackageRateDetail.TotalSurcharges.Currency);
                    if (ratedPackage.PackageRateDetail.Surcharges != null)
                    {
                        foreach (Surcharge surcharge in ratedPackage.PackageRateDetail.Surcharges)
                        {
                            Console.WriteLine(" {0} surcharge {1} {2}", surcharge.SurchargeType, surcharge.Amount.Amount, surcharge.Amount.Currency);
                        }
                    }
                    Console.WriteLine("Net charge {0} {1}", ratedPackage.PackageRateDetail.NetCharge.Amount, ratedPackage.PackageRateDetail.NetCharge.Currency);
                }
            }
        }

        private static void ShowDeliveryDetails(RateReplyDetail rateReplyDetail)
        {
            if (rateReplyDetail.DeliveryTimestampSpecified)
                Console.WriteLine("Delivery timestamp: " + rateReplyDetail.DeliveryTimestamp.ToString());
            if (rateReplyDetail.TransitTimeSpecified)
                Console.WriteLine("Transit time: " + rateReplyDetail.TransitTime);
        }

        private static void ShowNotifications(RateReply reply)
        {
            Console.WriteLine("Notifications");
            for (int i = 0; i < reply.Notifications.Length; i++)
            {
                Notification notification = reply.Notifications[i];
                Console.WriteLine("Notification no. {0}", i);
                Console.WriteLine(" Severity: {0}", notification.Severity);
                Console.WriteLine(" Code: {0}", notification.Code);
                Console.WriteLine(" Message: {0}", notification.Message);
                Console.WriteLine(" Source: {0}", notification.Source);
            }
        }
        private static bool usePropertyFile() //Set to true for common properties to be set with getProperty function.
        {
            return getProperty("usefile").Equals("True");
        }
        private static String getProperty(String propertyname) //Sets common properties for testing purposes.
        {
            try
            {
                String filename = "C:\\filepath\\filename.txt";
                if (System.IO.File.Exists(filename))
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(filename);
                    do
                    {
                        String[] parts = sr.ReadLine().Split(',');
                        if (parts[0].Equals(propertyname) && parts.Length == 2)
                        {
                            return parts[1];
                        }
                    }
                    while (!sr.EndOfStream);
                }
                Console.WriteLine("Property {0} set to default 'XXX'", propertyname);
                return "XXX";
            }
            catch (Exception e)
            {
                Console.WriteLine("Property {0} set to default 'XXX'", propertyname);
                return "XXX";
            }
        }
        private bool IncludedService(string serviceCode)
        {

            return true;
        }

        public enum FedExType
        {
            FedExSameDay = 1,
            FedExFirstOvernight = 2,
            FedExPriorityOvernight = 3,
            FedExStandardOvernight = 4,
            FedEx2DayAm = 30,
            FedEx2Day = 5,
            FedExExpressSaver = 6,
            FedExHawaiiNeighborIsland = 7,
            FedExGround = 8,
            FedexHomeDelivery = 9,
            FedExIntlNextFlight = 10,
            FedExIntlFirst = 11,
            FedExIntlPriority = 12,
            FedExIntlEconomy = 13,
            FedExGroundUsToCanada = 26,
            FedExUsToPuertoRico = 18,
            FedExExpressFreightUs = 19,
            FedExExpressIntlFreight = 25,
            FedExIntlPremiumSm = 23
        }        
    }
}
