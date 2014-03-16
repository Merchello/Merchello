using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchOrderIndex")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class OrderIndexDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("orderKey")]
        [ForeignKey(typeof(OrderDto), Name = "FK_merchOrderIndex_merchOrder", Column = "pk")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_merchOrderIndex")]
        public Guid OrderKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }     
    }
}
