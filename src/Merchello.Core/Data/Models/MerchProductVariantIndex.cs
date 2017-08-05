namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchProductVariantIndex
    {
        public int Id { get; set; }

        public Guid ProductVariantKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchProductVariant ProductVariantKeyNavigation { get; set; }
    }
}