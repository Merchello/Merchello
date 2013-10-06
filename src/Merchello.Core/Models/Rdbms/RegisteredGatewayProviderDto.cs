using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchRegisteredGatewayProvider")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class RegisteredGatewayProviderDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Key { get; set; }

        [Column("gatewayProviderTypeFieldKey")]
        public Guid GatewayProviderTypeFieldKey { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("typeFullName")]
        [Length(255)]
        public string TypeFullName { get; set; }

        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        [Column("encryptExtendedData")]
        [Constraint(Default = "0")]
        public bool EncryptExtendedData { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}