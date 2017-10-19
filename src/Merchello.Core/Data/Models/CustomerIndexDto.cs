namespace Merchello.Core.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    // TODO drop this class
    // FYI we are not going to use Examine for customers
    internal partial class CustomerIndexDto
    {
        public int Id { get; set; }

        [ForeignKey("CustomerDto")]
        public Guid CustomerKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        //public virtual CustomerDto CustomerDtoKeyNavigation { get; set; }
    }
}