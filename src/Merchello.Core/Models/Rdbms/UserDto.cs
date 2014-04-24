using System.Collections.Generic;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    /// <summary>
    /// Internal Umbraco dto used for package installation and removal - in GrantPermissionForApp PackageAction
    /// </summary>
    [TableName("umbracoUser")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    internal class UserDto
    {
        [Column("id")]
        [PrimaryKeyColumn(Name = "PK_user")]
        public int Id { get; set; }        

        [Column("userName")]
        public string UserName { get; set; }

        [Column("userLogin")]
        [Length(125)]
        [Index(IndexTypes.NonClustered)]
        public string Login { get; set; }

        [Column("userEmail")]
        public string Email { get; set; }
        
        [Column("userLanguage")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(10)]
        public string UserLanguage { get; set; }

    }
}