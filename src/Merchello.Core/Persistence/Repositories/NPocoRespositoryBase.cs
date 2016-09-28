namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Acquired.Persistence.Querying;
    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.SqlSyntax;
    using Merchello.Core.Persistence.UnitOfWork;

    using NPoco;

    /// <summary>
    /// Represents a base NPoco repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity
    /// </typeparam>
    internal abstract class NPocoRespositoryBase<TEntity> : RepositoryWritableBase<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPocoRespositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="mappingResolver">
        /// The mapping resolver.
        /// </param>
        protected NPocoRespositoryBase(IDatabaseUnitOfWork work, ICacheHelper cache, ILogger logger, IMappingResolver mappingResolver)
            : base(work, cache, logger)
        {
            QueryFactory = new QueryFactory(SqlSyntax, mappingResolver);
        }

        /// <summary>
        /// Gets the query factory.
        /// </summary>
        public override IQueryFactory QueryFactory { get; }

        /// <summary>
        /// Creates a new query.
        /// </summary>
        /// <returns>A new query.</returns>
        public override IQuery<TEntity> Query => QueryFactory.Create<TEntity>();

        /// <summary>
        /// Gets the repository's database sql syntax.
        /// </summary>
        public ISqlSyntaxProviderAdapter SqlSyntax => UnitOfWork.Database.SqlSyntax;

        /// <summary>
        /// Gets the repository's unit of work.
        /// </summary>
        protected internal new IDatabaseUnitOfWork UnitOfWork => (IDatabaseUnitOfWork)base.UnitOfWork;

        /// <summary>
        /// Gets the repository's database.
        /// </summary>
        protected Database Database => UnitOfWork.Database.Database;

        /// <summary>
        /// Creates a new Sql statement.
        /// </summary>
        /// <returns>A new Sql statement.</returns>
        protected Sql<SqlContext> Sql()
        {
            return UnitOfWork.Database.Sql();
        }

        /// <inheritdoc/>
        protected override bool PerformExists(Guid key)
        {
            var sql = GetBaseQuery(true);
            sql.Where(GetBaseWhereClause(), new { Key = key });
            var count = Database.ExecuteScalar<int>(sql);
            return count == 1;
        }

        /// <inheritdoc/>
        protected override int PerformCount(IQuery<TEntity> query)
        {
            var sqlClause = GetBaseQuery(true);
            var translator = new SqlTranslator<TEntity>(sqlClause, query);
            var sql = translator.Translate();

            return Database.ExecuteScalar<int>(sql);
        }

        /// <inheritdoc/>
        protected override void PersistDeletedItem(TEntity entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Key = entity.Key });
            }
        }

        /// <summary>
        /// Gets the base SQL Query.
        /// </summary>
        /// <param name="isCount">
        /// A value indicating whether the SQL should be used for a count operation.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected abstract Sql<SqlContext> GetBaseQuery(bool isCount);

        /// <summary>
        /// Gets the base WHERE clause.
        /// </summary>
        /// <returns>
        /// The base WHERE clause.
        /// </returns>
        protected abstract string GetBaseWhereClause();

        /// <summary>
        /// Gets the collection of delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of delete clauses.
        /// </returns>
        protected abstract IEnumerable<string> GetDeleteClauses();
    }
}