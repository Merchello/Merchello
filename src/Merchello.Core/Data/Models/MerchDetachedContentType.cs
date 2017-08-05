namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchDetachedContentType
    {
        public MerchDetachedContentType()
        {
            this.MerchProductOption = new HashSet<MerchProductOption>();
            this.MerchProductVariantDetachedContent = new HashSet<MerchProductVariantDetachedContent>();
        }

        public Guid Pk { get; set; }

        public Guid EntityTfKey { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? ContentTypeKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchProductOption> MerchProductOption { get; set; }

        public ICollection<MerchProductVariantDetachedContent> MerchProductVariantDetachedContent { get; set; }
    }
}