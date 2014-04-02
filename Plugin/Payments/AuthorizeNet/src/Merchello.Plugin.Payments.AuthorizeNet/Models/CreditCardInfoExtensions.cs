using Merchello.Core.Gateways.Payment;

namespace Merchello.Plugin.Payments.AuthorizeNet.Models
{
    public static class CreditCardInfoExtensions
    {
        public static ProcessorArgumentCollection AsProcessorArgumentCollection(this CreditCardFormData creditCard)
        {
            return new ProcessorArgumentCollection()
            {
                { "creditCardType", creditCard.CreditCardType },
                { "cardholderName", creditCard.CardholderName },
                { "cardNumber", creditCard.CardNumber },
                { "expireMonth", creditCard.ExpireMonth },
                { "expireYear", creditCard.ExpireYear },
                { "cardCode", creditCard.CardCode },
                { "customerIp", creditCard.CustomerIp }
            };
        }
    }
}