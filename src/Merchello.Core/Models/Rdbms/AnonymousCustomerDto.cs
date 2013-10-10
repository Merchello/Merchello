using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchAnonymousCustomer")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class AnonymousCustomerDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("memberId")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? MemberId { get; set; }

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
