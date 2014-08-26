namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core.Cache;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;

    /// <summary>
	/// Represent an abstract Repository for PetaPoco based repositories
	/// </summary>
	/// <typeparam name="TEntity">The type of entity</typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    internal abstract class MerchelloPetaPocoRepositoryBase<TEntity> : MerchelloRepositoryBase<TEntity>
		where TEntity : class, IEntity
	{
	
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloPetaPocoRepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        protected MerchelloPetaPocoRepositoryBase(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
			: base(work, cache)
		{
		}


        /// <summary>
		/// Gets the database Unit of Work added to the repository
		/// </summary>
		protected internal new IDatabaseUnitOfWork UnitOfWork
		{
			get { return (IDatabaseUnitOfWork)base.UnitOfWork; }
		}

        /// <summary>
        /// Gets the database.
        /// </summary>
        protected UmbracoDatabase Database
		{
			get { return UnitOfWork.Database; }			
		}

		#region Abstract Methods

        /// <summary>
        /// The get base query.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected abstract Sql GetBaseQuery(bool isCount);

        /// <summary>
        /// The get base where clause.
        /// </summary>
        /// <returns>
        /// The base "where" string.
        /// </returns>
        protected abstract string GetBaseWhereClause();

        /// <summary>
        /// The get delete clauses.
        /// </summary>
        /// <returns>
        /// The collection of delete clauses
        /// </returns>
        protected abstract IEnumerable<string> GetDeleteClauses();

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="entity">
        /// The entity to be deleted
        /// </param>
        protected abstract override void PersistNewItem(TEntity entity);

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="entity">
        /// The entity to be updated
        /// </param>
        protected abstract override void PersistUpdatedItem(TEntity entity);

		#endregion

        /// <summary>
        /// The perform exists.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool PerformExists(Guid key)
		{
			var sql = GetBaseQuery(true);
			sql.Where(GetBaseWhereClause(), new { Key = key});
			var count = Database.ExecuteScalar<int>(sql);
			return count == 1;
		}

        /// <summary>
        /// The perform count.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> count.
        /// </returns>
        protected override int PerformCount(IQuery<TEntity> query)
		{
			var sqlClause = GetBaseQuery(true);
			var translator = new SqlTranslator<TEntity>(sqlClause, query);
			var sql = translator.Translate();

			return Database.ExecuteScalar<int>(sql);
		}

        /// <summary>
        /// The persist deleted item.
        /// </summary>
        /// <param name="entity">
        /// The entity to be deleted.
        /// </param>
        protected override void PersistDeletedItem(TEntity entity)
		{
			var deletes = GetDeleteClauses();
			foreach (var delete in deletes)
			{
				Database.Execute(delete, new {Key = entity.Key});
			}
		}
	}
}