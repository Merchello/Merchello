namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The customer dto.
    /// </summary>
    [TableName("merchCustomer")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class CustomerDto : IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the login name.
        /// </summary>
        [Column("loginName")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchCustomerLoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [Column("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Column("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tax exempt.
        /// </summary>
        [Column("taxExempt")]
        public bool TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the last activity date.
        /// </summary>
        [Column("lastActivityDate")]
        [Constraint(Default = "getdate()")]
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [Column("notes")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Notes { get; set; }

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

        /// <summary>
        /// Gets or sets the customer index dto.
        /// </summary>
        [ResultColumn]
        public CustomerIndexDto CustomerIndexDto { get; set; }
    }
}