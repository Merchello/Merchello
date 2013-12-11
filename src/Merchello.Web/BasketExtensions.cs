using System;
using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web.Models;
using Merchello.Web.Shipping;
using Umbraco.Web.org.umbraco.our;

namespace Merchello.Web
{
    /// <summary>
    /// Extension methods for the <see cref="IBasket"/>
    /// </summary>
    public static class BasketExtensions
    {
        public static IEnumerable<IShipment> PackageBasket(this IBasket basket, IAddress destination)
        {
            // get the default strategy from the configuration file
            var defaultStrategy = MerchelloConfiguration.Current.DefaultBasketPackagingStrategy;

            var ctorArgs = new[] {typeof (Basket), typeof (Address)};
            var ctoArgValues = new object[] { basket, destination};
            var strategy = ActivatorHelper.CreateInstance<IBasketPackagingStrategy>(Type.GetType(defaultStrategy), ctorArgs, ctoArgValues);

            return strategy.PackageShipments();
        }

    }
}