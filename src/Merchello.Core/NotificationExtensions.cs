using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Payment;

namespace Merchello.Core
{
    public static class NotificationExtensions
    {
        public static void Notify(this IPaymentResult result, string alias, object model)
        {
            result.Notify(alias, model, new string[]{});
        }

        public static void Notify(this IPaymentResult result, string alias, object model, IEnumerable<string> contacts)
        {
            throw new NotImplementedException();
        }
       

    }

}