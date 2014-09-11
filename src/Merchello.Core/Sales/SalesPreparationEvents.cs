namespace Merchello.Core.Sales
{
    using System;
    using Events;
    using Gateways.Payment;
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// Handles sales preparation events.
    /// </summary>
    public class SalesPreparationEvents : ApplicationEventHandler
    {
        /// <summary>
        /// The Umbraco application started event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The Umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The Umbraco application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            SalePreparationBase.Finalizing += SalePreparationBaseOnFinalizing;
        }

        /// <summary>
        /// Handles the <see cref="SalePreparationBase"/> finalizing event
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The <see cref="IPaymentResult"/>
        /// </param>
        private void SalePreparationBaseOnFinalizing(SalePreparationBase sender, SalesPreparationEventArgs<IPaymentResult> args)
        {
            var result = args.Entity;

            var customerKey = result.Invoice.CustomerKey;

            // Clean up the sales prepartation item cache
            if (customerKey == null || Guid.Empty.Equals(customerKey)) return;
            var customer = MerchelloContext.Current.Services.CustomerService.GetAnyByKey(customerKey.Value);
            
            if (customer == null) return;
            var itemCacheService = MerchelloContext.Current.Services.ItemCacheService;
            var itemCache = itemCacheService.GetItemCacheByCustomer(customer,  ItemCacheType.Checkout);
            itemCacheService.Delete(itemCache);
        }
    }
}