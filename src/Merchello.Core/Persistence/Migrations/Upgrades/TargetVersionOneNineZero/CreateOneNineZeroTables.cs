namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneNineZero
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// Adds new database tables .
    /// </summary>
    [Migration("1.8.2", "1.9.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateOneNineZeroTables : IMerchelloMigration
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
        /// The schema helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _schemaHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOneNineZeroTables"/> class.
        /// </summary>
        public CreateOneNineZeroTables()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            _schemaHelper = new DatabaseSchemaHelper(dbContext.Database, LoggerResolver.Current.Logger, dbContext.SqlSyntax);
        }

        /// <summary>
        /// The up.
        /// </summary>
        public void Up()
        {
            foreach (var item in OrderedTables.OrderBy(x => x.Key))
            {
                _schemaHelper.CreateTable(false, item.Value);
            }
        }

        /// <summary>
        /// Throws a data loss exception
        /// </summary>
        public void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.9.0 database to a prior version, the database schema has already been modified");
        }
    }
}