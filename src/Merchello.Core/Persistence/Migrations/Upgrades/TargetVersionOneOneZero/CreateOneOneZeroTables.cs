using System;
using System.Collections.Generic;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Migrations.Initial;
using Umbraco.Core.Logging;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneZero
{
    /// <summary>
    /// Represents the creation of new database table introduced in Merchello 1.1.0
    /// </summary>
    internal class CreateOneOneZeroTables
    {
        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
        {
            {0, typeof(NotificationTriggerRuleDto)},
            {1, typeof(NotificationDto)}
        };

        internal void UninstallDatabaseSchema()
        {
            LogHelper.Info<CreateOneOneZeroTables>("Start 1.1.0 UninstallDataSchema");
        }
    }
}