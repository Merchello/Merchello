using System;
using System.Web.WebSockets;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    /// <summary>
    /// Helper class to assist in putting together payment data for testing
    /// </summary>
    public class MockPaymentDataMaker : MockDataMakerBase
    {
        public static IPayment PaymentForInserting(ICustomer customer, Guid providerKey, PaymentMethodType paymentMethodType, decimal amount)
        {
            var payment = new Payment(customer, paymentMethodType, amount)
            {
                ProviderKey = providerKey
            };

            return payment;
        }
    }
}