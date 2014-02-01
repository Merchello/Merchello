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

        [Column("fromOrganization")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromOrganization { get; set; }

        [Column("fromName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromName { get; set; }

        [Column("fromAddress1")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromAddress1 { get; set; }

        [Column("fromAddress2")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromAddress2 { get; set; }

        [Column("fromLocality")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromLocality { get; set; }

        [Column("fromRegion")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromRegion { get; set; }

        [Column("fromPostalCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromPostalCode { get; set; }

        [Column("fromCountryCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromCountryCode { get; set; }

        [Column("fromIsCommercial")]
        public bool FromIsCommercial { get; set; }

        [Column("toOrganization")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ToOrganization { get; set; }

        [Column("toName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ToName { get; set; }

        [Column("toAddress1")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ToAddress1 { get; set; }

        [Column("toAddress2")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ToAddress2 { get; set; }

        [Column("toLocality")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ToLocality { get; set; }

        [Column("toRegion")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ToRegion { get; set; }

        [Column("toPostalCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ToPostalCode { get; set; }

        [Column("toCountryCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ToCountryCode { get; set; }

        [Column("toIsCommercial")]
        public bool ToIsCommercial { get; set; }

        [Column("phone")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Phone { get; set; }

        [Column("email")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Email { get; set; }

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