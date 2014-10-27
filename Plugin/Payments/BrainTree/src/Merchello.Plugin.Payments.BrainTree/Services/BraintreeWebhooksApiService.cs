namespace Merchello.Plugin.Payments.Braintree.Services
{
    using global::Braintree;
    using Core;
    using Models;

    /// <summary>
    /// Represents the BraintreeWebhooksApiService.
    /// </summary>
    internal class BraintreeWebhooksApiService : BraintreeApiServiceBase, IBraintreeWebhooksApiService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreeWebhooksApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public BraintreeWebhooksApiService(IMerchelloContext merchelloContext, BraintreeProviderSettings settings) 
            : base(merchelloContext, settings)
        {
        }

        /// <summary>
        /// Performs the Braintree webhook "verify".
        /// </summary>
        /// <param name="challenge">
        /// The challenge.
        /// </param>
        /// <returns>
        /// The content of the verify challenge.
        /// </returns>
        /// <remarks>
        /// https://developers.braintreepayments.com/javascript+dotnet/guides/webhooks
        /// </remarks>
        public string Verify(string challenge)
        {
            return BraintreeGateway.WebhookNotification.Verify(challenge);
        }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="signature">
        /// The signature.
        /// </param>
        /// <param name="payload">
        /// The payload.
        /// </param>
        /// <returns>
        /// The <see cref="WebhookNotification"/>.
        /// </returns>
        public WebhookNotification Parse(string signature, string payload)
        {
            return BraintreeGateway.WebhookNotification.Parse(signature, payload);
        }

        /// <summary>
        /// The sample notification.
        /// </summary>
        /// <param name="kind">
        /// The kind.
        /// </param>
        /// <param name="sampleId">
        /// The sample id.
        /// </param>
        /// <returns>
        /// The <see cref="WebhookNotification"/>.
        /// </returns>
        public WebhookNotification SampleNotification(WebhookKind kind, string sampleId)
        {
            var sample = BraintreeGateway.WebhookTesting.SampleNotification(kind, sampleId);

            return this.Parse(sample["signature"], sample["payload"]);
        }
    }
}