using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchOrderStatus")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class OrderStatusDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("alias")]
        public string Alias { get; set; }

        [Column("reportable")]
        public bool Reportable { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("sortOrder")]
        public int SortOrder { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}