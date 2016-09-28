namespace Merchello.Umbraco.Adapters.Persistence
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
        private readonly Lazy<ISqlSyntaxProviderAdapter> _sqlSyntax;

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

            this._db = database;

            this._sqlSyntax = new Lazy<ISqlSyntaxProviderAdapter>(() => new SqlSyntaxProviderAdapter(this._db.SqlSyntax));
        }

        /// <inheritdoc/>
        public Database Database
        {
            get
            {
                return this._db;
            }
        }

        /// <inheritdoc/>
        public ISqlSyntaxProviderAdapter SqlSyntax
        {
            get
            {
                return this._sqlSyntax.Value;
            }
        }
    }
}