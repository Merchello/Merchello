namespace Merchello.Core.Data.Models
{
    using System;

    internal class ProductVariant2ProductAttributeDto
    {
        public Guid ProductVariantKey { get; set; }

        public Guid OptionKey { get; set; }

        public Guid ProductAttributeKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual ProductOptionDto OptionDtoKeyNavigation { get; set; }

        public virtual ProductAttributeDto ProductAttributeDtoKeyNavigation { get; set; }

        public virtual ProductVariantDto ProductVariantDtoKeyNavigation { get; set; }
    }
}