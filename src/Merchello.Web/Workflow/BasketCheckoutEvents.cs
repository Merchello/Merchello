using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Web.Workflow
{
    public class BasketCheckoutEvents : ApplicationEventHandler
    {

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,  ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<BasketCheckoutEvents>("Initializing Merchello ServerVariablesParsingEvents");

            ItemCacheService.Saved += BasketItemCacheSaved;
        }

        /// <summary>
        /// Purges customer <see cref="BasketCheckoutPreparation"/> information on customer <see cref="IBasket"/> saves.  The will
        /// also handle the Basket items saves & deletes
        /// </summary>
        static void BasketItemCacheSaved(IItemCacheService sender, SaveEventArgs<IItemCache> e)
        {
            foreach (var item in e.SavedEntities.Where(item => item.ItemCacheType == ItemCacheType.Basket))
            {
                CheckoutPreparationBase.RestartCheckout(MerchelloContext.Current, item.EntityKey);  
            }
        }
    }
}