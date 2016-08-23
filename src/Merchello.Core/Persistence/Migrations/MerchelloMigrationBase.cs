namespace Merchello.Core.Persistence.Migrations
{
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The merchello migration base.
    /// </summary>
    public abstract class MerchelloMigrationBase : MigrationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloMigrationBase"/> class.
        /// </summary>
        protected MerchelloMigrationBase()
            : this(ApplicationContext.Current.DatabaseContext.SqlSyntax, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloMigrationBase"/> class.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        protected MerchelloMigrationBase(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
        }
    }
}