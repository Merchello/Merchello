using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.ModelBinding;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines the Shipping Gateway abstract class
    /// </summary>
    public abstract class ShippingGatewayProviderBase : GatewayProviderBase, IShippingGatewayProvider        
    {
        private readonly IRuntimeCacheProvider _runtimeCache;

        protected ShippingGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider)
        {
            _runtimeCache = runtimeCacheProvider;
        }

        /// <summary>
        /// Creates an instance of a ship method (T) without persisting it to the database
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 
        /// ShipMethods should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>
        /// 
        /// </remarks>
        public abstract IGatewayShipMethod CreateShipMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name);
        
        /// <summary>
        /// Saves a shipmethod
        /// </summary>
        /// <param name="shipMethod"></param>
        public abstract void SaveShipMethod(IGatewayShipMethod shipMethod);

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IGatewayResource> ListResourcesOffered();

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IGatewayShipMethod> GetActiveShipMethods(IShipCountry shipCountry);

        public virtual IEnumerable<IGatewayShipMethod> GetAvailableShipMethodsForDestination(IShipment shipment)
        {
                      
            var visitor = new ShimpmentWarehouseCatalogValidationVisitor();
            shipment.Items.Accept(visitor);

            // quick validation of shipment
            if (visitor.CatalogValidationStatus != ShimpmentWarehouseCatalogValidationVisitor.ShipmentCatalogValidationStatus.Ok)
            {
                LogHelper.Error<ShippingGatewayProviderBase>("ShipMethods could not be determined for Shipment passed to GetAvailableShipMethodsForDestination method. Validator returned: " + visitor.CatalogValidationStatus, new ArgumentException("merchWarehouseCatalogKey"));
                return new List<IGatewayShipMethod>();
            }

            var shipCountry = new ShipCountry(visitor.WarehouseCatalogKey, new Country(shipment.ToCountryCode));

            var shipmethods = GetActiveShipMethods(shipCountry);

            var gatewayShipMethods = shipmethods as IGatewayShipMethod[] ?? shipmethods.ToArray();
            if (!gatewayShipMethods.Any()) return new List<IGatewayShipMethod>();

            if (!shipCountry.HasProvinces) return gatewayShipMethods;

            var available = new List<IGatewayShipMethod>();
            foreach (var gwshipmethod in gatewayShipMethods)
            {
                var province = gwshipmethod.ShipMethod.Provinces.FirstOrDefault(x => x.Code == shipment.ToRegion);
                if (province == null)
                {
                    LogHelper.Debug<ShippingGatewayProviderBase>("Province code '" + shipment.ToRegion + "' was not found in ShipCountry with code : " + shipCountry.CountryCode);
                    available.Add(gwshipmethod);
                }
                else
                {
                    if(province.AllowShipping) available.Add(gwshipmethod);
                }
            }

            return available;
        }

        /// <summary>
        /// Gets the RuntimeCache
        /// </summary>
        /// <returns></returns>
        protected IRuntimeCacheProvider RuntimeCache
        {
            get { return _runtimeCache; }
        }
        
    }

}