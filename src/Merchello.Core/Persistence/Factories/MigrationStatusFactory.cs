namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models.Migrations;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Represents a migration status factory.
    /// </summary>
    internal class MigrationStatusFactory : IEntityFactory<IMigrationStatus, MigrationStatusDto>
    {
        /// <inheritdoc/>
        public IMigrationStatus BuildEntity(MigrationStatusDto dto)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public MigrationStatusDto BuildDto(IMigrationStatus entity)
        {
            throw new System.NotImplementedException();
        }
    }
}