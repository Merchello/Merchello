namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneElevenZero
{
    using System.Threading;

    using Merchello.Core.Configuration;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.DatabaseAnnotations;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// The alter product variant table.
    /// </summary>
    [Migration("1.10.0", "1.10.0.1", 3, MerchelloConfiguration.MerchelloMigrationName)]
    public class AlterProductVariantTable : MigrationBase 
    {
        /// <summary>
        /// Updates the database.
        /// </summary>
        public override void Up()
        {
            var textType = SqlSyntaxContext.SqlSyntaxProvider.GetSpecialDbType(SpecialDbTypes.NTEXT);
            this.Alter.Table("merchProductVariant").AddColumn("detachedContentValues").AsCustom(textType).Nullable();
            this.Alter.Table("merchProductVariant").AddColumn("detachedContentTypeKey").AsGuid().Nullable();                              
        }

        /// <summary>
        /// The down.
        /// </summary>
        /// <exception cref="DataLossException">
        /// Throws a data loss exception on a downgrade attempt
        /// </exception>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.11.0 database to a prior version, the database schema has already been modified");
        }
    }
}