﻿using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal partial class BasketItemRepository : MerchelloPetaPocoRepositoryBase<int, IBasketItem>, IBasketItemRepository
    {

        public BasketItemRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public BasketItemRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IBasketItem>


        protected override IBasketItem PerformGet(int id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<BasketItemDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new BasketItemFactory();

            var basketItem = factory.BuildEntity(dto);

            return basketItem;
        }

        protected override IEnumerable<IBasketItem> PerformGetAll(params int[] ids)
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
                var factory = new BasketItemFactory();
                var dtos = Database.Fetch<BasketItemDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IBasketItem>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From("merchBasketItem");

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchBasketItem.id = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchBasketItem WHERE BasketItemPk = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(IBasketItem entity)
        {
            ((IdEntity)entity).AddingEntity();

            var factory = new BasketItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);
            entity.Id = dto.Id;
            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IBasketItem entity)
        {
            ((IdEntity)entity).UpdatingEntity();

            var factory = new BasketItemFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);
            
            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IBasketItem entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Id });
            }
        }


        protected override IEnumerable<IBasketItem> PerformGetByQuery(IQuery<IBasketItem> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IBasketItem>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<BasketItemDto>(sql);

            return dtos.DistinctBy(x => x.Id).Select(dto => Get(dto.Id));

        }


        #endregion



    }
}
