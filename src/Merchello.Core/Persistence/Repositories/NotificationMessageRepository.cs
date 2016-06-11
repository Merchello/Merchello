﻿namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents the NotificationMessageRepository
    /// </summary>
    internal class NotificationMessageRepository : MerchelloPetaPocoRepositoryBase<INotificationMessage>, INotificationMessageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMessageRepository"/> class.
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
        /// <param name="sqlSyntax">
        /// The SQL Syntax.
        /// </param>
        public NotificationMessageRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache, ILogger logger, ISqlSyntaxProvider sqlSyntax)
            : base(work, cache, logger, sqlSyntax)
        {            
        }

        /// <summary>
        /// Gets a value indicating whether is cached repository.
        /// </summary>
        /// <remarks>
        /// This will be removed when we refactor the NotificationContext.  
        /// 
        /// TODO The workflow in Notification Monitors updates cached models (which is by reference).  
        /// The triggers and monitors should be moved to use business layer models in Merchello.Web rather than Core models directly.
        /// </remarks>
        /// <seealso cref="http://issues.merchello.com/youtrack/issue/M-859"/>
        protected override bool IsCachedRepository
        {
            get
            {
                return false;
            }
        }

        protected override INotificationMessage PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
            .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<NotificationMessageDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new NotificationMessageFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<INotificationMessage> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    yield return Get(key);
                }
            }
            else
            {
                var factory = new NotificationMessageFactory();
                var dtos = Database.Fetch<NotificationMessageDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="INotificationMessage"/> by query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{INotificationMessage}"/>.
        /// </returns>
        protected override IEnumerable<INotificationMessage> PerformGetByQuery(IQuery<INotificationMessage> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<INotificationMessage>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<NotificationMessageDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        /// <summary>
        /// Gets the base query.
        /// </summary>
        /// <param name="isCount">
        /// The is count.
        /// </param>
        /// <returns>
        /// The <see cref="Sql"/>.
        /// </returns>
        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<NotificationMessageDto>(SqlSyntax);

            return sql;
        }

        /// <summary>
        /// Gets the base where clause.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected override string GetBaseWhereClause()
        {
            return "merchNotificationMessage.pk = @Key";
        }

        /// <summary>
        /// Gets a list of delete clauses.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchNotificationMessage WHERE pk = @Key"
            };

            return list;
        }

        /// <summary>
        /// Adds a new item to the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistNewItem(INotificationMessage entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new NotificationMessageFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();

            //RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<INotificationMethod>(entity.MethodKey));
        }

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void PersistUpdatedItem(INotificationMessage entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new NotificationMessageFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
           // RuntimeCache.ClearCacheItem(Cache.CacheKeys.GetEntityCacheKey<INotificationMethod>(entity.MethodKey));
        }
    }
}