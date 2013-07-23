using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomerAddress")]
    [PrimaryKey("id", autoIncrement = false)]
    [ExplicitColumns]
    public class CustomerAddressDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("customerId")]
        public int? CustomerId { get; set; }

        [Column("label")]
        public string Label { get; set; }

        [Column("fullName")]
        public string FullName { get; set; }

        [Column("company")]
        public string Company { get; set; }

        [Column("type")]
        public int? Type { get; set; }

        [Column("address1")]
        public string Address1 { get; set; }

        [Column("address2")]
        public string Address2 { get; set; }

        [Column("locality")]
        public string Locality { get; set; }

        [Column("region")]
        public string Region { get; set; }

        [Column("postalCode")]
        public string PostalCode { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

    }
}