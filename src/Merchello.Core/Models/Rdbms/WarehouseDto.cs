using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchWarehouse")]
    [PrimaryKey("id", autoIncrement = false)]
    [ExplicitColumns]
    public class WarehouseDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("address1")]
        public string Address1 { get; set; }

        [Column("address2")]
        public string Address2 { get; set; }

        [Column("locality")]
        public string Locality { get; set; }

        [Column("region")]
        public string Region { get; set; }

        [Column("postalCode")]
        public string PostalCode { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

    }
}