using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInvoiceStatus")]
    [PrimaryKey("id", autoIncrement = false)]
    [ExplicitColumns]
    public class InvoiceStatusDto
    {
        [Column("id")]
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

    }
}