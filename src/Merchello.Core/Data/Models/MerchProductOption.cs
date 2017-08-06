namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchProductOption
    {
        public MerchProductOption()
        {
            this.MerchProduct2ProductOption = new HashSet<MerchProduct2ProductOption>();
            this.MerchProductAttribute = new HashSet<MerchProductAttribute>();
            this.MerchProductOptionAttributeShare = new HashSet<MerchProductOptionAttributeShare>();
            this.MerchProductVariant2ProductAttribute = new HashSet<MerchProductVariant2ProductAttribute>();
        }

        public Guid Pk { get; set; }

        public string Name { get; set; }

        public Guid? DetachedContentTypeKey { get; set; }

        public bool Required { get; set; }

        public bool Shared { get; set; }

        public string UiOption { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchProduct2ProductOption> MerchProduct2ProductOption { get; set; }

        public ICollection<MerchProductAttribute> MerchProductAttribute { get; set; }

        public ICollection<MerchProductOptionAttributeShare> MerchProductOptionAttributeShare { get; set; }

        public ICollection<MerchProductVariant2ProductAttribute> MerchProductVariant2ProductAttribute { get;
            set; }

        public DetachedContentTypeDto DetachedContentTypeDtoKeyNavigation { get; set; }
    }
}