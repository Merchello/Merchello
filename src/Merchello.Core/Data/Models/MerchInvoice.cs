namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchInvoice
    {
        public MerchInvoice()
        {
            this.MerchAppliedPayment = new HashSet<AppliedPaymentDto>();
            this.MerchInvoice2EntityCollection = new HashSet<MerchInvoice2EntityCollection>();
            this.MerchInvoiceItem = new HashSet<MerchInvoiceItem>();
            this.MerchOfferRedeemed = new HashSet<MerchOfferRedeemed>();
            this.MerchOrder = new HashSet<MerchOrder>();
        }

        public Guid Pk { get; set; }

        public Guid? CustomerKey { get; set; }

        public string InvoiceNumberPrefix { get; set; }

        public int InvoiceNumber { get; set; }

        public string PoNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public Guid InvoiceStatusKey { get; set; }

        public Guid VersionKey { get; set; }

        public string BillToName { get; set; }

        public string BillToAddress1 { get; set; }

        public string BillToAddress2 { get; set; }

        public string BillToLocality { get; set; }

        public string BillToRegion { get; set; }

        public string BillToPostalCode { get; set; }

        public string BillToCountryCode { get; set; }

        public string BillToEmail { get; set; }

        public string BillToPhone { get; set; }

        public string BillToCompany { get; set; }

        public string CurrencyCode { get; set; }

        public bool Exported { get; set; }

        public bool Archived { get; set; }

        public decimal Total { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<AppliedPaymentDto> MerchAppliedPayment { get; set; }

        public ICollection<MerchInvoice2EntityCollection> MerchInvoice2EntityCollection { get; set; }

        public MerchInvoiceIndex MerchInvoiceIndex { get; set; }

        public ICollection<MerchInvoiceItem> MerchInvoiceItem { get; set; }

        public ICollection<MerchOfferRedeemed> MerchOfferRedeemed { get; set; }

        public ICollection<MerchOrder> MerchOrder { get; set; }

        public MerchCustomer CustomerKeyNavigation { get; set; }

        public MerchInvoiceStatus InvoiceStatusKeyNavigation { get; set; }
    }
}