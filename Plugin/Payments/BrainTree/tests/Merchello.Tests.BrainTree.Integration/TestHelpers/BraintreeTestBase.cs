namespace Merchello.Tests.Braintree.Integration.TestHelpers
{
    using System;
    using System.Configuration;

    using global::Braintree;

    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Plugin.Payments.Braintree.Models;

    using NUnit.Framework;

    using Environment = global::Braintree.Environment;

    public abstract class BraintreeTestBase
    {
        protected BraintreeProviderSettings BraintreeProviderSettings;

        protected Guid CustomerKey = new Guid("1A6E8170-9CB9-41C0-B944-36F16B97BED2");

        protected BraintreeGateway Gateway;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            this.BraintreeProviderSettings = new BraintreeProviderSettings()
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
                                    }
                            };

            AutoMapper.Mapper.CreateMap<BraintreeProviderSettings, BraintreeGateway>();

            Gateway = BraintreeProviderSettings.AsBraintreeGateway();
        }
    }
}