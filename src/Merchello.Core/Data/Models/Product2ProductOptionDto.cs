namespace Merchello.Core.Data.Models
{
    using System;

    internal class Product2ProductOptionDto
    {
        public Guid ProductKey { get; set; }

        public Guid OptionKey { get; set; }

        public string UseName { get; set; }

        public int SortOrder { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual ProductOptionDto OptionDtoKeyNavigation { get; set; }

        public virtual ProductDto ProductDtoKeyNavigation { get; set; }
    }
}