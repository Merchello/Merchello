using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchExtendedData")]
    [PrimaryKey("foreignKeyId", autoIncrement = false)]
    [ExplicitColumns]
    public class ExtendedDataDto
    {
        [Column("foreignKeyId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchExtendedData", OnColumns = "foreignKeyId, tableName, propertyName")]
        public int ForeignKeyId { get; set; }

        [Column("tableName")]
        public string TableName { get; set; }

        [Column("propertyName")]
        public string PropertyName { get; set; }

        [Column("propertyValue")]
        public string PropertyValue { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
