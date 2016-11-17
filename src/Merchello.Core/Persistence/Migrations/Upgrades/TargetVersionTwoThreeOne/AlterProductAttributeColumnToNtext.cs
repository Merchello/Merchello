namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoThreeOne
{
    using System.Linq;

    using Merchello.Core.Configuration;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Alters detachedContentValues column to NTEXT.
    /// </summary>
    /// <seealso cref="http://issues.merchello.com/youtrack/issue/M-1225" />
    [Migration("2.3.1", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class AlterProductAttributeColumnToNtext : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Updates the merchProductAttribute table altering detachedContentValues to NTEXT.
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            
            var databaseSchemaHelper = new DatabaseSchemaHelper(database, this.Logger, this.SqlSyntax);
            if (databaseSchemaHelper.TableExist("merchProductAttribute"))
            {
                // Add another assertion that the field size has not already been changed to NTEXT 
                // and some other migration failed which flagged the version to 2.3.0 or earlier
                var size = database.GetDbTableColumnSize("merchProductAttribute", "detachedContentValues");

                if (size > 500) return;

                // Update the column to be NTEXT instead of 255 - BUG FIX
                var textType = this.SqlSyntax.GetSpecialDbType(SpecialDbTypes.NTEXT);
                Alter.Table("merchProductAttribute").AlterColumn("detachedContentValues").AsCustom(textType).Nullable();
            }

            Logger.Debug<AlterProductAttributeColumnToNtext>("AlterProductAttributeColumnToNtext has been called.");
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.3.1 database to a prior version, the database schema has already been modified");
        }
    }
}