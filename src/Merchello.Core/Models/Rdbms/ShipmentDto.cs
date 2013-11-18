using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipment")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ShipmentDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("orderKey")]
        [ForeignKey(typeof(OrderDto), Name = "FK_merchShipment_merchOrder", Column = "pk")]
        public Guid OrderKey { get; set; }

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

        [Column("email")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Email { get; set; }

        [Column("company")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Company { get; set; }

        [Column("countryCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string CountryCode { get; set; }

        [Column("phone")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Phone { get; set; }

        [Column("shipMethodKey")]
        [ForeignKey(typeof(ShipMethodDto), Name = "FK_merchShipment_merchShipMethod", Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? ShipMethodKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}