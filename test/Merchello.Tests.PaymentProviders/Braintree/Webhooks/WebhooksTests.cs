namespace Merchello.Tests.PaymentProviders.Braintree.Webhooks
{
    using global::Braintree;

    using Merchello.Tests.PaymentProviders.Braintree.TestHelpers;

    using NUnit.Framework;

    public class WebhooksTests : BraintreeTestBase
    {
        [Test]
        public void Can_Emulate_A_Webhook_Request()
        {
            //// Arrange
            // This will fail if no plans are configured
            var subscriptionId = "b66ckg";

            //// Act
            var notification = this.BraintreeApiService.Webhook.SampleNotification(WebhookKind.SUBSCRIPTION_CHARGED_SUCCESSFULLY, subscriptionId);

            //// Assert
            Assert.NotNull(notification);
        }
    }
}