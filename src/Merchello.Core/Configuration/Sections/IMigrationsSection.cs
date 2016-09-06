namespace Merchello.Core.Configuration.Sections
{
    /// <summary>
    /// Represents a configuration section for configurations related to Merchello "migrations" (upgrade process). 
    /// </summary>
    public interface IMigrationsSection : IMerchelloConfigurationSection
    {

        /// <summary>
        /// Gets a value indicating whether or not to automatically run database schema changes when an install or upgrade migration
        /// has occurred.
        /// </summary>
        /// REFACTOR - this should be respected by new Migrations in V3
        bool AutoUpdateDbSchema { get; }
    }
}