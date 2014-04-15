﻿using Merchello.Core.Gateways.Payment;

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

        public static CreditCardFormData AsCreditCardFormData(this ProcessorArgumentCollection args)
        {
            return new CreditCardFormData()
            {
                CreditCardType = args.ArgValue("creditCardType"),
                CardholderName = args.ArgValue("cardholderName"),
                CardNumber = args.ArgValue("cardNumber"),
                ExpireMonth = args.ArgValue("expireMonth"),
                ExpireYear = args.ArgValue("expireYear"),
                CardCode = args.ArgValue("cardCode"),
                CustomerIp = args.ArgValue("customerIp")
            };
        }

        private static string ArgValue(this ProcessorArgumentCollection args, string key)
        {
            return args.ContainsKey(key) ? args[key] : string.Empty;
        }

    }
}