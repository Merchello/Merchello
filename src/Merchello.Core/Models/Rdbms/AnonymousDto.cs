using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchAnonymous")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class AnonymousDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Pk { get; set; }

        [Column("lastActivityDate")]
        [Constraint(Default = "getdate()")]
        public DateTime LastActivityDate { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
