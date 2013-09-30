using System;
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
    internal class ProductRepository : MerchelloPetaPocoRepositoryBase<Guid, IProductActual>, IProductRepository
    {

        public ProductRepository(IDatabaseUnitOfWork work)
            : base(work)
        {

        }

        public ProductRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache)
            : base(work, cache)
        {
        }

        #region Overrides of RepositoryBase<IProduct>


        protected override IProductActual PerformGet(Guid id)
        {
            var sql = GetBaseQuery(false)
                .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<ProductActualDto>(sql).FirstOrDefault();

            if (dto == null)
                return null;

            var factory = new ProductFactory();

            var product = factory.BuildEntity(dto);

            return product;
        }

        protected override IEnumerable<IProductActual> PerformGetAll(params Guid[] ids)
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
                var factory = new ProductFactory();
                var dtos = Database.Fetch<ProductActualDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return factory.BuildEntity(dto);
                }
            }
        }

        #endregion

        #region Overrides of MerchelloPetaPocoRepositoryBase<IProduct>

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
               .From<ProductActualDto>();

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchProduct.pk = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
                {
                    "DELETE FROM merchProductActual WHERE pk = @Id",
                };

            return list;
        }

        protected override void PersistNewItem(IProductActual entity)
        {
            ((KeyEntity)entity).AddingEntity();

            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            Database.Insert(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IProductActual entity)
        {
            ((KeyEntity)entity).UpdatingEntity();

            var factory = new ProductFactory();
            var dto = factory.BuildDto(entity);

            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        protected override void PersistDeletedItem(IProductActual entity)
        {
            var deletes = GetDeleteClauses();
            foreach (var delete in deletes)
            {
                Database.Execute(delete, new { Id = entity.Key });
            }
        }


        protected override IEnumerable<IProductActual> PerformGetByQuery(IQuery<IProductActual> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProductActual>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductActualDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }


        #endregion



    }
}
