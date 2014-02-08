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

            ItemCacheService.Created += BasketItemCacheCreated;
            ItemCacheService.Saved += BasketItemCacheSaved;
            ItemCacheService.Deleted += BasketItemCacheDeleted;
        }

        /// <summary>
        /// Purges customer <see cref="BasketCheckout"/> information on customer <see cref="IBasket"/> creation
        /// </summary>
        static void BasketItemCacheCreated(IItemCacheService sender, Core.Events.NewEventArgs<IItemCache> e)
        {
            if (e.Entity.ItemCacheType != ItemCacheType.Basket) return;
            ClearCheckoutItemCache(e.Entity.EntityKey);
        }

        /// <summary>
        /// Purges customer <see cref="BasketCheckout"/> information on customer <see cref="IBasket"/> saves.  The will
        /// also handle the Basket items saves & deletes
        /// </summary>
        static void BasketItemCacheSaved(IItemCacheService sender, SaveEventArgs<IItemCache> e)
        {
            foreach (var item in e.SavedEntities.Where(item => item.ItemCacheType == ItemCacheType.Basket))
            {
                ClearCheckoutItemCache(item.EntityKey);
            }
        }

        /// <summary>
        /// Purges customer <see cref="BasketCheckout"/> information on customer <see cref="IBasket"/> deletions
        /// </summary>
        static void BasketItemCacheDeleted(IItemCacheService sender, DeleteEventArgs<IItemCache> e)
        {
            foreach (var item in e.DeletedEntities.Where(item => item.ItemCacheType == ItemCacheType.Basket))
            {
                ClearCheckoutItemCache(item.EntityKey);
            }
        }

        private static void ClearCheckoutItemCache(Guid entityKey)
        {
            CheckoutBase.RestartCheckout(MerchelloContext.Current, entityKey);   
        }
    }
}