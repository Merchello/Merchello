namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed class ProductDto
    {
        public ProductDto()
        {
            this.MerchProduct2EntityCollection = new HashSet<Product2EntityCollectionDto>();
            this.MerchProduct2ProductOption = new HashSet<Product2ProductOptionDto>();
            this.MerchProductOptionAttributeShare = new HashSet<ProductOptionAttributeShareDto>();
            this.MerchProductVariant = new HashSet<ProductVariantDto>();
        }

        public Guid Pk { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<Product2EntityCollectionDto> MerchProduct2EntityCollection { get; set; }

        public ICollection<Product2ProductOptionDto> MerchProduct2ProductOption { get; set; }

        public ICollection<ProductOptionAttributeShareDto> MerchProductOptionAttributeShare { get; set; }

        public ICollection<ProductVariantDto> MerchProductVariant { get; set; }
    }
}