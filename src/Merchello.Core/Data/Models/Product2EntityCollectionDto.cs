namespace Merchello.Core.Data.Models
{
    using System;

    internal class Product2EntityCollectionDto
    {
        public Guid ProductKey { get; set; }

        public Guid EntityCollectionKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual EntityCollectionDto EntityCollectionDtoKeyNavigation { get; set; }

        public virtual ProductDto ProductDtoKeyNavigation { get; set; }
    }
}