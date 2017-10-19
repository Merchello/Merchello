namespace Merchello.Core.Data.Models
{
    using System;

    internal class OfferRedeemedDto
    {
        public Guid Pk { get; set; }

        public Guid? OfferSettingsKey { get; set; }

        public string OfferCode { get; set; }

        public Guid OfferProviderKey { get; set; }

        public Guid? CustomerKey { get; set; }

        public Guid InvoiceKey { get; set; }

        public DateTime RedeemedDate { get; set; }

        public string ExtendedData { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual InvoiceDto InvoiceDtoKeyNavigation { get; set; }

        public virtual OfferSettingsDto OfferSettingsDtoKeyNavigation { get; set; }
    }
}