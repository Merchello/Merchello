using System.Collections.Generic;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Represent an abstract Repository for PetaPoco based repositories
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class MerchelloPetaPocoRepositoryBase<TId, TEntity> : MerchelloRepositoryBase<TId, TEntity>
        where TEntity : class, ISingularRoot
    {
		protected MerchelloPetaPocoRepositoryBase(IDatabaseUnitOfWork work)
			: base(work)
        {
        }

        protected MerchelloPetaPocoRepositoryBase(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
			: base(work, cache)
        {
        }

		/// <summary>
		/// Returns the database Unit of Work added to the repository
		/// </summary>
		protected internal new IDatabaseUnitOfWork UnitOfWork
		{
			get { return (IDatabaseUnitOfWork)base.UnitOfWork; }
		}

		protected UmbracoDatabase Database
        {
            get { return UnitOfWork.Database; }			
        }

        #region Abstract Methods
        
        protected abstract Sql GetBaseQuery(bool isCount);
        protected abstract string GetBaseWhereClause();
        protected abstract IEnumerable<string> GetDeleteClauses();
        protected abstract override void PersistNewItem(TEntity entity);
        protected abstract override void PersistUpdatedItem(TEntity entity);

        #endregion

        protected override bool PerformExists(TId id)
        {
            var sql = GetBaseQuery(true);
            sql.Where(GetBaseWhereClause(), new { Id = id});
            var count = Database.ExecuteScalar<int>(sql);
            return count == 1;
        }

        protected override int PerformCount(IQuery<TEntity> query)
        {
            var sqlClause = GetBaseQuery(true);
            var translator = new SqlTranslator<TEntity>(sqlClause, query);
            var sql = translator.Translate();

            return Database.ExecuteScalar<int>(sql);
        }

        protected override void PersistDeletedItem(TEntity entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new {Id = entity.Id});
            }
        }
    }
}