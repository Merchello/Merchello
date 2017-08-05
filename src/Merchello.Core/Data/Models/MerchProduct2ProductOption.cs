namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class MerchProduct2ProductOption
    {
        public Guid ProductKey { get; set; }

        public Guid OptionKey { get; set; }

        public string UseName { get; set; }

        public int SortOrder { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchProductOption OptionKeyNavigation { get; set; }

        public virtual MerchProduct ProductKeyNavigation { get; set; }
    }
}