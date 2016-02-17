namespace Merchello.Tests.PaymentProviders.Braintree.Subscriptions
{
    using System.Linq;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Tests.PaymentProviders.Braintree.TestHelpers;

    using NUnit.Framework;

    [TestFixture]
    public class SubscriptionTests : BraintreeTestBase
    {
        [Test]
        public void Can_Get_A_List_Of_All_Plans()
        {
            //// Arrange
            var planid = "1MONTH";

            //// Act
            var plans = this.BraintreeApiService.Subscription.GetAllPlans().ToArray();
            Assert.IsTrue(plans.Any());

            var boxPlan = plans.FirstOrDefault(x => x.Id.Equals(planid));

            //// Assert
            Assert.NotNull(boxPlan);

        }



        [Test]
        public void Can_Create_A_Subscription_For_A_Customer()
        {
            //// Arrange
            // Step 1 - Plan selection

            var plan = this.BraintreeApiService.Subscription.GetAllPlans().FirstOrDefault();
            Assert.NotNull(plan, "The plan was null");

            // Step 2 - establish the customer
            var customer = this.BraintreeApiService.Customer.GetBraintreeCustomer(this.TestCustomer);


            // Step 3 - select the paymentmethod
            PaymentMethod paymentMethod;

            if (customer.PaymentMethods.Any())
            {
                // use the default payment method
                paymentMethod = customer.DefaultPaymentMethod;
            }
            else
            {
                // or create a new payment method
                var billingAddress = new Core.Models.Address()
                {
                    Address1 = "114 W. Magnolia St. Suite 300",
                    Locality = "Bellingham",
                    Region = "WA",
                    PostalCode = "98225",
                    CountryCode = "US",
                    AddressType = AddressType.Billing
                };

                // The 'true' value sets this to the default payment method.  This is true by default
                var attemptPaymentMethod = this.BraintreeApiService.PaymentMethod.Create(this.TestCustomer, "nonce-from-the-client", billingAddress, true);

                if (!attemptPaymentMethod.Success) Assert.Fail("Failed to create the payment method");

                paymentMethod = attemptPaymentMethod.Result;
            }
            
            //// Act
            
            // note there are additional paremeters to fill out here
            var subscriptionAttempt = this.BraintreeApiService.Subscription.Create(paymentMethod.Token, plan.Id);

            //// Assert
            Assert.IsTrue(subscriptionAttempt.Success, "Failed to create the subscription ");


            var subscription = subscriptionAttempt.Result;

            if (subscription.Status == SubscriptionStatus.ACTIVE)
            { 
                var trans = subscription.Transactions.FirstOrDefault();

                var cs = trans.Customer;

                Assert.NotNull(cs);
            }
        }

        [Test]
        public void Can_Create_A_Subscription_For_A_Customer_With_Discount()
        {
            //// Arrange
            // Step 1 - Plan selection

            var plan = this.BraintreeApiService.Subscription.GetAllPlans().FirstOrDefault();
            Assert.NotNull(plan, "The plan was null");

            var discount = this.BraintreeApiService.Subscription.GetAllDiscounts().FirstOrDefault();
            Assert.NotNull(discount, "The discount was null");

            // Step 2 - establish the customer
            var customer = this.BraintreeApiService.Customer.GetBraintreeCustomer(this.TestCustomer);


            // Step 3 - select the paymentmethod
            PaymentMethod paymentMethod;

            if (customer.PaymentMethods.Any())
            {
                // use the default payment method
                paymentMethod = customer.DefaultPaymentMethod;
            }
            else
            {
                // or create a new payment method
                var billingAddress = new Core.Models.Address()
                {
                    Address1 = "114 W. Magnolia St. Suite 300",
                    Locality = "Bellingham",
                    Region = "WA",
                    PostalCode = "98225",
                    CountryCode = "US",
                    AddressType = AddressType.Billing
                };

                // The 'true' value sets this to the default payment method.  This is true by default
                var attemptPaymentMethod = this.BraintreeApiService.PaymentMethod.Create(this.TestCustomer, "nonce-from-the-client", billingAddress, true);

                if (!attemptPaymentMethod.Success) Assert.Fail("Failed to create the payment method");

                paymentMethod = attemptPaymentMethod.Result;
            }
            
            //// Act
            
            // note there are additional paremeters to fill out here
            var subscriptionRequest = new SubscriptionRequest()
                                          {
                                              PaymentMethodToken = paymentMethod.Token,
                                              PlanId = plan.Id,
                                              Discounts = new DiscountsRequest()
                                                              {
                                                                  Add = new[]
                                                                    {
                                                                        new AddDiscountRequest()
                                                                            {
                                                                                InheritedFromId = discount.Id
                                                                            }
                                                                    }
                                                              }
                                          };

            var subscriptionAttempt = this.BraintreeApiService.Subscription.Create(subscriptionRequest);

            //// Assert
            Assert.IsTrue(subscriptionAttempt.Success, "Failed to create the subscription ");


            var subscription = subscriptionAttempt.Result;

            if (subscription.Status == SubscriptionStatus.ACTIVE)
            { 
                var trans = subscription.Transactions.FirstOrDefault();

                var cs = trans.Customer;

                Assert.NotNull(cs);
            }
        }
    }
}