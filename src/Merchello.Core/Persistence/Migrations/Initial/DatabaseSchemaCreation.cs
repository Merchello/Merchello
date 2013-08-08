using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence.Migrations.Initial
{
    // TODO Generate SQL SCRIPT and follow the order of table creation

    /// <summary>
    /// Represents the initial database schema creation by running CreateTable for all DTOs against the db.
    /// </summary>
    internal class DatabaseSchemaCreation
    {
        #region Private Members
        private readonly Database _database;

        private static readonly Dictionary<int, Type> OrderedTables = new Dictionary<int, Type>
            {
                {0, typeof(BasketDto)},
                {1, typeof(BasketItemDto)},
                {2, typeof(TypeFieldDto)}
            };

        #endregion
    }
}
