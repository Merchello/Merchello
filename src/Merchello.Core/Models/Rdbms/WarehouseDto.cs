namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The warehouse dto.
    /// </summary>
    [TableName("merchWarehouse")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class WarehouseDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the address 1.
        /// </summary>
        [Column("address1")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2.
        /// </summary>
        [Column("address2")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        [Column("locality")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        [Column("region")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [Column("postalCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        [Column("countryCode")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        [Column("phone")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Column("email")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is default.
        /// </summary>
        [Column("isDefault")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}