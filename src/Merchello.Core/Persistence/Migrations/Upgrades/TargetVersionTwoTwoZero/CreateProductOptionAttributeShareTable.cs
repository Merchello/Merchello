namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoTwoZero
{
    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Creates the new merchProductOptionAttributeShare table.
    /// </summary>
    [Migration("2.2.0", 3, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateProductOptionAttributeShareTable : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The _schema helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _schemaHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProductOptionAttributeShareTable"/> class.
        /// </summary>
        public CreateProductOptionAttributeShareTable()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _schemaHelper = new DatabaseSchemaHelper(dbContext.Database, LoggerResolver.Current.Logger, dbContext.SqlSyntax);
        }

        /// <summary>
        /// Adds the new table to the database.
        /// </summary>
        public override void Up()
        {
            if (!_schemaHelper.TableExist("merchProductOptionAttributeShare"))
            {
                _schemaHelper.CreateTable(false, typeof(ProductOptionAttributeShareDto));
            }
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.2.0 database to a prior version, the database schema has already been modified");
        }
    }
}