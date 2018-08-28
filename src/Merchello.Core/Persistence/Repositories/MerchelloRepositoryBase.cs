namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Models.EntityBase;
    using UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
	/// Represent an abstract Repository, which is the base of the Repository implementations
	/// </summary>
	/// <typeparam name="TEntity">Type of <see cref="IEntity"/> entity for which the repository is used</typeparam>
	internal abstract class MerchelloRepositoryBase<TEntity> : DisposableObject, IRepositoryQueryable<Guid, TEntity>, IUnitOfWorkRepository 
		where TEntity : class, IEntity
	{
        /// <summary>
        /// The unit of work.
        /// </summary>
        private readonly IUnitOfWork _work;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloRepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="work">
        /// The unit of work.
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/>.
        /// </param>
        protected MerchelloRepositoryBase(IUnitOfWork work, ILogger logger)
        {
            Mandate.ParameterNotNull(work, "work");
            Mandate.ParameterNotNull(logger, "logger");

            _work = work;
            _logger = logger;
        }

        /// <summary>
        /// Gets the unit of work key. Used for testing purposes
        /// </summary>
        internal Guid UnitKey
        {
            get { return (Guid)_work.Key; }
        }

        /// <summary>
        /// Gets the Unit of Work added to the repository
        /// </summary>
        protected internal IUnitOfWork UnitOfWork
        {
            get { return _work; }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger
        {
            get { return _logger; }
        }

        #region IUnitOfWorkRepository Members

        /// <summary>
        /// Unit of work method that tells the repository to persist the new entity
        /// </summary>
        /// <param name="entity">The entity to be created</param>
        public virtual void PersistNewItem(IEntity entity)
        {
            try
            {
                PersistNewItem((TEntity)entity);
            }
            catch (Exception ex)
            {
                LogHelper.Error(GetType(), "An error occurred trying to add a new entity", ex);
                ////if an exception is thrown we need to remove the entry from cache, this is ONLY a work around because of the way
                //// that we cache entities: http://issues.umbraco.org/issue/U4-4259
                throw;
            }
        }

        /// <summary>
        /// Unit of work method that tells the repository to persist the updated entity
        /// </summary>
        /// <param name="entity">The entity to be updated</param>
        public virtual void PersistUpdatedItem(IEntity entity)
        {
            try
            {
                PersistUpdatedItem((TEntity)entity);
            }
            catch (Exception ex)
            {

                LogHelper.Error(GetType(), "An error occurred trying to update an exiting entity", ex);
                ////if an exception is thrown we need to remove the entry from cache, this is ONLY a work around because of the way
                //// that we cache entities: http://issues.umbraco.org/issue/U4-4259
                throw;
            }
        }

        /// <summary>
        /// Unit of work method that tells the repository to persist the deletion of the entity
        /// </summary>
        /// <param name="entity">The entity to be deleted</param>
        public virtual void PersistDeletedItem(IEntity entity)
        {
            PersistDeletedItem((TEntity)entity);
        }

        #endregion


		#region IRepository<TEntity> Members

		/// <summary>
		/// Adds or Updates an entity of type TEntity
		/// </summary>
		/// <remarks>This method is backed by an <see cref="ICacheProvider"/> cache</remarks>
		/// <param name="entity">The entity to be added or updated</param>
		public void AddOrUpdate(TEntity entity)
		{
            if (entity.HasIdentity == false)
			{
				_work.RegisterAdded(entity, this);
			}
			else
			{
				_work.RegisterChanged(entity, this);
			}
		}

		/// <summary>
		/// Deletes the passed in entity
		/// </summary>
		/// <param name="entity">The entity to be deleted</param>
        public virtual void Delete(TEntity entity)
		{
			if (_work != null)
			{
				_work.RegisterRemoved(entity, this);
			}
		}

		/// <summary>
		/// Gets an entity by the passed in Id
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <returns>
		/// The entity retrieved
		/// </returns>
		public TEntity Get(Guid key)
		{

            // TODO - This is hit a lot, we need to find out why and where
			var entity = PerformGet(key);
			if (entity != null)
			{
				entity.ResetDirtyProperties();
			}
			
			return entity;
		}

        /// <summary>
        /// Gets all entities of type TEntity or a list according to the passed in Ids
        /// </summary>
        /// <param name="keys">The keys of the entities to be returned</param>
        /// <returns>A collection of entities</returns>
        public IEnumerable<TEntity> GetAll(params Guid[] keys)
        {
            return PerformGetAll(keys).ToArray();
        }

        /// <summary>
        /// Gets a list of entities by the passed in query
        /// </summary>
        /// <param name="query">The <see cref="IQuery{T}"/></param>
        /// <returns>A collection of entities</returns>
        public IEnumerable<TEntity> GetByQuery(IQuery<TEntity> query)
        {
            return PerformGetByQuery(query);
        }

        /// <summary>
        /// Returns a boolean indicating whether an entity with the passed Key exists
        /// </summary>
        /// <param name="key">The key of the entity</param>
        /// <returns>A value indicating whether or not the entity exists</returns>
        public bool Exists(Guid key)
        {
            return this.PerformExists(key);
        }

        /// <summary>
        /// Returns an integer with the count of entities found with the passed in query
        /// </summary>
        /// <param name="query">The <see cref="IQuery{T}"/></param>
        /// <returns>The count of entities</returns>
        public int Count(IQuery<TEntity> query)
        {
            return PerformCount(query);
        }

        /// <summary>
        /// Gets the cache key for the entity.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The cache key <see cref="string"/>.
        /// </returns>
        protected static string GetCacheKey(Guid key)
        {
            return Cache.CacheKeys.GetEntityCacheKey<TEntity>(key);
        }

        /// <summary>
        /// The perform get.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TEntity"/>.
        /// </returns>
        protected abstract TEntity PerformGet(Guid key);

        /// <summary>
        /// The abstract perform get all.
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The collection of all entities or with keys matching those in the parameter collection.
        /// </returns>
        protected abstract IEnumerable<TEntity> PerformGetAll(params Guid[] keys);

        /// <summary>
        /// The perform get by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The collection of entities matching the query
        /// </returns>
        protected abstract IEnumerable<TEntity> PerformGetByQuery(IQuery<TEntity> query);

        /// <summary>
        /// The perform exists.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// A value indicating whether or not an entity with the key exists.
        /// </returns>
        protected abstract bool PerformExists(Guid key);

        /// <summary>
        /// Perform count query
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> count
        /// </returns>
        protected abstract int PerformCount(IQuery<TEntity> query);

		#endregion

		#region Abstract IUnitOfWorkRepository Methods

        /// <summary>
        /// The persist new item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        protected abstract void PersistNewItem(TEntity item);

        /// <summary>
        /// The persist updated item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        protected abstract void PersistUpdatedItem(TEntity item);

        /// <summary>
        /// The persist deleted item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        protected abstract void PersistDeletedItem(TEntity item);

		#endregion
		
		/// <summary>
		/// Dispose disposable properties
		/// </summary>
		/// <remarks>
		/// Ensure the unit of work is disposed
		/// </remarks>
		protected override void DisposeResources()
		{
			UnitOfWork.DisposeIfDisposable();
		}
	}
}