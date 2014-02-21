using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
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
        
        protected ShippingGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

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
        /// <param name="gatewayShipMethod"></param>
        public abstract void SaveShipMethod(IGatewayShipMethod gatewayShipMethod);

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

        /// <summary>
        /// Deletes an Active ShipMethod
        /// </summary>
        /// <param name="gatewayShipMethod"></param>
        public virtual void DeleteActiveShipMethod(IGatewayShipMethod gatewayShipMethod)
        {
            GatewayProviderService.Delete(gatewayShipMethod.ShipMethod);
        }

        /// <summary>
        /// Deletes all active shipMethods
        /// </summary>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal virtual void DeleteAllActiveShipMethods(IShipCountry shipCountry)
        {
            foreach (var gatewayShipMethod in GetActiveShipMethods(shipCountry))
            {
                DeleteActiveShipMethod(gatewayShipMethod);
            }
        }


        

        /// <summary>
        /// Returns a collection of available <see cref="IGatewayShipMethod"/> associated by this provider for a given shipment
        /// </summary>
        /// <param name="shipment"><see cref="IShipment"/></param>
        /// <returns>A collection of <see cref="IGatewayShipMethod"/></returns>
        public virtual IEnumerable<IGatewayShipMethod> GetAvailableShipMethodsForShipment(IShipment shipment)
        {

            var attempt = shipment.GetValidatedShipCountry(GatewayProviderService);

            // quick validation of shipment
            if (!attempt.Success)
            {
                LogHelper.Error<ShippingGatewayProviderBase>("ShipMethods could not be determined for Shipment passed to GetAvailableShipMethodsForDestination method. Attempt message: " + attempt.Exception.Message, new ArgumentException("merchWarehouseCatalogKey"));
                return new List<IGatewayShipMethod>();
            }
            
            var shipCountry = attempt.Result;

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
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>
        /// <param name="shipment"><see cref="IShipmentRateQuote"/></param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public virtual IEnumerable<IShipmentRateQuote> QuoteAvailableShipMethodsForShipment(IShipment shipment)
        {
            var gatewayShipMethods = GetAvailableShipMethodsForShipment(shipment);

            var ctrValues = new object[] {shipment, gatewayShipMethods.ToArray(), RuntimeCache};

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultShipmentRateQuote).Type;
            
            var attempt = ActivatorHelper.CreateInstance<ShipmentRateQuoteStrategyBase>(typeName, ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<ShippingGatewayProviderBase>("Failed to instantiate strategy " + typeName, attempt.Exception);
                throw attempt.Exception;
            }

            return QuoteAvailableShipMethodsForShipment(attempt.Result);
        }

        /// <summary>
        /// Quotes a single GatewayShipMethod for a shipment rate
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> used to generate the rate quote</param>
        /// <param name="gatewayShipMethod">The <see cref="IGatewayShipMethod"/> used to generate the rate quote</param>
        /// <returns>The <see cref="IShipmentRateQuote"/></returns>
        public virtual IShipmentRateQuote QuoteShipMethodForShipment(IShipment shipment, IGatewayShipMethod gatewayShipMethod)
        {
            var ctrValues = new object[] { shipment, new[] { gatewayShipMethod }, RuntimeCache };

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultShipmentRateQuote).Type;

            var attempt = ActivatorHelper.CreateInstance<ShipmentRateQuoteStrategyBase>(typeName, ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<ShippingGatewayProviderBase>("Failed to instantiate strategy " + typeName, attempt.Exception);
                throw attempt.Exception;
            }

            return QuoteAvailableShipMethodsForShipment(attempt.Result).FirstOrDefault();
        }

        

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>
        /// <param name="strategy">The quotation strategy</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public IEnumerable<IShipmentRateQuote> QuoteAvailableShipMethodsForShipment(ShipmentRateQuoteStrategyBase strategy)
        {
            return strategy.GetShipmentRateQuotes();
        }

        private IEnumerable<IShipMethod> _shipMethods;

        /// <summary>
        /// Gets the collection of all <see cref="IShipMethod"/> assoicated with this provider
        /// </summary>
        public IEnumerable<IShipMethod> ShipMethods
        {
            get {
                return _shipMethods ??
                       (_shipMethods = GatewayProviderService.GetGatewayProviderShipMethods(GatewayProvider.Key));
            }
        }
    }

}