namespace Merchello.Providers.Payment.Braintree.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using global::Braintree;

    using Merchello.Providers.Payment.Braintree.Services;

    using Umbraco.Core.Logging;
    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// An base controller for handling web hook notifications from Braintree.
    /// </summary>
    public abstract class BraintreeWebhooksControllerBase : SurfaceController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeWebhooksControllerBase"/> class.
        /// </summary>
        protected BraintreeWebhooksControllerBase()
            : this(UmbracoContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeWebhooksControllerBase"/> class.
        /// </summary>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        protected BraintreeWebhooksControllerBase(UmbracoContext umbracoContext)
            : base(umbracoContext)
        {
            this.BraintreeApiService = new BraintreeApiService(BraintreeApiHelper.GetBraintreeProviderSettings());
        }

        /// <summary>
        /// Gets the <see cref="BraintreeApiService"/>.
        /// </summary>
        protected IBraintreeApiService BraintreeApiService { get; private set; }

        /// <summary>
        /// Accepts a web notification request and handles the verification process.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Accept()
        {
            if (Request.HttpMethod == "POST")
            {
                var webhookNotification = BraintreeApiService.Webhook.Parse(
                    Request.Params["bt_signature"],
                    Request.Params["bt_payload"]);

                // if the notification is null return NOT ACCEPTABLE
                if (webhookNotification == null)
                {
                    LogHelper.Debug<BraintreeWebhooksControllerBase>("Failed to parse webhook notification");
                    return new HttpStatusCodeResult(406);
                }

                var message = string.Format(
                   "[Webhook Received {0}] | Kind: {1} | Subscription: {2}",
                    webhookNotification.Timestamp.Value,
                    webhookNotification.Kind,
                    webhookNotification.Subscription.Id);

                LogHelper.Info<BraintreeWebhooksControllerBase>(message);

                // Handle in notication in the sub-class
                HandleWebhookNotification(webhookNotification);

            }
            else
            {
                LogHelper.Info<BraintreeWebhooksControllerBase>("Verifing Braintree web hook notification");
                return this.Verify(Request.QueryString["bt_challenge"]);
            }

            return new HttpStatusCodeResult(200);
        }

        /// <summary>
        /// Method call to handle the web hook notification.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected abstract ActionResult HandleWebhookNotification(WebhookNotification notification);


        /// <summary>
        /// The get plans.
        /// </summary>
        /// <returns>
        /// The <see cref="IDictionary{String, Plan}"/>.
        /// </returns>
        protected IDictionary<string, Plan> GetPlans()
        {
            var braintreePlans = BraintreeApiService.Subscription.GetAllPlans().ToArray();

            return braintreePlans.ToDictionary(p => p.Id);
        }

        /// <summary>
        /// Verifies the challenge from the web hook notification
        /// </summary>
        /// <param name="challenge">
        /// The challenge.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        private ActionResult Verify(string challenge)
        {
            LogHelper.Info<BraintreeWebhooksControllerBase>("Challenge: " + challenge);
            return Content(BraintreeApiService.Webhook.Verify(challenge));
        }

    }
}