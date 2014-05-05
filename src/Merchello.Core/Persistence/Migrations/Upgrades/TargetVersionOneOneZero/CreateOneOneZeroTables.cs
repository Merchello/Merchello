using System;
using System.Collections.Generic;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneZero
{
    /// <summary>
    /// Represents the creation of new database table introduced in Merchello 1.1.0
    /// </summary>
    internal class CreateOneOneZeroTables
    {
        private readonly Database _database;

        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            {0, typeof(NotificationMethodDto)},
            {1, typeof(NotificationMessageDto)}
        };

        public CreateOneOneZeroTables(Database database)
        {
            _database = database;
        }

        internal void InitializeDatabaseSchema()
        {
            DatabaseSchemaHelper.InitializeDatabaseSchema(_database, OrderedTables, "1.1.0 upgrade");
        }
    }
}