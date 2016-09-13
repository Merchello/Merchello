namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchCustomer2EntityCollection" table.
    /// </summary>
    [TableName("merchCustomer2EntityCollection")]
    [PrimaryKey("customerKey", AutoIncrement = false)]
    [ExplicitColumns]
    internal class Customer2EntityCollectionDto : IDto
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        [Column("customerKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchCustomer2EntityCollection", OnColumns = "customerKey, entityCollectionKey")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchCustomer2EntityCollection_merchInvoice", Column = "pk")]
        public Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the product collection key.
        /// </summary>
        [Column("entityCollectionKey")]
        [ForeignKey(typeof(EntityCollectionDto), Name = "FK_merchCustomer2EntityCollection_merchEntityCollection", Column = "pk")]
        public Guid EntityCollectionKey { get; set; }

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