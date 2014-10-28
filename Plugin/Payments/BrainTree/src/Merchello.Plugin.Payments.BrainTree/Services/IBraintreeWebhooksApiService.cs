namespace Merchello.Plugin.Payments.Braintree.Services
{
    using global::Braintree;

    /// <summary>
    /// Defines the BraintreeWebhooksApiService.
    /// </summary>
    public interface IBraintreeWebhooksApiService
    {
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
        string Verify(string challenge);

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
        WebhookNotification Parse(string signature, string payload);

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
        WebhookNotification SampleNotification(WebhookKind kind, string sampleId);
    }
}