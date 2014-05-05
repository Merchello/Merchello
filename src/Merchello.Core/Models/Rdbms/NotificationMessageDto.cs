using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchNotificationMessage")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class NotificationMessageDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

       
        [Column("methodKey")]
        [ForeignKey(typeof(NotificationMethodDto), Name = "FK_merchNotificationMessage_merchNotificationMethod", Column = "pk")]
        public Guid MethodKey { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }


        [Column("message")]
        public string Message { get; set; }

        [Column("maxLength")]
        public int MaxLength { get; set; }

        [Column("messageIsFilePath")]
        public bool MessageIsFilePath { get; set; }

        [Column("recipients")]
        public string Recipients { get; set; }

        [Column("sendToCustomer")]
        public bool SendToCustomer { get; set; }

        [Column("disabled")]
        public bool Disabled { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}