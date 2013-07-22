using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipment")]
    [PrimaryKey("id", autoIncrement = false)]
    [ExplicitColumns]
    public class ShipmentDto
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("invoiceId")]
        public int InvoiceId { get; set; }

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

        [Column("countryCode")]
        public string CountryCode { get; set; }

        [Column("shipMethodId")]
        public int ShipMethodId { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

    }
}