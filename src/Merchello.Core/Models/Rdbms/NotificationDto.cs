using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchItemCache")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class NotificationDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("src")]
        public string Src { get; set; }

        [Column("statusKey")]
        public Guid StatusKey { get; set; }

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