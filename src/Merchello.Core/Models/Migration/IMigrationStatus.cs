namespace Merchello.Core.Models.Migration
{
    using Merchello.Core.Models.EntityBase;

    using Semver;

    /// <summary>
    /// The MigrationStatus interface.
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