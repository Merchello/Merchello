namespace Merchello.Tests.PaymentProviders.Braintree.TestHelpers
{
    using Merchello.Providers.Payment.Models;

    using Newtonsoft.Json;

    public class TestHelper
    {
        public static BraintreeProviderSettings GetBraintreeProviderSettings()
        {
            string json = System.IO.File.ReadAllText(@"C:\Working Repositories\Github\Merchello\test\Merchello.Tests.PaymentProviders\Braintree\extendedData.json");
            return JsonConvert.DeserializeObject<BraintreeProviderSettings>(json);
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