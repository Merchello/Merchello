namespace Merchello.Core.Gateways.Shipping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Models;
    using Services;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Defines the Shipping Gateway abstract class
    /// </summary>
    public abstract class ShippingGatewayProviderBase : GatewayProviderBase, IShippingGatewayProvider        
    {
        /// <summary>
        /// The ship methods.
        /// </summary>
        private IEnumerable<IShipMethod> _shipMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingGatewayProviderBase"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="gatewayProviderSettings">
        /// The gateway provider settings.
        /// </param>
        /// <param name="runtimeCacheProvider">
        /// The runtime cache provider.
        /// </param>
        protected ShippingGatewayProviderBase(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {            
        }

        /// <summary>
        /// Gets or sets the collection of all <see cref="IShipMethod"/> assoicated with this provider
        /// </summary>
        public IEnumerable<IShipMethod> ShipMethods
        {
            get
            {
                return _shipMethods ??
                       (_shipMethods = GatewayProviderService.GetShipMethodsByShipCountryKey(GatewayProviderSettings.Key));
            }

            protected set
            {
                _shipMethods = value;
            }
        }

        /// <summary>
        /// Creates an instance of a ship method (T) without persisting it to the database
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
        /// The newly created <see cref="IShippingGatewayMethod"/>
        /// </returns>
        /// <remarks>
        /// 
        /// ShipMethods should be unique with respect to <see cref="IShipCountry"/> and <see cref="IGatewayResource"/>
        /// 
        /// </remarks>
        public abstract IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource, IShipCountry shipCountry, string name);
        
        /// <summary>
        /// Saves a shipmethod
        /// </summary>
        /// <param name="shippingGatewayMethod">The <see cref="IShippingGatewayMethod"/> to be saved</param>
        public abstract void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod);
        
        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <returns>
        /// A collection of all <see cref="IShippingGatewayMethod"/> for shipCountry
        /// </returns>
        public abstract IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry);


        /// <summary>
        /// Gets a <see cref="IShippingGatewayMethod"/> by it's <see cref="IShipMethod"/> key
        /// </summary>
        /// <param name="shipMethodKey">The <see cref="IShipMethod"/> key</param>
        /// <param name="shipCountrKey">The <see cref="IShipCountry"/> ky</param>
        /// <returns>The <see cref="IShippingGatewayMethod"/></returns>
        public IShippingGatewayMethod GetShippingGatewayMethod(Guid shipMethodKey, Guid shipCountrKey)
        {
            return
                GetAllShippingGatewayMethodsForShipCountry(shipCountrKey)
                    .FirstOrDefault(x => x.ShipMethod.Key == shipMethodKey);
        }

        /// <summary>
        /// Returns a collection of ship methods assigned for this specific provider configuration (associated with the ShipCountry)
        /// </summary>
        /// <param name="shipCountryKey">The key for the <see cref="IShipCountry"/></param>
        /// <returns>
        /// A collection of <see cref="IShippingGatewayMethod"/>
        /// </returns>
        public IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethodsForShipCountry(Guid shipCountryKey)
        {
            var shipCountry = GatewayProviderService.GetShipCountryByKey(shipCountryKey);

            return GetAllShippingGatewayMethods(shipCountry);
        }

        /// <summary>
        /// Deletes an Active ShipMethod
        /// </summary>
        /// <param name="shippingGatewayMethod">The shipping gateway method</param>
        public virtual void DeleteShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod)
        {
            GatewayProviderService.Delete(shippingGatewayMethod.ShipMethod);
            _shipMethods = null;
        }        
        
        /// <summary>
        /// Returns a collection of available <see cref="IShippingGatewayMethod"/> associated by this provider for a given shipment
        /// </summary>
        /// <param name="shipment">the <see cref="IShipment"/></param>
        /// <returns>A collection of <see cref="IShippingGatewayMethod"/></returns>
        public virtual IEnumerable<IShippingGatewayMethod> GetShippingGatewayMethodsForShipment(IShipment shipment)
        {
            var attempt = shipment.GetValidatedShipCountry(GatewayProviderService);

            // quick validation of shipment
            if (!attempt.Success)
            {
                LogHelper.Error<ShippingGatewayProviderBase>("ShipMethods could not be determined for Shipment passed to GetAvailableShipMethodsForDestination method. Attempt message: " + attempt.Exception.Message, new ArgumentException("merchWarehouseCatalogKey"));
                return new List<IShippingGatewayMethod>();
            }
            
            var shipCountry = attempt.Result;

            var shipmethods = GetAllShippingGatewayMethods(shipCountry);

            var gatewayShipMethods = shipmethods as IShippingGatewayMethod[] ?? shipmethods.ToArray();
            if (!gatewayShipMethods.Any()) return new List<IShippingGatewayMethod>();

            if (!shipCountry.HasProvinces) return gatewayShipMethods;

            var available = new List<IShippingGatewayMethod>();
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
                    if (province.AllowShipping) available.Add(gwshipmethod);
                }
            }

            return available;
        }

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>
        /// <param name="shipment">The <see cref="IShipmentRateQuote"/></param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public virtual IEnumerable<IShipmentRateQuote> QuoteShippingGatewayMethodsForShipment(IShipment shipment)
        {
            var gatewayShipMethods = GetShippingGatewayMethodsForShipment(shipment);

            var ctrValues = new object[] { shipment, gatewayShipMethods.ToArray(), RuntimeCache };

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultShipmentRateQuote).Type;
            
            var attempt = ActivatorHelper.CreateInstance<ShipmentRateQuoteStrategyBase>(typeName, ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<ShippingGatewayProviderBase>("Failed to instantiate strategy " + typeName, attempt.Exception);
                throw attempt.Exception;
            }

            return QuoteShippingGatewayMethodsForShipment(attempt.Result);
        }

        /// <summary>
        /// Quotes a single GatewayShipMethod for a shipment rate
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> used to generate the rate quote</param>
        /// <param name="shippingGatewayMethod">The <see cref="IShippingGatewayMethod"/> used to generate the rate quote</param>
        /// <returns>The <see cref="IShipmentRateQuote"/></returns>
        public virtual IShipmentRateQuote QuoteShipMethodForShipment(IShipment shipment, IShippingGatewayMethod shippingGatewayMethod)
        {
            var ctrValues = new object[] { shipment, new[] { shippingGatewayMethod }, RuntimeCache };

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultShipmentRateQuote).Type;

            var attempt = ActivatorHelper.CreateInstance<ShipmentRateQuoteStrategyBase>(typeName, ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<ShippingGatewayProviderBase>("Failed to instantiate strategy " + typeName, attempt.Exception);
                throw attempt.Exception;
            }

            return QuoteShippingGatewayMethodsForShipment(attempt.Result).FirstOrDefault();
        }

        /// <summary>
        /// Returns a collection of all available <see cref="IShipmentRateQuote"/> for a given shipment
        /// </summary>
        /// <param name="strategy">The quotation strategy</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public IEnumerable<IShipmentRateQuote> QuoteShippingGatewayMethodsForShipment(ShipmentRateQuoteStrategyBase strategy)
        {
            return strategy.GetShipmentRateQuotes();
        }

        /// <summary>
        /// Deletes all active shipMethods
        /// </summary>
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal virtual void DeleteAllActiveShipMethods(IShipCountry shipCountry)
        {
            foreach (var gatewayShipMethod in GetAllShippingGatewayMethods(shipCountry))
            {
                DeleteShippingGatewayMethod(gatewayShipMethod);
            }
        }

        /// <summary>
        /// The reset ship methods.
        /// </summary>
        internal virtual void ResetShipMethods()
        {
            _shipMethods = null;
        }
    }
}