namespace Merchello.Core.Models.Migration
{
    using System;
    using System.Reflection;

    using Merchello.Core.Acquired;
    using Merchello.Core.Models.EntityBase;

    using Semver;

    /// <summary>
    /// Represents a Merchello Migration status.
    /// </summary>
    public class MigrationStatus : Entity, IMigrationStatus
    {
        /// <summary>
        /// Lazy loads the property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The migration name.
        /// </summary>
        private string _migrationName;

        /// <summary>
        /// The version.
        /// </summary>
        private SemVersion _version;

        public MigrationStatus()
        {
        }

        public MigrationStatus(Guid key, string migrationName, SemVersion version, DateTime createDate, DateTime updateDate)
        {
            
        }

        public string MigrationName
        {
            get
            {
                return _migrationName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _migrationName, _ps.Value.NameSelector); 
            }
        }

        public SemVersion Version { get; set; }

        /// <summary>
        /// Property selectors.
        /// </summary>
        // ReSharper disable once ClassNeverInstantiated.Local
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<MigrationStatus, string>(x => x.MigrationName);

            /// <summary>
            /// The version selector.
            /// </summary>
            public readonly PropertyInfo VersionSelector = ExpressionHelper.GetPropertyInfo<MigrationStatus, SemVersion>(x => x.Version);
        }
    }
}