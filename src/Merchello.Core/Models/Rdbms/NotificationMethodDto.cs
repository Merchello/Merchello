using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchNotificationMethod")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class NotificationMethodDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("providerKey")]
        [ForeignKey(typeof(GatewayProviderSettingsDto), Name = "FK_merchNotificationMethod_merchGatewayProvider", Column = "pk")]
        public Guid ProviderKey { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string Description { get; set; }


        [Column("serviceCode")]
        public string ServiceCode { get; set; }


        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}