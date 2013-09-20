using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class BasketRepository : MerchelloPetaPocoRepositoryBase<int, IBasket>, IBasketRepository
    {

        public BasketRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public BasketRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }



        #region Overrides of RepositoryBase<IBasket>


        protected override IBasket PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<BasketDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new BasketFactory();

            var basket = factory.BuildEntity(dto);

            return basket;
        }

        protected override IEnumerable<IBasket> PerformGetAll(params int[] ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    yield return Get(id);
                }
            }
            else
            {
                var factory = new BasketFactory();
                var dtos = Database.Fetch<BasketDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IBasket>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchBasket");

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchBasket.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchBasketItem WHERE basketId = @Id",
                    "DELETE FROM merchBasket WHERE id = @Id"
                };

            return list;
        }

        protected override void PersistNewItem(IBasket entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new BasketFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IBasket entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new BasketFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IBasket entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IBasket> PerformGetByQuery(IQuery<IBasket> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IBasket>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<BasketDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }

        

        #endregion

    }
}
