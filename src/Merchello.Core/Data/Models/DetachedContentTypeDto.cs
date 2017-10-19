namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class DetachedContentTypeDto
    {
        public DetachedContentTypeDto()
        {
            this.MerchProductOption = new HashSet<ProductOptionDto>();
            this.MerchProductVariantDetachedContent = new HashSet<ProductVariantDetachedContentDto>();
        }

        public Guid Pk { get; set; }

        public Guid EntityTfKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? ContentTypeKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<ProductOptionDto> MerchProductOption { get; set; }

        public ICollection<ProductVariantDetachedContentDto> MerchProductVariantDetachedContent { get; set; }
    }
}