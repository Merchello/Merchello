namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneElevenZero
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    //using DatabaseSchemaHelper = Merchello.Core.Persistence.Migrations.DatabaseSchemaHelper;

    /// <summary>
    /// The create customer 2 entity collection table.
    /// </summary>
    [Migration("1.10.0", "1.11.0", 6, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateCustomer2EntityCollectionTable : MigrationBase
    {
        /// <summary>
        /// Tables in the order of creation or reverse deletion.
        /// </summary>
        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            { 0, typeof(Customer2EntityCollectionDto) }
        };


        /// <summary>
        /// Adds the table to the database
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            if (!database.TableExist("merchCustomer2EntityCollection"))
            {
                //DatabaseSchemaHelper.InitializeDatabaseSchema(database, OrderedTables, "Merchello 1.11.0 upgrade");
            }
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