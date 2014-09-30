namespace Merchello.Tests.Braintree.Integration.TestHelpers
{
    using System;
    using System.Configuration;

    using Merchello.Plugin.Payments.Braintree.Models;

    using Environment = global::Braintree.Environment;

    public class TestHelper
    {
        public static BraintreeProviderSettings GetBraintreeProviderSettings()
        {
            return new BraintreeProviderSettings()
                            {
                                Environment = Environment.SANDBOX,
                                PublicKey = ConfigurationManager.AppSettings["publicKey"],
                                PrivateKey = ConfigurationManager.AppSettings["privateKey"],
                                MerchantId = ConfigurationManager.AppSettings["merchantId"],
                                MerchantDescriptor = new MerchantDescriptor()
                                    {
                                        Name = ConfigurationManager.AppSettings["merchantName"],
                                        Url = ConfigurationManager.AppSettings["merchantUrl"],
                                        Phone = ConfigurationManager.AppSettings["merchantPhone"]
                                    },
                                DefaultTransactionOption = (TransactionOption)Enum.Parse(typeof(TransactionOption), ConfigurationManager.AppSettings["defaultTransactionOption"])
                            };
        }

        public static string PaymentMethodNonce
        {
            get
            {
                return "nonce-from-the-client";
            }
        }

        public static string PaymentMethodToken
        {
            get { return "the_token"; }
        }


    }
}