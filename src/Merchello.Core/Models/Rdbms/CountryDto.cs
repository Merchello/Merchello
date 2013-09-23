using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCountry")]
    [PrimaryKey("countryCode", autoIncrement = false)]
    [ExplicitColumns]
    internal class CountryDto
    {
        [Column("countryCode")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Length(2)]
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