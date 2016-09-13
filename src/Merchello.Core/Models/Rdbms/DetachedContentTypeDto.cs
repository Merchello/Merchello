namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchDetachedContentType" table.
    /// </summary>
    [TableName("merchDetachedContentType")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class DetachedContentTypeDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the entity type field key.
        /// </summary>
        [Column("entityTfKey")]
        public Guid EntityTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column("description")]
        [Length(1000)]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the content type id.
        /// </summary>
        [Column("contentTypeKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? ContentTypeKey { get; set; }
    }
}