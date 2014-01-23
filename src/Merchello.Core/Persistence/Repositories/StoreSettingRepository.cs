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
    /// Represents the Store Settings Repository
    /// </summary>
    internal class StoreSettingRepository :  MerchelloPetaPocoRepositoryBase<IStoreSetting>, IStoreSettingRepository
    {
        public StoreSettingRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) 
            : base(work, cache)
        { }

        protected override IStoreSetting PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<StoreSettingDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new StoreSettingFactory();

            var setting = factory.BuildEntity(dto);

            return setting;
        }

        protected override IEnumerable<IStoreSetting> PerformGetAll(params Guid[] keys)
        {
            if (keys.Any())
            {
                foreach (var id in keys)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new StoreSettingFactory();
                var dtos = Database.Fetch<StoreSettingDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<IStoreSetting> PerformGetByQuery(IQuery<IStoreSetting> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IStoreSetting>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<StoreSettingDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<StoreSettingDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchStoreSetting.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE FROM merchStoreSetting WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(IStoreSetting entity)
        {

            ((Entity)entity).AddingEntity();

            var factory = new StoreSettingFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IStoreSetting entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new StoreSettingFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}