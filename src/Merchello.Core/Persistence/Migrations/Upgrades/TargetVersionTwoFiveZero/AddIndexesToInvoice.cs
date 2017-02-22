namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoFiveZero
{
    using System;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Persistence.DatabaseModelDefinitions;

    using Umbraco.Core;
    using Umbraco.Core.Persistence.Migrations;
    using Umbraco.Core.Persistence.Migrations.Syntax.Create.Index;
    using Umbraco.Core.Persistence.Migrations.Syntax.Expressions;

    /// <summary>
    /// Adds name, price, sale price, barcode and manufacturer indexes to product variant.
    /// </summary>
    [Migration("2.5.0", 1, MerchelloConfiguration.MerchelloMigrationName)]
    public class AddIndexesToInvoice : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// Adds the indexes to the merchInvoice table.
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

            if (dbIndexes == null) throw new NullReferenceException();

            CreateIndex(dbIndexes, "IX_merchInvoiceBillToPostalCode", "billToPostalCode");
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.5.0 database to a prior version, the database schema has already been modified");
        }

        /// <summary>
        /// Creates an index.
        /// </summary>
        /// <param name="dbIndexes">
        /// The collection of database indexes.
        /// </param>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        private void CreateIndex(DbIndexDefinition[] dbIndexes, string indexName, string columnName)
        {
            //// make sure it doesn't already exist
            if (dbIndexes.Any(x => x.IndexName.InvariantEquals(indexName)) == false)
            {
                Logger.Info(typeof(AddIndexesToProductVariant), "Adding nonclustered index to " + columnName + " column on merchInvoice table.");

                var sqlSyntax = ApplicationContext.Current.DatabaseContext.SqlSyntax;

                var dbProvider = ApplicationContext.Current.DatabaseContext.DatabaseProvider;

                var createExpression = new CreateIndexExpression(dbProvider, new[] { dbProvider }, sqlSyntax)
                {
                    Index = { Name = indexName }
                };

                var builder = new CreateIndexBuilder(createExpression);

                builder.OnTable("merchInvoice").OnColumn(columnName).Ascending().WithOptions().NonClustered();

                ApplicationContext.Current.DatabaseContext.Database.Execute(createExpression.ToString());
            }
        }
    }
}