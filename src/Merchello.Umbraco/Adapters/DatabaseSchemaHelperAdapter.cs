namespace Merchello.Umbraco.Adapters
{
    using System;

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
        /// <param name="dbFactory">
        /// Merchello database factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DatabaseSchemaHelperAdapter(DatabaseSchemaHelper schemaHelper, global::Merchello.Core.Persistence.IDatabaseFactory dbFactory, ILogger logger)
            : base(dbFactory, logger)
        {
            Ensure.ParameterNotNull(schemaHelper, nameof(schemaHelper));
            _helper = schemaHelper;
        }

        /// <inheritdoc/>
        public override bool TableExist(string tableName)
        {
            return _helper.TableExist(tableName);
        }

        /// <inheritdoc/>
        public override void CreateTable<T>(bool overwrite)
        {
            _helper.CreateTable<T>(overwrite);
        }

        /// <inheritdoc/>
        public override void DropTable(string tableName)
        {
            _helper.DropTable(tableName);
        }

        /// <inheritdoc/>
        public override void CreateTable<T>()
        {
            _helper.CreateTable<T>();
        }
    }
}