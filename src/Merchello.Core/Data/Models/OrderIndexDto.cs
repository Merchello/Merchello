namespace Merchello.Core.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    // TODO drop this class
    // FYI we are not going to use Examine for orders
    internal class OrderIndexDto
    {
        public int Id { get; set; }

        [ForeignKey("MerchOrder")]
        public Guid OrderKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

       // public virtual MerchOrder OrderKeyNavigation { get; set; }
    }
}