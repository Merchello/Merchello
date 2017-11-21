using System;
using System.Linq;
using Merchello.Core.Configuration;
using Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneFourteenZero;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionTwoSevenZero
{
    /// <summary>
    /// Alters the product table to add a virtual variants column
    /// </summary>
    [Migration("2.7.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    internal class AddVirtualVariantsToProduct : MerchelloMigrationBase, IMerchelloMigration
    {
        /// <summary>
        /// The Umbraco database.
        /// </summary>
        private readonly Database _database;

        /// <summary>
        /// The SQL syntax provider.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntax;


        /// <summary>
        /// Initializes a new instance of the <see cref="AddVirtualVariantsToProduct"/> class.
        /// </summary>
        public AddVirtualVariantsToProduct()
            : base(
                ApplicationContext.Current.DatabaseContext.SqlSyntax,
                Umbraco.Core.Logging.Logger.CreateWithDefaultLog4NetConfiguration())
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _database = dbContext.Database;
            _sqlSyntax = dbContext.SqlSyntax;
        }

        /// <summary>
        /// Upgrades the database.
        /// </summary>
        public override void Up()
        {
            //// Don't exeucte if the column is already there
            /// 
            var columns = _sqlSyntax.GetColumnsInSchema(_database).ToArray();
            if (
                columns.Any(
                    x => x.TableName.InvariantEquals("merchProduct") && x.ColumnName.InvariantEquals("virtualVariants"))
                == false)
            {
                Logger.Info(typeof(AddInvoiceCurrencyCodeColumn), "Adding virtualVariants column to merchProduct table.");

                //// Add the new virtual variants column
                Create.Column("virtualVariants").OnTable("merchProduct").AsBoolean().WithDefaultValue(false);
                
            }
        }

        /// <summary>
        /// Downgrades the database.
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 2.7.0 database to a prior version, the database schema has already been modified");
        }
        
    }
}