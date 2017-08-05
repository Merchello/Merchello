namespace Merchello.Core.Data.Models
{
    using System;
    using System.Collections.Generic;

    internal sealed partial class MerchOrderStatus
    {
        public MerchOrderStatus()
        {
            this.MerchOrder = new HashSet<MerchOrder>();
        }

        public Guid Pk { get; set; }

        public string Name { get; set; }

        public string Alias { get; set; }

        public bool Reportable { get; set; }

        public bool Active { get; set; }

        public int SortOrder { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public ICollection<MerchOrder> MerchOrder { get; set; }
    }
}