using System;
using System.Collections.Generic;
using Merchello.Core.Configuration;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core.Persistence.Migrations;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneZero
{
    /// <summary>
    /// Represents the creation of new database table introduced in Merchello 1.1.0
    /// </summary>
    //[Migration("1.1.0", 0, MerchelloConfiguration.MerchelloMigrationName)]
    public class CreateOneOneZeroTables : MigrationBase
    {
        public static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            {0, typeof(NotificationMethodDto)},
            {1, typeof(NotificationMessageDto)}
        };
  
        public override void Up()
        {
            //DatabaseSchemaHelper.InitializeDatabaseSchema(Context.Database, OrderedTables, "1.1.0 upgrade");
        }

        public override void Down()
        {
            throw new DataLossException("Cannot downgrade from a version 1.1.0 database to a prior version, the database schema has already been modified");
        }
    }
}