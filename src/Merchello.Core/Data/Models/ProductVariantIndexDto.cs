namespace Merchello.Core.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    internal class ProductVariantIndexDto
    {
        public int Id { get; set; }

        [ForeignKey("MerchProductVariant")]
        public Guid ProductVariantKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        //public virtual MerchProductVariant ProductVariantKeyNavigation { get; set; }
    }
}