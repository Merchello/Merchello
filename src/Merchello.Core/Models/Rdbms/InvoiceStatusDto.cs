using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInvoiceStatus")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class InvoiceStatusDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        // TODO: RSS should either name or publicName be "alias"
        [Column("publicName")]
        public string PublicName { get; set; }

        [Column("reportable")]
        public bool Reportable { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("actionTriggerId")]
        public int ActionTriggerId { get; set; }

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