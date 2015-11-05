namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Configuration;
    using Core.Models;
    using Core.Strategies.Packaging;
    using Umbraco.Core.Logging;
    using Workflow;

    /// <summary>
    /// Extension methods for the <see cref="IBasket"/>
    /// </summary>
    public static class CheckoutExtensions
    {
        /// <summary>
        /// Packages the current basket instantiation into a collection of <see cref="IShipment"/>s
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IShipment"/>
        /// </returns>
        public static IEnumerable<IShipment> PackageBasket(this IBasket basket, IAddress destination)
        {
            return basket.PackageBasket(MerchelloContext.Current, destination);
        }

        /// <summary>
        /// Packages the current basket instantiation into a collection of <see cref="IShipment"/> using the <see cref="PackagingStrategyBase"/> strategy
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="strategy">
        /// The strategy.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IShipment"/>
        /// </returns>
        public static IEnumerable<IShipment> PackageBasket(this IBasket basket, IAddress destination, PackagingStrategyBase strategy)
        {
            return basket.PackageBasket(MerchelloContext.Current, destination, strategy);
        }

        /// <summary>
        /// Gets the <see cref="IBasketSalePreparation"/>
        /// </summary>
        /// <param name="basket">
        /// The basket with items use in the checkout
        /// </param>
        /// <returns>
        /// A <see cref="IBasketSalePreparation"/>
        /// </returns>
        /// <remarks>
        /// TODO change this to return IBasketSalePreparation
        /// </remarks>
        public static BasketSalePreparation SalePreparation(this IBasket basket)
        {
            return basket.SalePreparation(MerchelloContext.Current);
        }

        /// <summary>
        /// Packages a basket into one or more shipments
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IShipment"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if the basket packaging strategy fails to instantiate
        /// </exception>
        internal static IEnumerable<IShipment> PackageBasket(this IBasket basket, IMerchelloContext merchelloContext, IAddress destination)
        {
            var defaultStrategy = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultPackaging).Type;

            var ctoArgValues = new object[] { merchelloContext, basket.Items, destination, basket.VersionKey };
            var strategy = ActivatorHelper.CreateInstance<PackagingStrategyBase>(defaultStrategy, ctoArgValues);

            if (strategy.Success)
            {
                return basket.PackageBasket(merchelloContext, destination, strategy.Result);
            }

            LogHelper.Error<PackagingStrategyBase>("PackageBasket failed to instantiate the defaultStrategy.", strategy.Exception);
            
            throw strategy.Exception;
        }

        /// <summary>
        /// Packages a basket into one or more shipments.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="strategy">
        /// The strategy.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IShipment"/>.
        /// </returns>
        internal static IEnumerable<IShipment> PackageBasket(this IBasket basket, IMerchelloContext merchelloContext, IAddress destination, PackagingStrategyBase strategy)
        {
            return !basket.Items.Any() ? new List<IShipment>() : strategy.PackageShipments();
        }

        /// <summary>
        /// Gets the <see cref="IBasketSalePreparation"/>
        /// </summary>
        /// <param name="basket">
        /// The basket with items use in the checkout
        /// </param>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>
        /// </param>
        /// <returns>
        /// A <see cref="IBasketSalePreparation"/>
        /// </returns>
        internal static BasketSalePreparation SalePreparation(this IBasket basket, IMerchelloContext merchelloContext)
        {
            return BasketSalePreparation.GetBasketCheckoutPreparation(merchelloContext, basket);
        }
    }
}