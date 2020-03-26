using AutoMapper;
using Umbraco.Core.Models.Mapping;

namespace Merchello.Providers.Payment.Braintree
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using global::Braintree;

    using Merchello.Providers.Payment.Braintree.Controllers;
    using Merchello.Providers.Payment.Braintree.Services;

    using Umbraco.Core;
    using Umbraco.Core.Events;
    using Umbraco.Web;
    using Umbraco.Web.UI.JavaScript;

    public class UmbracoApplicationEvents : ApplicationEventHandler
	{
		protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			//// Clear cache for customer
			//// TODO this is sort of punting to blanket clear all cached braintree requests
			//// but in some cases a customer needs to be cleared when the id is not available
			//// likewise a subscription when the payment method changes, etc.
			BraintreeSubscriptionApiService.Created += BraintreeSubscriptionApiServiceOnCreated;
			BraintreeSubscriptionApiService.Updated += BraintreeSubscriptionApiServiceOnUpdated;
			BraintreePaymentMethodApiService.Created += BraintreePaymentMethodApiServiceOnCreated;
			BraintreePaymentMethodApiService.Updating += BraintreePaymentMethodApiServiceOnUpdating;

			ServerVariablesParser.Parsing += ServerVariablesParserOnParsing;
			Mapper.Initialize(cfg => cfg.AddProfile<AutomapperProfile>());

			base.ApplicationStarted(umbracoApplication, applicationContext);
		}

		/// <summary>
		/// The server variables parser on parsing.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The dictionary.
		/// </param>
		private static void ServerVariablesParserOnParsing(object sender, Dictionary<string, object> e)
		{
			if (HttpContext.Current == null) throw new InvalidOperationException("HttpContext is null");

			if (e.ContainsKey("merchelloPaymentsUrls")) return;

			var merchelloPaymentsUrls = new Dictionary<string, object>();

			var url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

			merchelloPaymentsUrls.Add(
				"merchelloBraintreeApiBaseUrl",
				url.GetUmbracoApiServiceBaseUrl<BraintreeApiController>(
					controller => controller.GetClientRequestToken(Guid.Empty)));


			e.Add("merchelloPaymentsUrls", merchelloPaymentsUrls);
		}

		/// <summary>
        /// The clear braintree cache.
        /// </summary>
        private static void ClearBraintreeCache()
        {
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch("braintree.");
        }

        /// <summary>
        /// Clears the cache when a payment method is updated
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private static void BraintreePaymentMethodApiServiceOnUpdating(BraintreePaymentMethodApiService sender, SaveEventArgs<PaymentMethodRequest> saveEventArgs)
        {
            ClearBraintreeCache();
        }

        /// <summary>
        /// Clears the cache when a payment method is created
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="newEventArgs">
        /// The new event args.
        /// </param>
        private static void BraintreePaymentMethodApiServiceOnCreated(BraintreePaymentMethodApiService sender, Core.Events.NewEventArgs<global::Braintree.PaymentMethod> newEventArgs)
        {
            ClearBraintreeCache();
        }

        /// <summary>
        /// Clears the cache with a subscription is updated
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="saveEventArgs">
        /// The save event args.
        /// </param>
        private static void BraintreeSubscriptionApiServiceOnUpdated(BraintreeSubscriptionApiService sender, SaveEventArgs<Subscription> saveEventArgs)
        {
            ClearBraintreeCache();
        }

        /// <summary>
        /// The braintree subscription api service on created.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="newEventArgs">
        /// The new event args.
        /// </param>
        private static void BraintreeSubscriptionApiServiceOnCreated(BraintreeSubscriptionApiService sender, Core.Events.NewEventArgs<Subscription> newEventArgs)
        {
            ClearBraintreeCache();
        }
	}
}
