namespace Merchello.Umbraco.Adapters.Persistence
{
    using Merchello.Core;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence;

    using global::Umbraco.Core.Persistence;

    /// <summary>
    /// An adapter for Umbraco's <see cref="DatabaseSchemaHelper"/>
    /// </summary>
    internal class DatabaseSchemaHelperAdapter : DatabaseSchemaManagerBase
    {
        /// <summary>
        /// The _helper.
        /// </summary>
        private readonly DatabaseSchemaHelper _helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchemaHelperAdapter"/> class.
        /// </summary>
        /// <param name="schemaHelper">
        /// Umbraco's DatabaseSchemaHelper.
        /// </param>
        /// <param name="database">
        /// Merchello;s database.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DatabaseSchemaHelperAdapter(DatabaseSchemaHelper schemaHelper, IMerchelloDatabase database, ILogger logger)
            : base(database, logger)
        {
            Ensure.ParameterNotNull(schemaHelper, nameof(schemaHelper));
            this._helper = schemaHelper;
        }

        /// <inheritdoc/>
        public override bool TableExist(string tableName)
        {
            return this._helper.TableExist(tableName);
        }

        /// <inheritdoc/>
        public override void CreateTable<T>(bool overwrite)
        {
            this._helper.CreateTable<T>(overwrite);
        }

        /// <inheritdoc/>
        public override void DropTable(string tableName)
        {
            this._helper.DropTable(tableName);
        }

        /// <inheritdoc/>
        public override void CreateTable<T>()
        {
            this._helper.CreateTable<T>();
        }
    }
}