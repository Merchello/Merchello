using Merchello.Core.Configuration;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoSevenZeroTwo
{
    /// <summary>
    /// Adds the digital media table.
    /// </summary>
    [Migration("2.7.0.2", 2, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddVirtualVariantsTable : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The _schema helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _schemaHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddVirtualVariantsTable"/> class.
        /// </summary>
        public AddVirtualVariantsTable()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _schemaHelper = new DatabaseSchemaHelper(dbContext.Database, LoggerResolver.Current.Logger, dbContext.SqlSyntax);
        }

        /// <summary>
        /// Adds the indexes to the merchDigitalMedia table.
        /// </summary>
        public override void Up()
        {
            if (!_schemaHelper.TableExist("merchVirtualVariants"))
            {
                _schemaHelper.CreateTable(false, typeof(VirtualVariantsDto));
            }
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.5.0 database to a prior version, the database schema has already been modified");
        }
    }
}