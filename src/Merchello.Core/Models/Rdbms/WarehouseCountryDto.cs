using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchWarehouseCountry")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class WarehouseCountryDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("warehouseKey")]
        [ForeignKey(typeof(WarehouseDto), Name = "FK_merchWarehouseCountry_merchWarehouse", Column = "pk")]
        public Guid WarehouseKey { get; set; }

        [Column("countryCode")]
        public string CountryCode { get; set; }

        [Column("name")]
        public string Name { get; set; }


        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}