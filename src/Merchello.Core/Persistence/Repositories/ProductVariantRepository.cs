using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Persistence.Caching;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class ProductVariantRepository : MerchelloPetaPocoRepositoryBase<Guid, IProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(IDatabaseUnitOfWork work) 
            : base(work)
        { }

        public ProductVariantRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache) 
            : base(work, cache)
        {
        }

        #region Overrides MerchelloPetaPocoRepositoryBase

        protected override IProductVariant PerformGet(Guid id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IProductVariant> PerformGetAll(params Guid[] ids)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IProductVariant> PerformGetByQuery(IQuery<IProductVariant> query)
        {
            throw new NotImplementedException();
        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            throw new NotImplementedException();
        }

        protected override string GetBaseWhereClause()
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            throw new NotImplementedException();
        }

        protected override void PersistNewItem(IProductVariant entity)
        {
            throw new NotImplementedException();
        }

        protected override void PersistUpdatedItem(IProductVariant entity)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}