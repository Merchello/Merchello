namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using LightInject;

    using Merchello.Core.Acquired.Persistence;
    using Merchello.Core.Cache;
    using Merchello.Core.DependencyInjection;
    using Merchello.Core.Logging;
    using Merchello.Core.Models.Migrations;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;
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
        public MigrationStatusRepository(IDatabaseUnitOfWork work, [Inject(Constants.Repository.DisabledCache)] ICacheHelper cache, ILogger logger, IMappingResolver mappingResolver)
            : base(work, cache, logger, mappingResolver)
        {
        }

        /// <inheritdoc/>
        public IMigrationStatus FindStatus(string migrationName, SemVersion version)
        {
            var versionString = version.ToString();

            var sql = Sql().SelectAll()
                .From<MigrationStatusDto>()
                .Where<MigrationStatusDto>(x => x.Name.InvariantEquals(migrationName) && x.Version == versionString);

            var result = Database.FirstOrDefault<MigrationStatusDto>(sql);

            var factory = new MigrationStatusFactory();

            return result == null ? null : factory.BuildEntity(result);
        }
    }
}