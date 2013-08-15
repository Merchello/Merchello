using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchShipment")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class ShipmentDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("invoiceId")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchShipment_merchInvoice", Column = "id")]
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
        [ForeignKey(typeof(ShipMethodDto), Name = "FK_merchShipment_merchShipMethod", Column = "id")]
        public int ShipMethodId { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}