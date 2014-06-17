using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
	/// <summary>
	/// Represent an abstract Repository, which is the base of the Repository implementations
	/// </summary>
	/// <typeparam name="TEntity">Type of <see cref="IEntity"/> entity for which the repository is used</typeparam>
	internal abstract class MerchelloRepositoryBase<TEntity> : DisposableObject, IRepositoryQueryable<Guid, TEntity>, IUnitOfWorkRepository 
		where TEntity : IEntity
	{
		private readonly IUnitOfWork _work;
		private readonly IRuntimeCacheProvider _cache;


		protected MerchelloRepositoryBase(IUnitOfWork work, IRuntimeCacheProvider cache)
		{
		    if (work == null) throw new ArgumentNullException("work");
		    if (cache == null) throw new ArgumentNullException("cache");
			_work = work;
			_cache = cache;
		}

		/// <summary>
		/// Returns the Unit of Work added to the repository
		/// </summary>
		protected internal IUnitOfWork UnitOfWork
		{
			get { return _work; }
		}

		/// <summary>
		/// Internal for testing purposes
		/// </summary>
		internal Guid UnitKey
		{
			get { return (Guid)_work.Key; }
		}

		#region IRepository<TEntity> Members

		/// <summary>
		/// Adds or Updates an entity of type TEntity
		/// </summary>
		/// <remarks>This method is backed by an <see cref="ICacheProvider"/> cache</remarks>
		/// <param name="entity"></param>
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
		/// <param name="entity"></param>
        public virtual void Delete(TEntity entity)
		{
			if (_work != null)
			{
				_work.RegisterRemoved(entity, this);
			}
		}

		protected abstract TEntity PerformGet(Guid key);
		/// <summary>
		/// Gets an entity by the passed in Id
		/// </summary>
		/// <returns></returns>
		public TEntity Get(Guid key)
		{
			var fromCache = TryGetFromCache(key);
			if (fromCache.Success)
			{
				return fromCache.Result;
			}

			var entity = PerformGet(key);
			if (entity != null)
			{
				_cache.GetCacheItem(GetCacheKey(key), () => entity);
			}

			if (entity != null)
			{
				entity.ResetDirtyProperties();
			}
			
			return entity;
		}

		protected Attempt<TEntity> TryGetFromCache(Guid key)
		{
			var cacheKey = GetCacheKey(key);
			var rEntity = _cache.GetCacheItem(cacheKey); 

			return rEntity != null ? 
				Attempt<TEntity>.Succeed((TEntity) rEntity) : 
				Attempt<TEntity>.Fail();
		} 

		protected abstract IEnumerable<TEntity> PerformGetAll(params Guid[] keys);

		/// <summary>
		/// Gets all entities of type TEntity or a list according to the passed in Ids
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		public IEnumerable<TEntity> GetAll(params Guid[] keys)
		{
			if (keys.Any())
			{
				var entities = new List<TEntity>();
				foreach (var key in keys)
				{
					var entity = _cache.GetCacheItem(GetCacheKey(key));
					if(entity != null) entities.Add((TEntity)entity);					
				}

				if (entities.Count().Equals(keys.Count()) && entities.Any(x => x == null) == false)
					return entities;
			}
			else
			{
				// fix http://issues.merchello.com/youtrack/issue/M-159
				// Since IProduct and IProductVaraint both start with IProduct which was causing the cache conflict
				var allEntities = _cache.GetCacheItemsByKeySearch(typeof (TEntity).Name + ".").ToArray(); //_cache.GetAllByType(typeof(TEntity));
				
				if (allEntities.Any())
				{
					
					var query = Querying.Query<TEntity>.Builder.Where(x => x.Key != Guid.Empty);
					var totalCount = PerformCount(query);

					if (allEntities.Count() == totalCount)
						return allEntities.Select(x => (TEntity)x);
				}
			}

			var entityCollection = PerformGetAll(keys).ToArray();

			foreach (var entity in entityCollection)
			{
				if (entity != null)
				{
					_cache.GetCacheItem(GetCacheKey(entity.Key), () => entity);
				}
			}

			return entityCollection;
		}

		protected abstract IEnumerable<TEntity> PerformGetByQuery(IQuery<TEntity> query);
		/// <summary>
		/// Gets a list of entities by the passed in query
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public IEnumerable<TEntity> GetByQuery(IQuery<TEntity> query)
		{
			return PerformGetByQuery(query);
		}

		protected abstract bool PerformExists(Guid key);

		/// <summary>
		/// Returns a boolean indicating whether an entity with the passed Key exists
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Exists(Guid key)
		{
			var fromCache = TryGetFromCache(key);
			if (fromCache.Success)
			{
				return true;
			}
			return PerformExists(key);            
		}

		protected abstract int PerformCount(IQuery<TEntity> query);
		/// <summary>
		/// Returns an integer with the count of entities found with the passed in query
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public int Count(IQuery<TEntity> query)
		{
			return PerformCount(query);
		}       

		#endregion

		#region IUnitOfWorkRepository Members

		/// <summary>
		/// Unit of work method that tells the repository to persist the new entity
		/// </summary>
		/// <param name="entity"></param>
		public virtual void PersistNewItem(IEntity entity)
		{
		    try
		    {
                PersistNewItem((TEntity)entity);
                _cache.GetCacheItem(GetCacheKey(entity.Key), () => entity);
		    }
		    catch (Exception)
		    {
                //if an exception is thrown we need to remove the entry from cache, this is ONLY a work around because of the way
                // that we cache entities: http://issues.umbraco.org/issue/U4-4259
                _cache.ClearCacheItem(GetCacheKey(entity.Key));
		        throw;
		    }
			
		}

		/// <summary>
		/// Unit of work method that tells the repository to persist the updated entity
		/// </summary>
		/// <param name="entity"></param>
		public virtual void PersistUpdatedItem(IEntity entity)
		{
		    try
		    {
                PersistUpdatedItem((TEntity)entity);
                _cache.GetCacheItem(GetCacheKey(entity.Key), () => entity);
		    }
		    catch (Exception)
		    {
                //if an exception is thrown we need to remove the entry from cache, this is ONLY a work around because of the way
                // that we cache entities: http://issues.umbraco.org/issue/U4-4259
                _cache.ClearCacheItem(GetCacheKey(entity.Key));
                throw;
		    }
			
		}

		/// <summary>
		/// Unit of work method that tells the repository to persist the deletion of the entity
		/// </summary>
		/// <param name="entity"></param>
		public virtual void PersistDeletedItem(IEntity entity)
		{
			PersistDeletedItem((TEntity)entity);
			_cache.ClearCacheItem(GetCacheKey(entity.Key));
		}

		#endregion

		#region Abstract IUnitOfWorkRepository Methods

		protected abstract void PersistNewItem(TEntity item);
		protected abstract void PersistUpdatedItem(TEntity item);
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

		protected IRuntimeCacheProvider RuntimeCache
		{
			get { return _cache; }
		}

		protected static string GetCacheKey(Guid key)
		{
			return Cache.CacheKeys.GetEntityCacheKey<TEntity>(key);
		}

	}
}