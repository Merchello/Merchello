namespace Merchello.Core.Models.Rdbms
{
    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// Internal Umbraco dto used for package installation and removal - in GrantPermissionForApp PackageAction
    /// </summary>
    [TableName("umbracoUser")]
    [PrimaryKey("id", AutoIncrement = true)]
    [ExplicitColumns]
    internal class UserDto
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Column("id")]
        [PrimaryKeyColumn(Name = "PK_user")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Column("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        [Column("userLogin")]
        [Length(125)]
        [Index(IndexTypes.NonClustered)]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Column("userEmail")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user language.
        /// </summary>
        [Column("userLanguage")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(10)]
        public string UserLanguage { get; set; }
    }
}