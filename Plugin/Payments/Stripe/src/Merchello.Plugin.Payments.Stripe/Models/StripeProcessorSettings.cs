namespace Merchello.Plugin.Payments.Stripe.Models
{
    public class StripeProcessorSettings
    {
        public string ApiKey { get; set; }

        public string ApiVersion
        {
            get { return "2014-06-17"; }
        }
    }
}
