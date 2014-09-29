using System;
using System.Configuration;
using  Merchello.Plugin.Payments.Chase.Models;

namespace Merchello.Tests.Chase.Integration.TestHelpers
{
    public class TestHelper
    {
        public static ChaseProcessorSettings GetChaseProviderSettings()
        {
            return new ChaseProcessorSettings()
            {
                MerchantId = ConfigurationManager.AppSettings["merchantId"],
                Bin = ConfigurationManager.AppSettings["bin"],
                Username = ConfigurationManager.AppSettings["username"],
                Password = ConfigurationManager.AppSettings["password"],
            };
        }

        public static string PaymentMethodNonce
        {
            get
            {
                return "nonce-from-the-client";
            }
        }

    }
}