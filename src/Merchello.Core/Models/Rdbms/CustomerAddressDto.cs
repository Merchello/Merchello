using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomerAddress")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class CustomerAddressDto 
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("customerId")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchCustomerAddress_merchCustomer", Column = "id")]
        public int CustomerId { get; set; }

        [Column("label")]
        public string Label { get; set; }

        [Column("fullName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FullName { get; set; }

        [Column("company")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Company { get; set; }

        [Column("addressTfKey")]
        public Guid AddressTfKey { get; set; }

        [Column("address1")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Address1 { get; set; }

        [Column("address2")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Address2 { get; set; }

        [Column("locality")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Locality { get; set; }

        [Column("region")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Region { get; set; }

        [Column("postalCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string PostalCode { get; set; }

        [Column("countryCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string CountryCode { get; set; }

        [Column("phone")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Phone { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}