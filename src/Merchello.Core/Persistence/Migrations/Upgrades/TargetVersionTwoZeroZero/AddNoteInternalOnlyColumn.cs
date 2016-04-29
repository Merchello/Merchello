namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoZeroZero
{
    using System.Linq;

    using Merchello.Core.Configuration;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Adds an internal only column to the notes table.
    /// </summary>
    [Migration("1.14.0", "2.0.0", 2, MerchelloConfiguration.MerchelloMigrationName)]
    internal class AddNoteInternalOnlyColumn : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The Umbraco database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNoteInternalOnlyColumn"/> class.
        /// </summary>
        public AddNoteInternalOnlyColumn()
            : base(ApplicationContext.Current.DatabaseContext.SqlSyntax, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
        }

        /// <summary>
        /// Upgrades the database.
        /// </summary>
        public override void Up()
        {
            var columns = SqlSyntax.GetColumnsInSchema(_database).ToArray();

            if (
                columns.Any(
                    x => x.TableName.InvariantEquals("merchNote") && x.ColumnName.InvariantEquals("internalOnly"))
                == false)
            {
                Logger.Info(typeof(AddNoteInternalOnlyColumn), "Adding internalOnly column to merchNode table.");

                //// Add the new currency code column
                Create.Column("internalOnly").OnTable("merchNote").AsBoolean().WithDefaultValue(false);
            }
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.0.0 database to a prior version, the database schema has already been modified");
        }
    }
}