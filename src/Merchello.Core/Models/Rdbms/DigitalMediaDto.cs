namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    ///// TODO SR - This is just a scaffold for example.  Do whatever you need to do =)

    /// <summary>
    /// The Digital Media DTO.
    /// </summary>
    [TableName("merchDigitalMedia")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class DigitalMediaDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        // TODO add other handler columns here

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <remarks>
        /// TODO - you may not want this column - I've just added this as an example for you to follow through the layers
        /// </remarks>
        [Column("name")]
        public string Name { get; set; }

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