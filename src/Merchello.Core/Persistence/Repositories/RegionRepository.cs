using System;
using System.Collections.Generic;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Repositories
{
    internal class RegionRepository : MerchelloPetaPocoRepositoryBase<IRegion>, IRegionRepository
    {
        public RegionRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache)
            : base(work, cache)
        { }

        protected override IRegion PerformGet(Guid key)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IRegion> PerformGetAll(params Guid[] keys)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IRegion> PerformGetByQuery(IQuery<IRegion> query)
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

        protected override void PersistNewItem(IRegion entity)
        {
            throw new NotImplementedException();
        }

        protected override void PersistUpdatedItem(IRegion entity)
        {
            throw new NotImplementedException();
        }
    }
}