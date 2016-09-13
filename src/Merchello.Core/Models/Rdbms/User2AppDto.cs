namespace Merchello.Core.Models.Rdbms
{
    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// Internal Umbraco dto used for package installation and removal - in GrantPermissionForApp PackageAction
    /// </summary>
    [TableName("umbracoUser2app")]
    [ExplicitColumns]
    internal class User2AppDto
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [Column("user")]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the app alias.
        /// </summary>
        [Column("app")]
        [Length(50)]
        public string AppAlias { get; set; }
    }
}