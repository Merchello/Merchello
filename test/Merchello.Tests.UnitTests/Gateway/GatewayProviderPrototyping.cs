using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Gateway;
using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Gateway;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Gateway
{
    [TestFixture]
    public class GatewayProviderPrototyping
    {
        [Test]
        public void Main()
        {
            ////// Arrange
            //var expected = new MockPaymentGatewayProvider(); 
             
            ////// Act
            //var instance = MockGatewayContext.CreateInstance<MockPaymentGatewayProvider>();

            ////// Assert
            //Console.WriteLine(instance.GetType().Name);
            //Assert.AreEqual(expected.GetType(), instance.GetType());
        }

        //[Test]
        //public void CollectPayment()
        //{
        //    var key = Guid.NewGuid();
        //    var context = new MockGatewayContext(new RegisteredGatewayProviderService());

        //    ReceivePaymentStrategyBase provider = context.PaymentProvider.GetInstance(key);

        //    var customer = MockCustomerDataMaker.CustomerForInserting();
        //    decimal amount = 11M;

        //    //IReceivePaymentStrategy receivePayment = new CashPayment(provider, customer, amount);

        //    var payment = receivePayment.ReceivePayment();
        //}
    }


    
}
