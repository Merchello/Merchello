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
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Represents the NotificationMethodRepository
    /// </summary>
    internal class NotificationMethodRepository : MerchelloPetaPocoRepositoryBase<INotificationMethod>, INotificationMethodRepository
    {
        public NotificationMethodRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        { }

        protected override INotificationMethod PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
             .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<NotificationMethodDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new NotificationMethodFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<INotificationMethod> PerformGetAll(params Guid[] keys)
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
                var factory = new NotificationMethodFactory();
                var dtos = Database.Fetch<NotificationMethodDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<INotificationMethod> PerformGetByQuery(IQuery<INotificationMethod> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<INotificationMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<NotificationMethodDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<NotificationMethodDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchNotificationMethod.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchNotificationMessage WHERE methodKey = @Key",
                "DELETE FROM merchNotificationMethod WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(INotificationMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new NotificationMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(INotificationMethod entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new NotificationMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}