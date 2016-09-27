namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.Migrations;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
    using Merchello.Core.Persistence.Querying;

    using NPoco;

    /// <inheritdoc/>
    internal partial class MigrationStatusRepository : NPocoEntityRepositoryBase<IMigrationStatus, MigrationStatusDto, MigrationStatusFactory>
    {



        /// <inheritdoc/>
        protected override IEnumerable<IMigrationStatus> PerformGetByQuery(IQuery<IMigrationStatus> query)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        protected override void PersistNewItem(IMigrationStatus entity)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        protected override void PersistUpdatedItem(IMigrationStatus entity)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        protected override Sql<SqlContext> GetBaseQuery(bool isCount)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        protected override string GetBaseWhereClause()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        protected override IEnumerable<string> GetDeleteClauses()
        {
            throw new NotImplementedException();
        }
    }
}