namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchPayment
    {
        public MerchPayment()
        {
            this.MerchAppliedPayment = new HashSet<AppliedPaymentDto>();
        }

        public Guid Pk { get; set; }

        public Guid? CustomerKey { get; set; }

        public Guid? PaymentMethodKey { get; set; }

        public Guid PaymentTfKey { get; set; }

        public string PaymentMethodName { get; set; }

        public string ReferenceNumber { get; set; }

        public decimal Amount { get; set; }

        public bool Authorized { get; set; }

        public bool Collected { get; set; }

        public bool Voided { get; set; }

        public string ExtendedData { get; set; }

        public bool Exported { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<AppliedPaymentDto> MerchAppliedPayment { get; set; }

        public CustomerDto CustomerDtoKeyNavigation { get; set; }

        public MerchPaymentMethod PaymentMethodKeyNavigation { get; set; }
    }
}