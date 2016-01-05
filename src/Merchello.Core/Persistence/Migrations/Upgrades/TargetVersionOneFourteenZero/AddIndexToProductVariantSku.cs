namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneFourteenZero
{
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.DatabaseModelDefinitions;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Alters the merchInvoice table to add a currency code column.
    /// </summary>
    [Migration("1.13.0", "1.13.5", 1, MerchelloConfiguration.MerchelloMigrationName)]
    internal class AddIndexToProductVariantSku : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddIndexToProductVariantSku"/> class.
        /// </summary>
        public AddIndexToProductVariantSku()
            : base(ApplicationContext.Current.DatabaseContext.SqlSyntax, Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
        }

        /// <summary>
        /// Adds the unique index to the SKU
        /// </summary>
        public override void Up()
        {
            var dbIndexes = SqlSyntax.GetDefinedIndexes(ApplicationContext.Current.DatabaseContext.Database)
                .Select(x => new DbIndexDefinition
                {
                    TableName = x.Item1,
                    IndexName = x.Item2,
                    ColumnName = x.Item3,
                    IsUnique = x.Item4
                }).ToArray();

            //// make sure it doesn't already exist
            if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_merchProductVariantSku")) == false)
            {
                Logger.Info(typeof(AddInvoiceCurrencyCodeColumn), "Adding unique nonclustered index to sku column on merchProductVariant table.");

                Create.Index("IX_merchProductVariantSku").OnTable("merchProductVariant").OnColumn("sku").Unique();
            }
        }

        /// <summary>
        /// Removes the index
        /// </summary>
        public override void Down()
        {
            Delete.Index("IX_merchProductVariantSku").OnTable("merchProductVariant");
        }
    }
}