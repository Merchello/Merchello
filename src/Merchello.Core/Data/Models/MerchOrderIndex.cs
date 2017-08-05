namespace Merchello.Core.Data.Models
{
    using System;

    // TODO drop this class
    // FYI we are not going to use Examine for orders
    internal partial class MerchOrderIndex
    {
        public int Id { get; set; }

        public Guid OrderKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual MerchOrder OrderKeyNavigation { get; set; }
    }
}