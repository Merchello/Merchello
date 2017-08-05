namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchProductAttribute
    {
        public MerchProductAttribute()
        {
            this.MerchProductOptionAttributeShare = new HashSet<MerchProductOptionAttributeShare>();
            this.MerchProductVariant2ProductAttribute = new HashSet<MerchProductVariant2ProductAttribute>();
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

        public ICollection<MerchProductOptionAttributeShare> MerchProductOptionAttributeShare { get; set; }

        public ICollection<MerchProductVariant2ProductAttribute> MerchProductVariant2ProductAttribute { get;
            set; }

        public MerchProductOption OptionKeyNavigation { get; set; }
    }
}