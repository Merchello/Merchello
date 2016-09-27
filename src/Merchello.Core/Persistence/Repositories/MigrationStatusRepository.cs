namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.Migrations;
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Core.Persistence.UnitOfWork;

    using Semver;

    /// <inheritdoc/>
    internal partial class MigrationStatusRepository : IMigrationStatusRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationStatusRepository"/> class.
        /// </summary>
        /// <param name="work">
        /// The work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="mappingResolver">
        /// The mapping resolver.
        /// </param>
        public MigrationStatusRepository(IUnitOfWork work, ICacheHelper cache, ILogger logger, IMappingResolver mappingResolver)
            : base(work, cache, logger, mappingResolver)
        {
        }


        /// <inheritdoc/>
        public IMigrationStatus FindStatus(string migrationName, SemVersion version)
        {
            throw new NotImplementedException();
        }
    }
}