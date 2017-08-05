namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchProduct
    {
        public MerchProduct()
        {
            this.MerchProduct2EntityCollection = new HashSet<MerchProduct2EntityCollection>();
            this.MerchProduct2ProductOption = new HashSet<MerchProduct2ProductOption>();
            this.MerchProductOptionAttributeShare = new HashSet<MerchProductOptionAttributeShare>();
            this.MerchProductVariant = new HashSet<MerchProductVariant>();
        }

        public Guid Pk { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchProduct2EntityCollection> MerchProduct2EntityCollection { get; set; }

        public ICollection<MerchProduct2ProductOption> MerchProduct2ProductOption { get; set; }

        public ICollection<MerchProductOptionAttributeShare> MerchProductOptionAttributeShare { get; set; }

        public ICollection<MerchProductVariant> MerchProductVariant { get; set; }
    }
}