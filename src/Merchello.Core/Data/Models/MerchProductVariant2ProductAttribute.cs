namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchProductVariant2ProductAttribute
    {
        public Guid ProductVariantKey { get; set; }

        public Guid OptionKey { get; set; }

        public Guid ProductAttributeKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchProductOption OptionKeyNavigation { get; set; }

        public virtual MerchProductAttribute ProductAttributeKeyNavigation { get; set; }

        public virtual MerchProductVariant ProductVariantKeyNavigation { get; set; }
    }
}