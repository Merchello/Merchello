namespace Merchello.Core.Gateways.Shipping.FixedRate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Services;
    using Umbraco.Core.Cache;

    /// <summary>
    /// Defines the RateTableLookupGateway
    /// </summary>
    /// <remarks>
    /// 
    /// This is Merchello's default ShippingGatewayProvider
    /// 
    /// </remarks>
    [GatewayProviderActivation("AEC7A923-9F64-41D0-B17B-0EF64725F576", "Fixed Rate Shipping Provider", "Fixed Rate Shipping Provider")]
    [GatewayProviderEditor("Fixed Rate Shipping Provider", "Fixed Rate Shipping Provider", "~/App_Plugins/Merchello/Modules/Settings/Shipping/Dialogs/shippingmethod.html")]
    public class FixedRateShippingGatewayProvider : ShippingGatewayProviderBase, IFixedRateShippingGatewayProvider
    {
        #region "Available Methods"

        public static string VaryByWeightPrefix = "VBW";
        public static string PercentOfTotalPrefix = "VBP";

        // In this case, the GatewayResource can be used to create multiple shipmethods of the same resource type.
        private static readonly IEnumerable<IGatewayResource> AvailableResources  = new List<IGatewayResource>()
            {
                new GatewayResource(VaryByWeightPrefix, "Vary by Weight"),
                new GatewayResource(PercentOfTotalPrefix, "Vary by Price")
            };

        #endregion


        public FixedRateShippingGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {            
        }

        /// <summary>
        /// Creates an instance of a <see cref="FixedRateShippingGatewayMethod"/>
        /// </summary>
        /// <param name="quoteType">
        /// The quote Type.
        /// </param>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <remarks>
        /// 
        /// This method is really specific to the RateTableShippingGateway due to the odd fact that additional shipmethods can be created 
        /// rather than defined up front.  
        /// 
        /// </remarks>
        /// <returns>
        /// The <see cref="IShippingGatewayMethod"/> created
        /// </returns>
        public IShippingGatewayMethod CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType quoteType, IShipCountry shipCountry, string name)
        {
            var resource = quoteType == FixedRateShippingGatewayMethod.QuoteType.VaryByWeight
                ? AvailableResources.First(x => x.ServiceCode == "VBW")
                : AvailableResources.First(x => x.ServiceCode == "VBP");

            return CreateShippingGatewayMethod(resource, shipCountry, name);
        }

        /// <summary>
        /// Creates an instance of a <see cref="FixedRateShippingGatewayMethod"/>
        /// </summary>
        /// <param name="gatewayResource">
        /// The gateway Resource.
        /// </param>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IShippingGatewayMethod"/> created
        /// </returns>
        /// <remarks>
        /// 
        /// GatewayShipMethods (in general) should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>.  However, this is a
        /// a provider is sort of a unique case, sense we want to be able to add as many ship methods with rate tables as needed in order to facilitate 
        /// tiered rate tables for various ship methods without requiring a carrier based shipping provider.
        /// 
        /// </remarks>
        public override IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name)
        {
            Mandate.ParameterNotNull(gatewayResource, "gatewayResource");
            Mandate.ParameterNotNull(shipCountry, "shipCountry");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            var attempt = GatewayProviderService.CreateShipMethodWithKey(GatewayProviderSettings.Key, shipCountry, name, gatewayResource.ServiceCode + string.Format("-{0}", Guid.NewGuid()));
            
            if (!attempt.Success) throw attempt.Exception;

            return new FixedRateShippingGatewayMethod(gatewayResource, attempt.Result, shipCountry);
        }

        /// <summary>
        /// Saves a <see cref="FixedRateShippingGatewayMethod"/> 
        /// </summary>
        /// <param name="shippingGatewayMethod">The <see cref="IShippingGatewayMethod"/> to be saved</param>
        public override void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod)
        {
            GatewayProviderService.Save(shippingGatewayMethod.ShipMethod);
            ShippingFixedRateTable.Save(GatewayProviderService, RuntimeCache, ((FixedRateShippingGatewayMethod) shippingGatewayMethod).RateTable);
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns>
        /// Returns the collection of <see cref="IGatewayResource"/> defined by this provider
        /// </returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return AvailableResources;
        }

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <returns>
        /// Returns a collection of all <see cref="IShippingGatewayMethod"/>s associated with the ship country
        /// </returns>
        public override IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry)
        {
            var methods = GatewayProviderService.GetShipMethodsByShipCountryKey(GatewayProviderSettings.Key, shipCountry.Key);
            return methods
                .Select(
                shipMethod => new FixedRateShippingGatewayMethod(AvailableResources.FirstOrDefault(x => shipMethod.ServiceCode.StartsWith(x.ServiceCode)), shipMethod, shipCountry, ShippingFixedRateTable.GetShipRateTable(GatewayProviderService, RuntimeCache, shipMethod.Key)))
                .OrderBy(x => x.ShipMethod.Name);
        }
    }
}