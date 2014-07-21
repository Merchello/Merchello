using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Plugin.Shipping.UPS.Provider
{
    [GatewayProviderActivation("AEB14625-B9DE-4DE8-9C92-99204D340342", "UPS Shipping Provider", "UPS Shipping Provider")]
    
    [GatewayProviderEditor("UPS configuration", "~/App_Plugins/Merchello.UPS/editor.html")]
    public class UPSShippingGatewayProvider : ShippingGatewayProviderBase, IUPSShippingGatewayProvider
    {
        public const string UPSNextDayAir = "01";
        public const string UPS2ndDayAir = "02";
        public const string UPSGround = "03";
        public const string UPSWorldwideExpress = "07";
        public const string UPSWorldwideExpidited = "08";
        public const string UPSStandard = "11";
        public const string UPS3DaySelect = "12";
        public const string UPSNextDayAirSaver = "13";
        public const string UPSNextDayAirEarlyAM = "14";
        public const string UPSWorldwideExpressPlus = "54";
        public const string UPS2ndDayAirAM = "59";
                                                          
        public UPSShippingGatewayProvider(IGatewayProviderService gatewayProviderService,
            IGatewayProviderSettings gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        {

        }

        // In this case, the GatewayResource can be used to create multiple shipmethods of the same resource type.
        internal static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>()
        {
            //new GatewayResource(Constants.ExtendedDataKeys.NextDayAirEarlyAmServiceCode, Constants.ExtendedDataKeys.NextDayAirEarlyAmServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.NextDayAirServiceCode, Constants.ExtendedDataKeys.NextDayAirServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.NextDayAirSaverServiceCode, Constants.ExtendedDataKeys.NextDayAirSaverServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.SecondDayAirAmServiceCode, Constants.ExtendedDataKeys.SecondDayAirAmServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.SecondDayAirServiceCode, Constants.ExtendedDataKeys.SecondDayAirServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.ThirdDaySelectServiceCode, Constants.ExtendedDataKeys.ThirdDaySelectServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.GroundServiceCode, Constants.ExtendedDataKeys.GroundServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.WorldwideExpressPlusServiceCode, Constants.ExtendedDataKeys.WorldwideExpressPlusServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.WorldwideExpressServiceCode, Constants.ExtendedDataKeys.WorldwideExpressServiceType),
            //new GatewayResource(Constants.ExtendedDataKeys.WorldwideSaverExpressServiceCode, Constants.ExtendedDataKeys.WorldwideSaverExpressServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.WorldwideExpeditedServiceCode, Constants.ExtendedDataKeys.WorldwideExpeditedServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.StandardServiceCode, Constants.ExtendedDataKeys.StandardServiceType),
            //new GatewayResource(Constants.ExtendedDataKeys.UPSExpressServiceCode, Constants.ExtendedDataKeys.UPSExpressServiceType)
        };

        /// <summary>
        /// Creates an instance of a <see cref="UPSShippingGatewayMethod"/>
        /// </summary>     
        /// <remarks>
        /// 
        /// This method is really specific to the RateTableShippingGateway due to the odd fact that additional shipmethods can be created 
        /// rather than defined up front.  
        /// 
        /// </remarks>   
        public IShippingGatewayMethod CreateShipMethod(UPSShippingGatewayMethod.UPSType upsType,
            IShipCountry shipCountry, string name)
        {
            var resource = AvailableResources.First();
            switch (upsType.ToString())
            {
                case UPSShippingGatewayProvider.UPSNextDayAir:
                    resource = AvailableResources.First(x => x.ServiceCode == "01");
                    break;
                case UPSShippingGatewayProvider.UPS2ndDayAir:
                    resource = AvailableResources.First(x => x.ServiceCode == "02");
                    break;
                case UPSShippingGatewayProvider.UPSGround:
                    resource = AvailableResources.First(x => x.ServiceCode == "03");
                    break;
                case UPSShippingGatewayProvider.UPSWorldwideExpress:
                    resource = AvailableResources.First(x => x.ServiceCode == "07");
                    break;
                case UPSShippingGatewayProvider.UPSWorldwideExpidited:
                    resource = AvailableResources.First(x => x.ServiceCode == "08");
                    break;
                case UPSShippingGatewayProvider.UPSStandard:
                    resource = AvailableResources.First(x => x.ServiceCode == "11");
                    break;
                case UPSShippingGatewayProvider.UPS3DaySelect:
                    resource = AvailableResources.First(x => x.ServiceCode == "12");
                    break;
                case UPSShippingGatewayProvider.UPSNextDayAirSaver:
                    resource = AvailableResources.First(x => x.ServiceCode == "13");
                    break;
                case UPSShippingGatewayProvider.UPSNextDayAirEarlyAM:
                    resource = AvailableResources.First(x => x.ServiceCode == "14");
                    break;
                case UPSShippingGatewayProvider.UPSWorldwideExpressPlus:
                    resource = AvailableResources.First(x => x.ServiceCode == "54");
                    break;
                case UPSShippingGatewayProvider.UPS2ndDayAirAM:
                    resource = AvailableResources.First(x => x.ServiceCode == "59");
                    break;
            }

            return CreateShippingGatewayMethod(resource, shipCountry, name);
        }

        /// <summary>
        /// Creates an instance of a <see cref="UPSShippingGatewayMethod"/>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// GatewayShipMethods (in general) should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>.  However, this is a
        /// a provider is sort of a unique case, sense we want to be able to add as many ship methods with rate tables as needed in order to facilitate 
        /// tiered rate tables for various ship methods without requiring a carrier based shipping provider.
        /// 
        /// </remarks>    
        public override IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource,
            IShipCountry shipCountry, string name)
        {

            //Mandate.ParameterNotNull(gatewayResource, "gatewayResource");
            //Mandate.ParameterNotNull(shipCountry, "shipCountry");
            //Mandate.ParameterNotNullOrEmpty(name, "name");

            var attempt = GatewayProviderService.CreateShipMethodWithKey(GatewayProviderSettings.Key, shipCountry, name,
                gatewayResource.ServiceCode);

            if (!attempt.Success) throw attempt.Exception;

            return new UPSShippingGatewayMethod(gatewayResource, attempt.Result, shipCountry, GatewayProviderSettings, RuntimeCache);
        }

        /// <summary>
        /// Saves a <see cref="UPSShippingGatewayMethod"/> 
        /// </summary>
        /// <param name="shippingGatewayMethod"></param>
        public override void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod)
        {
            GatewayProviderService.Save(shippingGatewayMethod.ShipMethod);
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            // PaymentMethods is created in PaymentGatewayProviderBase.  It is a list of all previously saved payment methods
            return AvailableResources;
        }

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry)
        {
            var methods = GatewayProviderService.GetShipMethodsByShipCountryKey(GatewayProviderSettings.Key, shipCountry.Key);
            return methods
                .Select(
                    shipMethod =>
                        new UPSShippingGatewayMethod(
                            AvailableResources.FirstOrDefault(x => shipMethod.ServiceCode.StartsWith(x.ServiceCode)),
                            shipMethod, shipCountry,
                            GatewayProviderSettings, RuntimeCache)
                ).OrderBy(x => x.ShipMethod.Name);
        }

        public override IEnumerable<IShippingGatewayMethod> GetShippingGatewayMethodsForShipment(IShipment shipment)
        {
            var methods = base.GetShippingGatewayMethodsForShipment(shipment);

            var shippingMethods = new List<IShippingGatewayMethod>();
            foreach (var method in methods)
            {
                var quote = method.QuoteShipment(shipment);

                if (quote.Result.Rate > (decimal)0.00)
                {
                    shippingMethods.Add(method);
                }
            }

            return shippingMethods;
        }
    }
}
