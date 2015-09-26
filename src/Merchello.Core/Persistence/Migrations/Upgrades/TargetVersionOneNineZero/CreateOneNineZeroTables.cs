namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneNineZero
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Adds new database tables .
    /// </summary>
    [Migration("1.8.2", "1.9.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateOneNineZeroTables : MigrationBase
    {
        /// <summary>
        /// Tables in the order of creation or reverse deletion.
        /// </summary>
        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            { 0, typeof(OfferSettingsDto) },
            { 1, typeof(OfferRedeemedDto) }
            // TODO add the digital media table
            //{ 2, typeof(DigitalMediaDto) }
        };

        /// <summary>
        /// The up.
        /// </summary>
        public override void Up()
        {
            var database = ApplicationContext.Current.DatabaseContext.Database;
            //DatabaseSchemaHelper.InitializeDatabaseSchema(database, OrderedTables, "1.9.0 upgrade");
        }

        /// <summary>
        /// Throws a data loss exception
        /// </summary>
        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.9.0 database to a prior version, the database schema has already been modified");
        }
    }
}