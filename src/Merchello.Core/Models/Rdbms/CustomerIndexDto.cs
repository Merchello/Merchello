namespace Merchello.Core.Models.Rdbms
{
    using System;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The customer index dto.
    /// </summary>
    [TableName("merchCustomerIndex")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class CustomerIndexDto
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        [Column("customerKey")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchCustomerIndex_merchCustomer", Column = "pk")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_merchCustomerIndex")]
        public Guid CustomerKey { get; set; }

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
