namespace Merchello.Tests.Braintree.Integration.Webhooks
{
    using global::Braintree;

    using Merchello.Tests.Braintree.Integration.TestHelpers;

    using NUnit.Framework;

    public class WebhooksTests : BraintreeTestBase
    {
        [Test]
        public void Can_Emulate_A_Webhook_Request()
        {
            //// Arrange
            var subscriptionId = "b66ckg";

            //// Act
            var notification = BraintreeApiService.Webhook.SampleNotification(WebhookKind.SUBSCRIPTION_CHARGED_SUCCESSFULLY, subscriptionId);

            //// Assert
            Assert.NotNull(notification);
        }
    }
}