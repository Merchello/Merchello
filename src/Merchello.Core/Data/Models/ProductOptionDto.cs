namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class ProductOptionDto
    {
        public ProductOptionDto()
        {
            this.MerchProduct2ProductOption = new HashSet<Product2ProductOptionDto>();
            this.MerchProductAttribute = new HashSet<ProductAttributeDto>();
            this.MerchProductOptionAttributeShare = new HashSet<ProductOptionAttributeShareDto>();
            this.MerchProductVariant2ProductAttribute = new HashSet<ProductVariant2ProductAttributeDto>();
        }

        public Guid Pk { get; set; }

        public string Name { get; set; }

        public Guid? DetachedContentTypeKey { get; set; }

        public bool Required { get; set; }

        public bool Shared { get; set; }

        public string UiOption { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<Product2ProductOptionDto> MerchProduct2ProductOption { get; set; }

        public ICollection<ProductAttributeDto> MerchProductAttribute { get; set; }

        public ICollection<ProductOptionAttributeShareDto> MerchProductOptionAttributeShare { get; set; }

        public ICollection<ProductVariant2ProductAttributeDto> MerchProductVariant2ProductAttribute { get;
            set; }

        public DetachedContentTypeDto DetachedContentTypeDtoKeyNavigation { get; set; }
    }
}