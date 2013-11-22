using System;
using System.Collections.Generic;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Persistence.UnitOfWork;
using Umbraco.Core.Cache;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;

namespace Merchello.Core.Persistence.Repositories
{
    internal class ShipRegionRepository : MerchelloPetaPocoRepositoryBase<IShipCountry>, IShipRegionRepository
    {
        public ShipRegionRepository(IDatabaseUnitOfWork work, IRuntimeCacheProvider cache) : base(work, cache)
        {
        }

        protected override IShipCountry PerformGet(Guid key)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IShipCountry> PerformGetAll(params Guid[] keys)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IShipCountry> PerformGetByQuery(IQuery<IShipCountry> query)
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

        protected override void PersistNewItem(IShipCountry entity)
        {
            throw new NotImplementedException();
        }

        protected override void PersistUpdatedItem(IShipCountry entity)
        {
            throw new NotImplementedException();
        }
    }
}