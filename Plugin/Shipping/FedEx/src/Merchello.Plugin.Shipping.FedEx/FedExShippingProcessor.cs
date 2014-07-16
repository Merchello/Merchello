using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Plugin.Shipping.FedEx.Models;
using RateAvailableServiceWebServiceClient.RateServiceWebReference;
using Umbraco.Core;

namespace Merchello.Plugin.Shipping.FedEx
{
    public class FedExShippingProcessor
    {
        private readonly FedExProcessorSettings _settings;

        public FedExShippingProcessor(FedExProcessorSettings settings)
        {
            _settings = settings;
        }

        private NameValueCollection GetInitialRequestForm(string currencyCode)
        {
            return new NameValueCollection()
            {
                { "x_fedex_access_key", _settings.FedExKey },
                { "x_fedex_password", _settings.FedExPassword }              
            };
        }

        /// <summary>
        /// Gets the FedEx Url
        /// </summary>
        private string GetFedExUrl()
        {
            return _settings.UseSandbox
                ? "https://wsbeta.fedex.com:443/web-services/rate"
                : "https://wsbeta.fedex.com:443/web-services/rate";
        }



        /// <summary>
        /// The FedEx API version
        /// </summary>
        public static string ApiVersion
        {
            get { return "14"; }
        }

        public Attempt<IShipmentRateQuote> CalculatePrice(IShipment shipment, IShipMethod shipMethod, decimal totalWeight, int quantity, IShipProvince province)
        {
            // First sum up the total weight for the shipment.
            // We're assumning that a custom order line property 
            // was set on the order line prior when the product was added to the order line.

            var shippingPrice = 0M;
            try
            {
                var service = new RateService {Url = "https://wsbeta.fedex.com:443/web-services/rate"};
                
                var reply = service.getRates(RateRequest(shipment, totalWeight, quantity));
                                                            
                if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING)
                {
                    var collection = BuildDeliveryOptions(reply, shipment);

                    var firstCarrierRate = collection.FirstOrDefault(option => option.Service.Contains(shipMethod.ServiceCode.Split('-').First()));
                    if (firstCarrierRate != null)
                        shippingPrice = firstCarrierRate.Rate;
                }

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

            request.ClientDetail = new ClientDetail {AccountNumber = "510087569", MeterNumber = "118628262"};
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
                        StreetLines = new [] {shipment.FromAddress1, shipment.FromAddress2},
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

                optionCollection.AddRange(rateReplyDetail.RatedShipmentDetails.Select(shipmentDetail => shipmentDetail.ShipmentRateDetail.TotalNetCharge.Amount).Select(rate => new DeliveryOption {Rate = rate, Service = service}));
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
    }
}
