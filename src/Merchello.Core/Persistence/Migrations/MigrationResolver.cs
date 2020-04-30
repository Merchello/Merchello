namespace Merchello.Core.Persistence.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence.Migrations;

    /// <summary>
    /// The migration resolver.
    /// </summary>
    internal class MigrationResolver
    {
        /// <summary>
        /// The Logger.
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationResolver"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="instanceTypes">
        /// The instanceTypes.
        /// </param>
        internal MigrationResolver(ILogger logger, IEnumerable<Type> instanceTypes)
        {
            Ensure.ParameterNotNull(logger, "logger");
            _logger = logger;

            // ReSharper disable PossibleMultipleEnumeration
            Ensure.ParameterNotNull(instanceTypes, "instanceTypes");
            this.InstanceTypes = instanceTypes;
            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        /// Gets the instance types.
        /// </summary>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal IEnumerable<Type> InstanceTypes { get; private set; }

        /// <summary>
        /// The ordered upgrade migrations.
        /// </summary>
        /// <param name="currentVersion">
        /// The current version.
        /// </param>
        /// <param name="targetVersion">
        /// The target version.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IMigration}"/>.
        /// </returns>
        internal IEnumerable<IMigration> OrderedUpgradeMigrations(Version currentVersion, Version targetVersion)
        {
            var targetVersionToCompare = targetVersion;
            var currentVersionToCompare = currentVersion;

            var migrations = (from migration in InstanceTypes
                              let migrationAttributes = migration.GetCustomAttributes<MigrationAttribute>(false)
                              from migrationAttribute in migrationAttributes
                              where migrationAttribute != null
                              where migrationAttribute.TargetVersion > currentVersionToCompare &&
                                    migrationAttribute.TargetVersion <= targetVersionToCompare &&
                                    (migrationAttribute.MinimumCurrentVersion == null || currentVersionToCompare >= migrationAttribute.MinimumCurrentVersion)
                              orderby migrationAttribute.TargetVersion, migrationAttribute.SortOrder ascending
                              select migration).Distinct();

            var activated = migrations.Select(Activator.CreateInstance);

            return activated.Select(x => (IMigration)x);
        }
    }
}