namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The notification message dto.
    /// </summary>
    [TableName("merchNotificationMessage")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class NotificationMessageDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the method key.
        /// </summary>
        [Column("methodKey")]
        [ForeignKey(typeof(NotificationMethodDto), Name = "FK_merchNotificationMessage_merchNotificationMethod", Column = "pk")]
        public Guid MethodKey { get; set; }

        /// <summary>
        /// Gets or sets the monitor key.
        /// </summary>
        [Column("monitorKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid? MonitorKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [Column("description")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the from address.
        /// </summary>
        [Column("fromAddress")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string FromAddress { get; set; }

        /// <summary>
        /// Gets or sets the reply to.
        /// </summary>
        [Column("replyTo")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        [Column("bodyText")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string BodyText { get; set; }

        /// <summary>
        /// Gets or sets the max length.
        /// </summary>
        [Column("maxLength")]
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether body text is file path.
        /// </summary>
        [Column("bodyTextIsFilePath")]
        public bool BodyTextIsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the recipients.
        /// </summary>
        [Column("recipients")]
        public string Recipients { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether send to customer.
        /// </summary>
        [Column("sendToCustomer")]
        public bool SendToCustomer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether disabled.
        /// </summary>
        [Column("disabled")]
        public bool Disabled { get; set; }

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