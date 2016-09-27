namespace Merchello.Core.Models.Migrations
{
    using Merchello.Core.Models.EntityBase;

    using Semver;

    /// <summary>
    /// Represents a migration status.
    /// </summary>
    public interface IMigrationStatus : IEntity
    {
        /// <summary>
        /// Gets or sets the migration name.
        /// </summary>
        string MigrationName { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        SemVersion Version { get; set; }
    }
}