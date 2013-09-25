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

        [Column("configurationData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ConfigurationData { get; set; }

        [Column("encryptConfigurationData")]
        [Constraint(Default = "0")]
        public bool EncryptConfigurationData { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}