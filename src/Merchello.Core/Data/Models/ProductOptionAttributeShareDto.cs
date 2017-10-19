namespace Merchello.Core.Data.Models
{
    using System;

    internal class ProductOptionAttributeShareDto
    {
        public Guid ProductKey { get; set; }

        public Guid OptionKey { get; set; }

        public Guid AttributeKey { get; set; }

        public bool IsDefaultChoice { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual ProductAttributeDto AttributeDtoKeyNavigation { get; set; }

        public virtual ProductOptionDto OptionDtoKeyNavigation { get; set; }

        public virtual ProductDto ProductDtoKeyNavigation { get; set; }
    }
}