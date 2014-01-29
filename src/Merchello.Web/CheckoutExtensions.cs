using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Configuration;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models;
using Merchello.Web.Shipping.Packaging;
using Merchello.Web.Workflow;

namespace Merchello.Web
{
    /// <summary>
    /// Extension methods for the <see cref="IBasket"/>
    /// </summary>
    public static class CheckoutExtensions
    {
        /// <summary>
        /// Packages the current basket instantiation into a collection of <see cref="IShipment"/>s
        /// </summary>        
        /// <returns>A collection of <see cref="IShipment"/></returns>
        public static IEnumerable<IShipment> PackageBasket(this IBasket basket, IAddress destination)
        {
            return basket.PackageBasket(MerchelloContext.Current, destination);
        }

        internal static IEnumerable<IShipment> PackageBasket(this IBasket basket, IMerchelloContext merchelloContext, IAddress destination)
        {
            if (!basket.Items.Any()) return new List<IShipment>();

            // get the default strategy from the configuration file
            // TODO move this to a resolver
            var defaultStrategy = MerchelloConfiguration.Current.DefaultBasketPackagingStrategy;

            var ctorArgs = new[] { typeof(MerchelloContext), typeof(Basket), typeof(Address) };
            var ctoArgValues = new object[] { merchelloContext, basket, destination };
            var strategy = ActivatorHelper.CreateInstance<BasketPackagingStrategyBase>(Type.GetType(defaultStrategy), ctorArgs, ctoArgValues);

            return strategy.PackageShipments();
        }

        /// <summary>
        /// Returns a collection of <see cref="IShipmentRateQuote"/> from the various configured shipping providers
        /// </summary>
        /// <param name="shipment"><see cref="IShipment"/></param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public static IEnumerable<IShipmentRateQuote> ShipmentRateQuotes(this IShipment shipment)
        {
            return shipment.ShipmentRateQuotes(MerchelloContext.Current);
        }

        internal static IEnumerable<IShipmentRateQuote> ShipmentRateQuotes(this IShipment shipment, IMerchelloContext merchelloContext)
        {
            return merchelloContext.Gateways.GetShipRateQuotesForShipment(shipment);
        }
    }
}