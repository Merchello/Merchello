namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class ProductAttributeDto
    {
        public ProductAttributeDto()
        {
            this.MerchProductOptionAttributeShare = new HashSet<ProductOptionAttributeShareDto>();
            this.MerchProductVariant2ProductAttribute = new HashSet<ProductVariant2ProductAttributeDto>();
        }

        public Guid Pk { get; set; }

        public Guid OptionKey { get; set; }

        public string Name { get; set; }

        public string Sku { get; set; }

        public string DetachedContentValues { get; set; }

        public int SortOrder { get; set; }

        public bool IsDefaultChoice { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<ProductOptionAttributeShareDto> MerchProductOptionAttributeShare { get; set; }

        public ICollection<ProductVariant2ProductAttributeDto> MerchProductVariant2ProductAttribute { get;
            set; }

        public ProductOptionDto OptionDtoKeyNavigation { get; set; }
    }
}