using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Shipping.USPS.Models;
using Umbraco.Core.Cache;

namespace Merchello.Plugin.Shipping.USPS.Provider
{
    [GatewayProviderActivation("94BD59AE-0FA8-45E4-BD9A-3577C964851F", "USPS Shipping Provider", "USPS Shipping Provider")]
    [GatewayProviderEditor("USPS configuration", "~/App_Plugins/Merchello.USPS/editor.html")]
    public class UspsShippingGatewayProvider : ShippingGatewayProviderBase, IUspsShippingGatewayProvider
    {
        #region GatewayResources
        internal static readonly IEnumerable<GatewayResource> GatewayResources = new List<GatewayResource>()
        {
            new GatewayResource(Constants.ExtendedDataKeys.LibraryServiceCode, Constants.ExtendedDataKeys.LibraryServiceType), 														
            new GatewayResource(Constants.ExtendedDataKeys.MediaMailServiceCode, Constants.ExtendedDataKeys.MediaMailServiceType),										
            new GatewayResource(Constants.ExtendedDataKeys.BpmServiceCode, Constants.ExtendedDataKeys.BpmServiceType),								
            new GatewayResource(Constants.ExtendedDataKeys.StandardServiceCode, Constants.ExtendedDataKeys.StandardServiceType),								
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailServiceCode, Constants.ExtendedDataKeys.PriorityMailServiceType),								
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail1DayServiceCode, Constants.ExtendedDataKeys.PriorityMail1DayServiceType),								
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail2DayServiceCode, Constants.ExtendedDataKeys.PriorityMail2DayServiceType),							
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail3DayServiceCode, Constants.ExtendedDataKeys.PriorityMail3DayServiceType),						
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailDpoServiceCode, Constants.ExtendedDataKeys.PriorityMailDpoServiceType),					
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailMilitaryServiceCode, Constants.ExtendedDataKeys.PriorityMailMilitaryServiceType),					
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailFlatRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailFlatRateEnvelopeServiceType),						
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail1DayFlateRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMail1DayFlateRateEnvelopeServiceType),								
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail2DayFlateRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMail2DayFlateRateEnvelopeServiceType),							
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail3DayFlateRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMail3DayFlateRateEnvelopeServiceType),						
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailDpoFlateRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailDpoFlateRateEnvelopeServiceType),					
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailMilitaryFlateRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailMilitaryFlateRateEnvelopeServiceType),					
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailSmallFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailSmallFlateRateBoxServiceType),					
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail1DaySmallFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail1DaySmallFlateRateBoxServiceType),					
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail2DaySmallFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail2DaySmallFlateRateBoxServiceType),				
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail3DaySmallFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail3DaySmallFlateRateBoxServiceType),			
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailDpoSmallFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailDpoSmallFlateRateBoxServiceType),		
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailMilitarySmallFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailMilitarySmallFlateRateBoxServiceType),		
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailMediumFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailMediumFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail1DayMediumFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail1DayMediumFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail2DayMediumFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail2DayMediumFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail3DayMediumFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail3DayMediumFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailDpoMediumFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailDpoMediumFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailMilitaryMediumFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailMilitaryMediumFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailLargeFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailLargeFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail1DayLargeFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail1DayLargeFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail2DayLargeFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail2DayLargeFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMail3DayLargeFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMail3DayLargeFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailDpoLargeFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailDpoLargeFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailMilitaryLargeFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailMilitaryLargeFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailInternationalServiceCode, Constants.ExtendedDataKeys.PriorityMailInternationalServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailInternationalFlatRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailInternationalFlatRateEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailInternationalSmallFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailInternationalSmallFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailInternationalMediumFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailInternationalMediumFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailInternationalLargeFlateRateBoxServiceCode, Constants.ExtendedDataKeys.PriorityMailInternationalLargeFlateRateBoxServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FirstClassMailPackageServiceCode, Constants.ExtendedDataKeys.FirstClassMailPackageServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FirstClassMailLargeEnvelopeServiceCode, Constants.ExtendedDataKeys.FirstClassMailLargeEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FirstClassMailLetterServiceCode, Constants.ExtendedDataKeys.FirstClassMailLetterServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FirstClassMailInternationalLetterServiceCode, Constants.ExtendedDataKeys.FirstClassMailInternationalLetterServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FirstClassMailInternationalLargeEnvelopeServiceCode, Constants.ExtendedDataKeys.FirstClassMailInternationalLargeEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.FirstClassPackageInternationalServiceServiceCode, Constants.ExtendedDataKeys.FirstClassPackageInternationalServiceServiceType),							
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress1DayServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress1DayServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress2DayServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress2DayServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress3DayServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress3DayServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressDpoServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressDpoServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressMilitaryServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressMilitaryServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressFlatRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressFlatRateEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress1DayFlatRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress1DayFlatRateEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress2DayFlatRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress2DayFlatRateEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress3DayFlatRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress3DayFlatRateEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressDpoFlatRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressDpoFlatRateEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressMilitaryFlatRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressMilitaryFlatRateEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress1DayHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress1DayHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress2DayHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress2DayHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress3DayHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress3DayHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressDpoHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressDpoHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressMilitaryHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressMilitaryHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressFlatRateEnvelopeHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressFlatRateEnvelopeHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress1DayFlatRateEnvelopeHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress1DayFlatRateEnvelopeHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress2DayFlatRateEnvelopeHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress2DayFlatRateEnvelopeHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress3DayFlatRateEnvelopeHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress3DayFlatRateEnvelopeHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressDpoFlatRateEnvelopeHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressDpoFlatRateEnvelopeHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressMilitaryFlatRateEnvelopeHoldForPickupServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressMilitaryFlatRateEnvelopeHoldForPickupServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressSundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressSundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress1DaySundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress1DaySundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress2DaySundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress2DaySundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress3DaySundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress3DaySundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressDpoSundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressDpoSundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressMilitarySundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressMilitarySundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressFlatRateEnvelopeSundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressFlatRateEnvelopeSundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress1DayFlatRateEnvelopeSundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress1DayFlatRateEnvelopeSundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress2DayFlatRateEnvelopeSundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress2DayFlatRateEnvelopeSundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpress3DayFlatRateEnvelopeSundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpress3DayFlatRateEnvelopeSundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressDpoFlatRateEnvelopeSundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressDpoFlatRateEnvelopeSundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressMilitaryFlatRateEnvelopeSundayHolidayGuaranteeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressMilitaryFlatRateEnvelopeSundayHolidayGuaranteeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressInternationalServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressInternationalServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.PriorityMailExpressInternationalFlateRateEnvelopeServiceCode, Constants.ExtendedDataKeys.PriorityMailExpressInternationalFlateRateEnvelopeServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.GlobalExpressGuaranteedServiceCode, Constants.ExtendedDataKeys.GlobalExpressGuaranteedServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.GlobalExpressGuaranteedNonDocumentRectangularServiceCode, Constants.ExtendedDataKeys.GlobalExpressGuaranteedNonDocumentRectangularServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.GlobalExpressGuaranteedNonDocumentNonRectangularServiceCode, Constants.ExtendedDataKeys.GlobalExpressGuaranteedNonDocumentNonRectangularServiceType),
            new GatewayResource(Constants.ExtendedDataKeys.GxgEnvelopesServiceCode, Constants.ExtendedDataKeys.GxgEnvelopesServiceType)
        };
        #endregion

        private IRuntimeCacheProvider _runtimeCache;
        public UspsShippingGatewayProvider(IGatewayProviderService gatewayProviderService, 
            IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) : 
            base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
            _runtimeCache = runtimeCacheProvider;
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return GatewayResources;
        }

        public override IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource, IShipCountry shipCountry,
            string name)
        {
            var attempt = GatewayProviderService.CreateShipMethodWithKey(GatewayProviderSettings.Key, shipCountry, name,
                gatewayResource.ServiceCode);

            if (!attempt.Success) throw attempt.Exception;

            return new UspsShippingGatewayMethod(gatewayResource, attempt.Result, shipCountry, GatewayProviderSettings, _runtimeCache);
        }

        public override void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod)
        {                                                                
            GatewayProviderService.Save(shippingGatewayMethod.ShipMethod);
        }

        public override IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry)
        {
            var methods = GatewayProviderService.GetShipMethodsByShipCountryKey(GatewayProviderSettings.Key, shipCountry.Key);
            return methods
                .Select(                                                         
                    shipMethod =>
                        new UspsShippingGatewayMethod(
                            GatewayResources.FirstOrDefault(x => shipMethod.ServiceCode.StartsWith(x.ServiceCode)),
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
