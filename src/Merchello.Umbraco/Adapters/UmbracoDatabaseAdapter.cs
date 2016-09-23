namespace Merchello.Umbraco.Adapters
{
    using System;

    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    /// <inheritdoc/>
    internal class UmbracoDatabaseAdapter : IMerchelloDatabase
    {
        /// <summary>
        /// Umbraco's database.
        /// </summary>
        private readonly global::Umbraco.Core.Persistence.UmbracoDatabase _db;

        /// <summary>
        /// The adapted SqlSyntaxProvider.
        /// </summary>
        private readonly Lazy<ISqlSyntaxProvider> _sqlSyntax;

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoDatabaseAdapter"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the database is null
        /// </exception>
        public UmbracoDatabaseAdapter(global::Umbraco.Core.Persistence.UmbracoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            _db = database;

            _sqlSyntax = new Lazy<ISqlSyntaxProvider>(() => new SqlSyntaxProviderAdapter(_db.SqlSyntax));
        }

        /// <inheritdoc/>
        public Database Database
        {
            get
            {
                return _db;
            }
        }

        /// <inheritdoc/>
        public ISqlSyntaxProvider SqlSyntax
        {
            get
            {
                return _sqlSyntax.Value;
            }
        }
    }
}