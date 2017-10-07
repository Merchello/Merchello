namespace Merchello.Core.Models.Rdbms
{
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    [TableName("umbracoUserGroup2App")]
    [ExplicitColumns]
    internal class UserGroup2AppDto
    {
        [Column("userGroupId")]
        public int UserGroupId { get; set; }
    
        [Column("app")]
        [Length(50)]
        public string AppAlias { get; set; }
    }
}