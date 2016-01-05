namespace Merchello.Core.Persistence.Migrations
{
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    internal abstract class MerchelloMigrationBase : MigrationBase
    {
        protected MerchelloMigrationBase(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
        }

        internal IMigrationContext Context;
    }
}