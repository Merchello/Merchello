namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The detached content type dto.
    /// </summary>
    [TableName("merchDetachedContentType")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class DetachedContentTypeDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

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