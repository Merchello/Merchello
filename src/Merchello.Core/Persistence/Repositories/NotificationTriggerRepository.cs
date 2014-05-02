using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Represents the NotificationTriggerRepository
    /// </summary>
    internal class NotificationTriggerRepository : MerchelloPetaPocoRepositoryBase<INotificationTrigger>, INotificationTriggerRepository
    {
        public NotificationTriggerRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        { }

        protected override INotificationTrigger PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
            .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<NotificationTriggerDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new NotificationTriggerFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<INotificationTrigger> PerformGetAll(params Guid[] keys)
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
                var factory = new NotificationTriggerFactory();
                var dtos = Database.Fetch<NotificationTriggerDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<INotificationTrigger> PerformGetByQuery(IQuery<INotificationTrigger> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<INotificationTrigger>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<NotificationTriggerDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<NotificationTriggerDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchNotificationTrigger.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            // TODO - if methods are associated with this trigger there will
            // be a constraint violation.  If this is ever exposed we will have to deal with it
            var list = new List<string>
            {
                "DELETE FROM merchNotificationTrigger WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(INotificationTrigger entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new NotificationTriggerFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(INotificationTrigger entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new NotificationTriggerFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}