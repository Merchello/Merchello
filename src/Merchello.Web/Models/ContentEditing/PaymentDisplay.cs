using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class PaymentDisplay
    {
        public Guid Key { get; set; }
        public Guid? CustomerKey { get; set; }
        public Guid? PaymentMethodKey { get; set; }
        public Guid PaymentTypeFieldKey { get; set; }
        public string PaymentMethodName { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal Amount { get; set; }
        public bool Authorized { get; set; }
        public bool Collected { get; set; }
        public bool Exported { get; set; }
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }
        public IEnumerable<AppliedPaymentDisplay> AppliedPayments { get; set; }
    }
}