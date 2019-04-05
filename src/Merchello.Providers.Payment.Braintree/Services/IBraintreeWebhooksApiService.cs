namespace Merchello.Providers.Payment.Braintree.Services
{
    using global::Braintree;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the Braintree Web hooks ApiService.
    /// </summary>
    public interface IBraintreeWebhooksApiService : IService
    {
        /// <summary>
        /// Performs the Braintree web hook "verify".
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