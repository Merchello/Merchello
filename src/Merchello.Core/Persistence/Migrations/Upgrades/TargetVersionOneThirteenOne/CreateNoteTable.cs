namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneThirteenOne
{
    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Responsible for creating the merchNote table on upgrading to 1.13.1.
    /// </summary>
    [Migration("1.13.0", "1.13.1", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateNoteTable : IMerchelloMigration
    {
        /// <summary>
        /// The _database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// The schema helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _schemaHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateNoteTable"/> class.
        /// </summary>
        public CreateNoteTable()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
            _schemaHelper = new DatabaseSchemaHelper(dbContext.Database, LoggerResolver.Current.Logger, dbContext.SqlSyntax);
        }

        /// <summary>
        /// Performs the task of adding the table.
        /// </summary>
        public void Up()
        {
            if (!_schemaHelper.TableExist("merchNote"))
            {
                _schemaHelper.CreateTable(false, typeof(NoteDto));
            }
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        /// <exception cref="DataLossException">
        /// Throws a data loss exception
        /// </exception>
        public void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.13.1 database to a prior version, the database schema has already been modified");
        }
    }
}