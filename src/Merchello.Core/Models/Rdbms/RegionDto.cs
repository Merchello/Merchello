using System;
using System.CodeDom;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchRegion")]
    [PrimaryKey("regionCode", autoIncrement = false)]
    [ExplicitColumns] 
    public class RegionDto
    {
        [Column("regionCode")]
        [Length(2)]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public string RegionCode { get; set; }

        [Column("countryCode")]
        [Length(2)]
        [ForeignKey(typeof(CountryDto), Name = "FK_merchRegion_merchCustomer", Column = "countryCode")]
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