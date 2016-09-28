namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchNote" table.
    /// </summary>
    [TableName("merchNote")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class NoteDto : EntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        [Column("entityKey")]
        public Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the reference type.
        /// </summary>
        [Column("entityTfKey")]
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        [Column("author")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [Column("message")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the note should be for internal use only.
        /// </summary>
        [Column("internalOnly")]
        public bool InternalOnly { get; set; }
    }
}