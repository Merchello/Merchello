using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Shipping.FedEx.Models;
using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Merchello.Plugin.Shipping.FedEx.Provider
{
    [GatewayProviderActivation("646d3ea7-3b31-45c1-9488-7c0449a564a6", "FedEx Shipping Provider", "FedEx Shipping Provider")]
    [GatewayProviderEditor("FedEx configuration", "~/App_Plugins/Merchello.FedEx/editor.html")]
    public class FedExShippingGatewayProvider : ShippingGatewayProviderBase, IFedExShippingGatewayProvider
    {
        private FedExProcessorSettings _settings;
        private IRuntimeCacheProvider _runtimeCache;

        public FedExShippingGatewayProvider(IGatewayProviderService gatewayProviderService,
            IGatewayProviderSettings gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        {
            _settings = new FedExProcessorSettings();
            _runtimeCache = runtimeCacheProvider;
        }


        // In this case, the GatewayResource can be used to create multiple shipmethods of the same resource type.
        internal static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>()
        {                  
            new GatewayResource(Constants.ExtendedDataKeys.FedEx2DayServiceCode, Constants.ExtendedDataKeys.FedEx2DayServiceType),     
            new GatewayResource(Constants.ExtendedDataKeys.FedEx2DayAmServiceCode, Constants.ExtendedDataKeys.FedEx2DayAmServiceType),     
            new GatewayResource(Constants.ExtendedDataKeys.FedExExpressSaverServiceCode, Constants.ExtendedDataKeys.FedExExpressSaverServiceType),        
            new GatewayResource(Constants.ExtendedDataKeys.FedExFirstOvernightServiceCode, Constants.ExtendedDataKeys.FedExFirstOvernightServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExPriorityOvernightServiceCode, Constants.ExtendedDataKeys.FedExPriorityOvernightServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExStandardOvernightServiceCode, Constants.ExtendedDataKeys.FedExStandardOvernightServiceType),  
            new GatewayResource(Constants.ExtendedDataKeys.FedExInternationalEconomyServiceCode, Constants.ExtendedDataKeys.FedExInternationalEconomyServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExInternationalEconomyDistributionServiceCode, Constants.ExtendedDataKeys.FedExInternationalEconomyDistributionServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExInternationalFirstServiceCode, Constants.ExtendedDataKeys.FedExInternationalFirstServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExInternationalPriorityServiceCode, Constants.ExtendedDataKeys.FedExInternationalPriorityServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExInternationalPriorityDistributionServiceCode, Constants.ExtendedDataKeys.FedExInternationalPriorityDistributionServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExEuropeFirstInternationalPriorityServiceCode, Constants.ExtendedDataKeys.FedExEuropeFirstInternationalPriorityServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedEx1DayFreightServiceCode, Constants.ExtendedDataKeys.FedEx1DayFreightServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedEx2DayFreightServiceCode, Constants.ExtendedDataKeys.FedEx2DayFreightServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedEx3DayFreightServiceCode, Constants.ExtendedDataKeys.FedEx3DayFreightServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExFirstFreightServiceCode, Constants.ExtendedDataKeys.FedExFirstFreightServiceType),        
            new GatewayResource(Constants.ExtendedDataKeys.FedExInternationalDistributionFreightServiceCode, Constants.ExtendedDataKeys.FedExInternationalDistributionFreightServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FedExEconomyFreightServiceCode, Constants.ExtendedDataKeys.FedExEconomyFreightServiceType),                                       
            new GatewayResource(Constants.ExtendedDataKeys.FedExInternationalPriorityFreightServiceCode, Constants.ExtendedDataKeys.FedExInternationalPriorityFreightServiceType)
        };

        /// <summary>
        /// Creates an instance of a <see cref="FedExShippingGatewayMethod"/>
        /// </summary>     
        /// <remarks>
        /// 
        /// This method is really specific to the RateTableShippingGateway due to the odd fact that additional shipmethods can be created 
        /// rather than defined up front.  
        /// 
        /// </remarks>   
        public IShippingGatewayMethod CreateShipMethod(FedExShippingGatewayMethod.FedExType fedExType,
            IShipCountry shipCountry, string name)
        {
            var resource = AvailableResources.First();
            switch (fedExType.ToString())
            {
                case Constants.ExtendedDataKeys.FedEx2DayServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedEx2DayServiceCode);
                    break;
                case Constants.ExtendedDataKeys.FedEx2DayAmServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedEx2DayAmServiceCode);
                    break;
                case Constants.ExtendedDataKeys.FedExExpressSaverServiceCode:
                     resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExExpressSaverServiceCode);        
                    break;
                case Constants.ExtendedDataKeys.FedExFirstOvernightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExFirstOvernightServiceCode);     
                    break;
                case Constants.ExtendedDataKeys.FedExPriorityOvernightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExPriorityOvernightServiceCode);     
                    break;
                case Constants.ExtendedDataKeys.FedExStandardOvernightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExStandardOvernightServiceCode);     
                    break;
                case Constants.ExtendedDataKeys.FedEx1DayFreightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedEx1DayFreightServiceCode);           
                    break;
                case Constants.ExtendedDataKeys.FedEx2DayFreightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedEx2DayFreightServiceCode);           
                    break;
                case Constants.ExtendedDataKeys.FedEx3DayFreightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedEx3DayFreightServiceCode);           
                    break;
                case Constants.ExtendedDataKeys.FedExFirstFreightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExFirstFreightServiceCode);          
                    break;
                case Constants.ExtendedDataKeys.FedExInternationalDistributionFreightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExInternationalDistributionFreightServiceCode);    
                    break;
                case Constants.ExtendedDataKeys.FedExEconomyFreightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExEconomyFreightServiceCode);                             
                    break;
                case Constants.ExtendedDataKeys.FedExInternationalEconomyServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExInternationalEconomyServiceCode);                       
                    break;
                case Constants.ExtendedDataKeys.FedExInternationalEconomyDistributionServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExInternationalEconomyDistributionServiceCode);               
                    break;
                case Constants.ExtendedDataKeys.FedExInternationalFirstServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExInternationalFirstServiceCode);                   
                    break;
                case Constants.ExtendedDataKeys.FedExInternationalPriorityServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExInternationalPriorityServiceCode);                
                    break;
                case Constants.ExtendedDataKeys.FedExInternationalPriorityDistributionServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExInternationalPriorityDistributionServiceCode);     
                    break;
                case Constants.ExtendedDataKeys.FedExInternationalPriorityFreightServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExInternationalPriorityFreightServiceCode);       
                    break;
                case Constants.ExtendedDataKeys.FedExEuropeFirstInternationalPriorityServiceCode:
                    resource = AvailableResources.First(x => x.ServiceCode == Constants.ExtendedDataKeys.FedExEuropeFirstInternationalPriorityServiceCode);   
                    break;
            }

            return CreateShippingGatewayMethod(resource, shipCountry, name);
        }

        /// <summary>
        /// Creates an instance of a <see cref="FedExShippingGatewayMethod"/>
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

            return new FedExShippingGatewayMethod(gatewayResource, attempt.Result, shipCountry, GatewayProviderSettings, _runtimeCache);
        }

        /// <summary>
        /// Saves a <see cref="FedExShippingGatewayMethod"/> 
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
                        new FedExShippingGatewayMethod(
                            AvailableResources.FirstOrDefault(x => shipMethod.ServiceCode.StartsWith(x.ServiceCode)),
                            shipMethod, shipCountry,
                            GatewayProviderSettings, _runtimeCache)
                ).OrderBy(x => x.ShipMethod.Name);
        }

        public override IEnumerable<IShippingGatewayMethod> GetShippingGatewayMethodsForShipment(IShipment shipment)
        {
            var methods = base.GetShippingGatewayMethodsForShipment(shipment);

            var shippingMethods = new List<IShippingGatewayMethod>();
            foreach (var method in methods)
            {     
                var quote = method.QuoteShipment(shipment);
                             
                if (quote.Result.Rate > (decimal) 0.00)
                {
                    shippingMethods.Add(method);
                }
            }
                                    
            return shippingMethods;
        }
    }
}
