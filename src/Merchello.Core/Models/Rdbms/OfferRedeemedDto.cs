namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchOfferRedeemed" table.
    /// </summary>
    [TableName("merchOfferRedeemed")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class OfferRedeemedDto : EntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the offer settings key.
        /// </summary>
        /// <remarks>
        /// This accepts a null so that the offer can be deleted without having to 
        /// delete this reference
        /// </remarks>
        [Column("offerSettingsKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [ForeignKey(typeof(OfferSettingsDto), Name = "FK_merchOfferRedeemed_merchOfferSettings", Column = "pk")]
        public Guid? OfferSettingsKey { get; set; }
        
        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        [Column("offerCode")]
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        /// <remarks>
        /// This does not need to allow nulls since the key is not associated
        /// with a database constraint.
        /// </remarks>
        [Column("offerProviderKey")]
        public Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        /// <remarks>
        /// Not all offers will be associated with known customers (could be anonymous)
        /// </remarks>
        [Column("customerKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        /// <remarks>
        /// If the invoice is deleted - so will this record
        /// </remarks>
        [Column("invoiceKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchOfferRedeemed_merchInvoice", Column = "pk")]
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the redeemed date.
        /// </summary>
        [Column("redeemedDate")]
        public DateTime RedeemedDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data serialization.
        /// </summary>
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        [Column("extendedData")]
        public string ExtendedData { get; set; }      
    }
}