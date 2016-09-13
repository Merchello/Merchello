namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchAnonymousCustomer" table.
    /// </summary>
    [TableName("merchAnonymousCustomer")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class AnonymousCustomerDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the last activity date of the anonymous customer.
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

    }
}
