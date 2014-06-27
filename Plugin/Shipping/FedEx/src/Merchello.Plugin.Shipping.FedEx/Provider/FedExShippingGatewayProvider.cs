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
        public const string FedEx2Day = "1";
        public const string FedEx2DayAm = "2";
        public const string FedExExpressSaver = "3";
        public const string FedExFirstOvernight = "4";
        public const string FedExPriorityOvernight = "5";
        public const string FedExStandardOvernight = "6";       
        public const string FedEx1DayFreight = "7";
        public const string FedEx2DayFreight = "8";
        public const string FedEx3DayFreight = "9";
        public const string FedExFirstFreight = "10";
        public const string FedExInternationalDistributionFreight = "11";
        public const string FedExEconomyFreight = "12";
        public const string FedExInternationalEconomy = "13";
        public const string FedExInternationalEconomyDistribution = "14";
        public const string FedExInternationalFirst = "15";
        public const string FedExInternationalPriority = "16";
        public const string FedExInternationalPriorityDistribution = "17";
        public const string FedExInternationalPriorityFreight = "18";
        public const string FedExEuropeFirstInternationalPriority = "19";
        

        private FedExProcessorSettings _settings;

        public FedExShippingGatewayProvider(IGatewayProviderService gatewayProviderService,
            IGatewayProviderSettings gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        {
            _settings = new FedExProcessorSettings();
        }

        // In this case, the GatewayResource can be used to create multiple shipmethods of the same resource type.
        internal static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>()
        {
            
            new GatewayResource(FedEx2Day, "FedEx 2Day®"),     
            new GatewayResource(FedEx2DayAm, "FedEx 2Day®A.M."),     
            new GatewayResource(FedExExpressSaver, "FedEx Express Saver®"),        
            new GatewayResource(FedExFirstOvernight, "FedEx First Overnight®"),
            new GatewayResource(FedExPriorityOvernight, "FedEx Priority Overnight®"),
            new GatewayResource(FedExStandardOvernight, "FedEx Standard Overnight®"),  
            new GatewayResource(FedExInternationalEconomy, "FedEx International Economy"),
            new GatewayResource(FedExInternationalEconomyDistribution, "FedEx International Distribution"),
            new GatewayResource(FedExInternationalFirst, "FedEx International First"),
            new GatewayResource(FedExInternationalPriority, "FedEx International Priority"),
            new GatewayResource(FedExInternationalPriorityDistribution, "FedEx International Priority Distribution"),
            new GatewayResource(FedExEuropeFirstInternationalPriority, "FedEx Europe First International Priority"),
            new GatewayResource(FedEx1DayFreight, "FedEx 1 Day Freight"),
            new GatewayResource(FedEx2DayFreight, "FedEx 2 Day Freight"),
            new GatewayResource(FedEx3DayFreight, "FedEx 3 Day Freight"),
            new GatewayResource(FedExFirstFreight, "FedEx First Freight"),        
            new GatewayResource(FedExInternationalDistributionFreight, "FedEx International Distribution Freight"),
            new GatewayResource(FedExEconomyFreight, "FedEx Economy Freight"),                                       
            new GatewayResource(FedExInternationalPriorityFreight, "FedEx International Priority Freight"),
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
                case FedEx2Day:
                    resource = AvailableResources.First(x => x.ServiceCode == "1");
                    break;
                case FedEx2DayAm:
                    resource = AvailableResources.First(x => x.ServiceCode ==  "2");
                    break;
                case FedExExpressSaver:
                     resource = AvailableResources.First(x => x.ServiceCode == "3");        
                    break;
                case FedExFirstOvernight:
                     resource = AvailableResources.First(x => x.ServiceCode == "4");     
                    break;
                case FedExPriorityOvernight:
                    resource = AvailableResources.First(x => x.ServiceCode == "5");     
                    break;
                case FedExStandardOvernight:
                    resource = AvailableResources.First(x => x.ServiceCode == "6");     
                    break;  
                case FedEx1DayFreight:
                    resource = AvailableResources.First(x => x.ServiceCode == "7");           
                    break;
                case FedEx2DayFreight:
                    resource = AvailableResources.First(x => x.ServiceCode == "8");           
                    break;
                case FedEx3DayFreight:
                    resource = AvailableResources.First(x => x.ServiceCode == "9");           
                    break;
                case FedExFirstFreight:
                    resource = AvailableResources.First(x => x.ServiceCode == "10");          
                    break;
                case FedExInternationalDistributionFreight:
                    resource = AvailableResources.First(x => x.ServiceCode == "11");    
                    break;
                case FedExEconomyFreight:
                    resource = AvailableResources.First(x => x.ServiceCode == "12");                             
                    break;
                case FedExInternationalEconomy:
                    resource = AvailableResources.First(x => x.ServiceCode == "13");                       
                    break;
                case FedExInternationalEconomyDistribution:
                    resource = AvailableResources.First(x => x.ServiceCode == "14");               
                    break;
                case FedExInternationalFirst:
                    resource = AvailableResources.First(x => x.ServiceCode == "15");                   
                    break;
                case FedExInternationalPriority:
                    resource = AvailableResources.First(x => x.ServiceCode == "16");                
                    break;
                case FedExInternationalPriorityDistribution:
                    resource = AvailableResources.First(x => x.ServiceCode == "17");     
                    break;
                case FedExInternationalPriorityFreight:
                    resource = AvailableResources.First(x => x.ServiceCode == "18");       
                    break;
                case FedExEuropeFirstInternationalPriority:
                    resource = AvailableResources.First(x => x.ServiceCode == "19");   
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
                gatewayResource.ServiceCode + string.Format("-{0}", Guid.NewGuid()));

            if (!attempt.Success) throw attempt.Exception;

            return new FedExShippingGatewayMethod(gatewayResource, attempt.Result, shipCountry, new ExtendedDataCollection());
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
                            new ExtendedDataCollection())
                ).OrderBy(x => x.ShipMethod.Name);
        }
    }
}
