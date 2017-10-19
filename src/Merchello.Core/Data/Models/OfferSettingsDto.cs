namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class OfferSettingsDto
    {
        public OfferSettingsDto()
        {
            this.MerchOfferRedeemed = new HashSet<OfferRedeemedDto>();
        }

        public Guid Pk { get; set; }

        public string Name { get; set; }

        public string OfferCode { get; set; }

        public Guid OfferProviderKey { get; set; }

        public DateTime? OfferStartsDate { get; set; }

        public DateTime? OfferEndsDate { get; set; }

        public bool Active { get; set; }

        public string ConfigurationData { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<OfferRedeemedDto> MerchOfferRedeemed { get; set; }
    }
}