using System.Collections.Generic;
using Merchello.Core.Persistence.Caching;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Respositories
{
    internal abstract class SqlPetaPocoRepositoryBase<TId, TEntity> : SqlRepositoryBase<TId, TEntity>
        where TEntity : class, IEntity
    {
        protected SqlPetaPocoRepositoryBase(IUnitOfWork work) : 
            base(work)
        { }

        protected SqlPetaPocoRepositoryBase(IUnitOfWork work, IRepositoryCacheProvider cache) : 
            base(work, cache)
        { }

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

        protected abstract IEnumerable<string> GetDeleteClauses();
        protected abstract override void PersistNewItem(TEntity entity);
        protected abstract override void PersistUpdatedItem(TEntity entity);

        #endregion


        protected override bool PerformExists(TId id)
        {
            var sql = GetBaseQuery(true);
            sql.Where(GetBaseWhereClause(), new { Id = id });
            var count = Database.ExecuteScalar<int>(sql);
            return count == 1;
        }

        protected override int PerformCount(Sql query)
        {            
            return Database.ExecuteScalar<int>(query);
        }



    }
}
