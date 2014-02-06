using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Configuration;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models;
using Merchello.Web.Workflow;
using Merchello.Web.Workflow.Shipping;

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

        /// <summary>
        /// Packages the current basket instantiation into a collection of <see cref="IShipment"/> using the <see cref="BasketPackagingStrategyBase"/> strategy
        /// </summary>        
        /// <returns>A collection of <see cref="IShipment"/></returns>
        public static IEnumerable<IShipment> PackageBasket(this IBasket basket, IAddress destination, BasketPackagingStrategyBase strategy)
        {
            return basket.PackageBasket(MerchelloContext.Current, destination, strategy);
        }


        internal static IEnumerable<IShipment> PackageBasket(this IBasket basket, IMerchelloContext merchelloContext, IAddress destination)
        {
            var defaultStrategy = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultBasketPackaging).Type;

            var ctorArgs = new[] { typeof(MerchelloContext), typeof(Basket), typeof(Address) };
            var ctoArgValues = new object[] { merchelloContext, basket, destination };
            var strategy = ActivatorHelper.CreateInstance<BasketPackagingStrategyBase>(Type.GetType(defaultStrategy), ctorArgs, ctoArgValues);

            return basket.PackageBasket(merchelloContext, destination, strategy);
        }

        internal static IEnumerable<IShipment> PackageBasket(this IBasket basket, IMerchelloContext merchelloContext, IAddress destination, BasketPackagingStrategyBase strategy)
        {
            return !basket.Items.Any() ? new List<IShipment>() : strategy.PackageShipments();
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