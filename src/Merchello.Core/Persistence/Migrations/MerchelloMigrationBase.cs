namespace Merchello.Core.Persistence.Migrations
{
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The merchello migration base.
    /// </summary>
    internal abstract class MerchelloMigrationBase : MigrationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloMigrationBase"/> class.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The sql syntax.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        protected MerchelloMigrationBase(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
        }

        /// <summary>
        /// The context.
        /// </summary>
        internal IMigrationContext Context;
    }
}