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
    internal class TaxMethodRepository : MerchelloPetaPocoRepositoryBase<ITaxMethod>, ITaxMethodRepository
    {
        public TaxMethodRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) : base(work, cache)
        { }

        protected override ITaxMethod PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<TaxMethodDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new TaxMethodFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<ITaxMethod> PerformGetAll(params Guid[] keys)
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
                var factory = new TaxMethodFactory();
                var dtos = Database.Fetch<TaxMethodDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<ITaxMethod> PerformGetByQuery(IQuery<ITaxMethod> query)
        {

            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ITaxMethod>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<TaxMethodDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<TaxMethodDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchTaxMethod.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {

            var list = new List<string>
            {
                "DELETE FROM merchTaxMethod WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(ITaxMethod entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new TaxMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ITaxMethod entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new TaxMethodFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}