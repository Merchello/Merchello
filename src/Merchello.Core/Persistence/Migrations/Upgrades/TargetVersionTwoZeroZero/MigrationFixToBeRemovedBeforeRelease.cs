namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoZeroZero
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Previous testing migrations have notes column in customer field removed (which it can't be since we have to alter the customerDto to remove the notes column ref).
    /// Removing this field will need to be done in the 2.1.0 release
    /// </summary>
    [Migration("1.14.0", "2.0.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class MigrationFixToBeRemovedBeforeRelease : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The Umbraco database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationFixToBeRemovedBeforeRelease"/> class.
        /// </summary>
        public MigrationFixToBeRemovedBeforeRelease()
            : base(ApplicationContext.Current.DatabaseContext.SqlSyntax, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
        }

        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            //// Don't exeucte if the column is already there
            var columns = SqlSyntax.GetColumnsInSchema(_database).ToArray();
            if (columns.Any(x => x.TableName.InvariantEquals("merchCustomer") && x.ColumnName.InvariantEquals("notes") == false))
            {
                Create.Column("notes").OnTable("merchCustomer").AsString().Nullable();
            }
        }


        public override void Down()
        {
            throw new System.NotImplementedException();
        }
    }
}