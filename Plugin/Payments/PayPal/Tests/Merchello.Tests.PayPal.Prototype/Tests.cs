using Merchello.Tests.PayPal.Prototype.ExpressCheckout;
using NUnit.Framework;

namespace Merchello.Tests.PayPal.Prototype
{
	[TestFixture]
    public class Tests
    {
		[Test]
		public void DoExpressCheckoutPayment()
		{
			var sample = new DoExpressCheckoutPaymentSample();
			var responseDoExpressCheckoutPaymentResponseType = sample.DoExpressCheckoutPaymentAPIOperation();
			Assert.IsNotNull(responseDoExpressCheckoutPaymentResponseType);
			// Please change the sample inputs according to the documentation in the sample for the following assertions:
			// Assert.AreEqual(responseDoExpressCheckoutPaymentResponseType.Ack.ToString().ToUpper(), "SUCCESS");
			// Assert.IsNotNull(responseDoExpressCheckoutPaymentResponseType.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID);
		}

		[Test]
		public void GetExpressCheckoutDetails()
		{
			var sample = new GetExpressCheckoutDetailsSample();
			var responseGetExpressCheckoutDetailsResponseType = sample.GetExpressCheckoutDetailsAPIOperation();
			Assert.IsNotNull(responseGetExpressCheckoutDetailsResponseType);
			// Please change the sample inputs according to the documentation in the sample for the following assertion:
			// Assert.AreEqual(responseGetExpressCheckoutDetailsResponseType.Ack.ToString().ToUpper(), "SUCCESS");
		}

		[Test]
		public void SetExpressCheckout()
		{
			var sample = new SetExpressCheckoutSample();
			var responseSetExpressCheckoutResponseType = sample.SetExpressCheckoutAPIOperation();
			Assert.IsNotNull(responseSetExpressCheckoutResponseType);
			Assert.AreEqual(responseSetExpressCheckoutResponseType.Ack.ToString().ToUpper(), "SUCCESS");
			Assert.IsNotNull(responseSetExpressCheckoutResponseType.Token);
		}
    }
}
