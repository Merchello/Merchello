using Merchello.Core;
using Merchello.Core.Builders;
using Merchello.Core.Checkout;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Chains
{

    [TestFixture]
    public class InvoiceBuilderTests
    {
        

        [Test]
        public void Can_Create_The_Default_Invoice_Builder_Having_3_Tasks()
        {
            //// Arrange
            
            //// Act
            //var invoiceBuild = new InvoiceBuilder()
        }
    }


    internal class CheckoutMock : CheckoutBase
    {
        public CheckoutMock(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer) : base(merchelloContext, itemCache, customer)
        {
        }

        public override void CompleteCheckout(IPaymentGatewayProvider paymentGatewayProvider)
        {
            throw new System.NotImplementedException();
        }
    }

}