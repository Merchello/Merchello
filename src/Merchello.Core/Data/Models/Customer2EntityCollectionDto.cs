namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class Customer2EntityCollectionDto
    {
        public Guid CustomerKey { get; set; }

        public Guid EntityCollectionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual CustomerDto CustomerDtoKeyNavigation { get; set; }

        public virtual EntityCollectionDto EntityCollectionDtoKeyNavigation { get; set; }
    }
}