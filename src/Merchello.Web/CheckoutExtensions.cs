using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Configuration;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
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

            var ctoArgValues = new object[] { merchelloContext, basket, destination };
            var strategy = ActivatorHelper.CreateInstance<BasketPackagingStrategyBase>(defaultStrategy, ctoArgValues);

            if (!strategy.Success)
            {
                LogHelper.Error<BasketPackagingStrategyBase>("PackageBasket failed to instantiate the defaultStrategy.", strategy.Exception);
                throw strategy.Exception;
            }

            return basket.PackageBasket(merchelloContext, destination, strategy.Result);
        }

        internal static IEnumerable<IShipment> PackageBasket(this IBasket basket, IMerchelloContext merchelloContext, IAddress destination, BasketPackagingStrategyBase strategy)
        {
            return !basket.Items.Any() ? new List<IShipment>() : strategy.PackageShipments();
        }


        /// <summary>
        /// Gets the <see cref="IBasketCheckoutPreparation"/>
        /// </summary>
        /// <param name="basket">The basket with items use in the checkout</param>
        /// <returns>A <see cref="IBasketCheckoutPreparation"/></returns>
        public static BasketCheckoutPreparation CheckoutPreparation(this IBasket basket)
        {
            return basket.CheckoutPreparation(MerchelloContext.Current);
        }

        /// <summary>
        /// Gets the <see cref="IBasketCheckoutPreparation"/>
        /// </summary>
        /// <param name="basket">The basket with items use in the checkout</param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A <see cref="IBasketCheckoutPreparation"/></returns>
        internal static BasketCheckoutPreparation CheckoutPreparation(this IBasket basket, IMerchelloContext merchelloContext)
        {
            return BasketCheckoutPreparation.GetBasketCheckout(merchelloContext, basket);
        }
    }
}