using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Configuration;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Strategies.Packaging;
using Merchello.Web.Workflow;
using Merchello.Web.Workflow.Shipping;
using Umbraco.Core.Logging;

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
        /// Packages the current basket instantiation into a collection of <see cref="IShipment"/> using the <see cref="PackagingStrategyBase"/> strategy
        /// </summary>        
        /// <returns>A collection of <see cref="IShipment"/></returns>
        public static IEnumerable<IShipment> PackageBasket(this IBasket basket, IAddress destination, PackagingStrategyBase strategy)
        {
            return basket.PackageBasket(MerchelloContext.Current, destination, strategy);
        }


        internal static IEnumerable<IShipment> PackageBasket(this IBasket basket, IMerchelloContext merchelloContext, IAddress destination)
        {
            var defaultStrategy = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultPackaging).Type;

            var ctoArgValues = new object[] { merchelloContext, basket.Items, destination, basket.VersionKey };
            var strategy = ActivatorHelper.CreateInstance<PackagingStrategyBase>(defaultStrategy, ctoArgValues);

            if (!strategy.Success)
            {
                LogHelper.Error<PackagingStrategyBase>("PackageBasket failed to instantiate the defaultStrategy.", strategy.Exception);
                throw strategy.Exception;
            }

            return basket.PackageBasket(merchelloContext, destination, strategy.Result);
        }

        internal static IEnumerable<IShipment> PackageBasket(this IBasket basket, IMerchelloContext merchelloContext, IAddress destination, PackagingStrategyBase strategy)
        {
            return !basket.Items.Any() ? new List<IShipment>() : strategy.PackageShipments();
        }


        /// <summary>
        /// Gets the <see cref="IBasketOrderPreparation"/>
        /// </summary>
        /// <param name="basket">The basket with items use in the checkout</param>
        /// <returns>A <see cref="IBasketOrderPreparation"/></returns>
        public static BasketOrderPreparation OrderPreparation(this IBasket basket)
        {
            return basket.OrderPreparation(MerchelloContext.Current);
        }

        /// <summary>
        /// Gets the <see cref="IBasketOrderPreparation"/>
        /// </summary>
        /// <param name="basket">The basket with items use in the checkout</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A <see cref="IBasketOrderPreparation"/></returns>
        internal static BasketOrderPreparation OrderPreparation(this IBasket basket, IMerchelloContext merchelloContext)
        {
            return BasketOrderPreparation.GetBasketCheckoutPreparation(merchelloContext, basket);
        }
    }
}