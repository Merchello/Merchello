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

        [Column("sortOrder")]
        public int SortOrder { get; set; }

    }
}