using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipMethod")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class ShipMethodDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("providerKey")]
        public Guid ProviderKey { get; set; }

        [Column("shipMethodTfKey")]
        public Guid ShipMethodTfKey { get; set; }

        [Column("surcharge")]
        [Constraint(Default = "0")]
        public decimal Surcharge { get; set; }        

        [Column("serviceCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ServiceCode { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}