namespace Merchello.Core.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    internal partial class AnonymousCustomerDto
    {
        public Guid Pk { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string ExtendedData { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
