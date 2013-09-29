using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Services;
using Merchello.Core.Strategies.Payment;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Activator
{
    [TestFixture]
    public class ActivatorHelperTests
    {
        //[Test]
        //public void Can_Instantiate_PaymentApplicationStrategy()
        //{
        //    //// Arrange
        //    var args = new Type[] { typeof(CustomerService), typeof (InvoiceService), typeof (AppliedPaymentService)};
        //    var argValues = new object[] { new CustomerService(), new InvoiceService(), new AppliedPaymentService() };
        //    var expected = new PaymentApplicationStrategy(new CustomerService(), new InvoiceService(), new AppliedPaymentService());
            
        //    //// Act
        //    var actual = ActivatorHelper.CreateInstance<PaymentApplicationStrategy>(typeof(PaymentApplicationStrategy), args, argValues);

        //    //// Assert
        //    Assert.AreEqual(expected.GetType(), actual.GetType());
        //}
    }
}
