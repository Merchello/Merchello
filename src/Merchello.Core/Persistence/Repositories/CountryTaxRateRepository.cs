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
    internal class CountryTaxRateRepository : MerchelloPetaPocoRepositoryBase<ICountryTaxRate>, ICountryTaxRateRepository
    {
        public CountryTaxRateRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) : base(work, cache)
        { }

        protected override ICountryTaxRate PerformGet(Guid key)
        {
            var sql = GetBaseQuery(false)
              .Where(GetBaseWhereClause(), new { Key = key });

            var dto = Database.Fetch<CountryTaxRateDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new CountryTaxRateFactory();
            return factory.BuildEntity(dto);
        }

        protected override IEnumerable<ICountryTaxRate> PerformGetAll(params Guid[] keys)
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
                var factory = new CountryTaxRateFactory();
                var dtos = Database.Fetch<CountryTaxRateDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        protected override IEnumerable<ICountryTaxRate> PerformGetByQuery(IQuery<ICountryTaxRate> query)
        {

            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<ICountryTaxRate>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<CountryTaxRateDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<CountryTaxRateDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchCountryTaxRate.pk = @Key";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {

            var list = new List<string>
            {
                "DELETE FROM merchCountryTaxRate WHERE pk = @Key"
            };

            return list;
        }

        protected override void PersistNewItem(ICountryTaxRate entity)
        {
            ((Entity)entity).AddingEntity();

            var factory = new CountryTaxRateFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(ICountryTaxRate entity)
        {
            ((Entity)entity).UpdatingEntity();

            var factory = new CountryTaxRateFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }
    }
}