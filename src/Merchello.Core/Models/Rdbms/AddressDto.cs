using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchAddress")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class AddressDto 
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("customerPk")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchAddress_merchCustomer", Column = "pk")]
        public Guid CustomerPk { get; set; }

        [Column("label")]
        public string Label { get; set; }

        [Column("fullName")]
        public string FullName { get; set; }

        [Column("company")]
        public string Company { get; set; }

        [Column("addressTypeKey")]
        public Guid AddressTypeFieldKey { get; set; }

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

        [Column("countryCode")]
        public string CountryCode { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}