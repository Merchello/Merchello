namespace Merchello.Core.Persistence
{
    using System;

    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    /// <summary>
    /// Represents a SQL context.
    /// </summary>
    internal class SqlContext : ISqlContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlContext"/> class.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The sql syntax.
        /// </param>
        /// <param name="pocoDataFactory">
        /// The poco data factory.
        /// </param>
        /// <param name="databaseType">
        /// The database type.
        /// </param>
        public SqlContext(ISqlSyntaxProviderAdapter sqlSyntax, IPocoDataFactory pocoDataFactory, DatabaseType databaseType)
        {
            if (sqlSyntax == null) throw new ArgumentNullException(nameof(sqlSyntax));
            if (pocoDataFactory == null) throw new ArgumentNullException(nameof(pocoDataFactory));
            if (databaseType == null) throw new ArgumentNullException(nameof(databaseType));

            SqlSyntax = sqlSyntax;
            PocoDataFactory = pocoDataFactory;
            DatabaseType = databaseType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlContext"/> class.
        /// </summary>
        /// <param name="db">
        /// The <see cref="IMerchelloDatabase"/>.
        /// </param>
        public SqlContext(IMerchelloDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));
            SqlSyntax = db.SqlSyntax;
            PocoDataFactory = db.Database.PocoDataFactory;
            DatabaseType = db.Database.DatabaseType;
        }

        /// <summary>
        /// Gets the database type.
        /// </summary>
        public DatabaseType DatabaseType { get; }

        /// <summary>
        /// Gets the poco data factory.
        /// </summary>
        public IPocoDataFactory PocoDataFactory { get; }

        /// <summary>
        /// Gets the sql syntax.
        /// </summary>
        public ISqlSyntaxProviderAdapter SqlSyntax { get; }
    }
}