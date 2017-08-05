namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchProductOptionAttributeShare
    {
        public Guid ProductKey { get; set; }

        public Guid OptionKey { get; set; }

        public Guid AttributeKey { get; set; }

        public bool IsDefaultChoice { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchProductAttribute AttributeKeyNavigation { get; set; }

        public virtual MerchProductOption OptionKeyNavigation { get; set; }

        public virtual MerchProduct ProductKeyNavigation { get; set; }
    }
}