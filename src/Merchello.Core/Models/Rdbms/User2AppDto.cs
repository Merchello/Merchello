using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    /// <summary>
    /// Internal Umbraco dto used for package installation and removal - in GrantPermissionForApp PackageAction
    /// </summary>
    [TableName("umbracoUser2app")]
    [ExplicitColumns]
    internal class User2AppDto
    {
        [Column("user")]
        public int UserId { get; set; }

        [Column("app")]
        [Length(50)]
        public string AppAlias { get; set; }
    }
}