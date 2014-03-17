using System;
using System.Collections.Generic;

namespace Merchello.Web.Models
{
    public class PaymentRequest
    {
        public Guid InvoiceKey { get; set; }
        public Guid? PaymentKey { get; set; }
        public Guid PaymentMethodKey { get; set; }
        public decimal Amount { get; set; }
        public IEnumerable<KeyValuePair<string, string>> ProcessorArgs { get; set; }
    }
}