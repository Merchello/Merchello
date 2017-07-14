namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Core;
    using Core.Configuration;
    using Core.Models;
    using Core.Strategies.Packaging;

    using Merchello.Core.Checkout;
    using Merchello.Core.Logging;
    using Merchello.Web.CheckoutManagers;

    using Umbraco.Core.Logging;
    using Workflow;

    /// <summary>
    /// Extension methods for the <see cref="IBasket"/>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed. Suppression is OK here.")]
    public static class CheckoutExtensions
    {
        /// <summary>
        /// Gets a <see cref="ICheckoutManagerBase"/> for the basket.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </returns>
        public static ICheckoutManagerBase GetCheckoutManager(this IBasket basket)
        {
            return basket.GetCheckoutManager(MerchelloContext.Current, new CheckoutContextSettings());
        }

        /// <summary>
        /// The get checkout manager.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="settings">
        /// The checkout context version change settings.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </returns>
        public static ICheckoutManagerBase GetCheckoutManager(this IBasket basket, ICheckoutContextSettings settings)
        {
            return new BasketCheckoutManager(basket.CreateCheckoutContext(MerchelloContext.Current, settings));
        }

        /// <summary>
        /// Gets a <see cref="ICheckoutManagerBase"/> for the basket.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello Context.
        /// </param>
        /// <param name="settings">
        /// The checkout context version change settings.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </returns>
        internal static ICheckoutManagerBase GetCheckoutManager(this IBasket basket, IMerchelloContext merchelloContext, ICheckoutContextSettings settings)
        {
            return new BasketCheckoutManager(basket.CreateCheckoutContext(merchelloContext, settings));
        }

        /// <summary>
        /// The get checkout context.
        /// </summary>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutContext"/>.
        /// </returns>
        internal static ICheckoutContext CreateCheckoutContext(this IBasket basket, IMerchelloContext merchelloContext, ICheckoutContextSettings settings)
        {
            var context = CheckoutContext.CreateCheckoutContext(merchelloContext, basket.Customer, basket.VersionKey, settings);

            if (context.IsNewVersion && basket.Validate())
            {
                // TODO - review redundant call to basket.Validate().
                if (!context.ItemCache.Items.Any() && basket.Validate())
                {
                    // this is either a new preparation or a reset due to version
                    foreach (var item in basket.Items)
                    {
                        // convert to a LineItem of the same type for use in the CheckoutPrepartion collection
                        context.ItemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
                    }

                    merchelloContext.Services.ItemCacheService.Save(context.ItemCache);
                }
            }

            return context;
        }

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
        [Obsolete("Use GetCheckoutManager()")]
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

            MultiLogHelper.Error<PackagingStrategyBase>("PackageBasket failed to instantiate the defaultStrategy.", strategy.Exception);
            
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
        [Obsolete("Use GetCheckoutManager()")]
        internal static BasketSalePreparation SalePreparation(this IBasket basket, IMerchelloContext merchelloContext)
        {
            return BasketSalePreparation.GetBasketCheckoutPreparation(merchelloContext, basket);
        }
    }
}