namespace Merchello.Core.Models.Migrations
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Acquired;
    using Merchello.Core.Models.EntityBase;

    using Semver;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    public sealed class MigrationStatus : Entity, IMigrationStatus
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationStatus"/> class.
        /// </summary>
        public MigrationStatus()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationStatus"/> class.
        /// </summary>
        /// <param name="migrationName">
        /// The migration name.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="createDate">
        /// The create date.
        /// </param>
        public MigrationStatus(string migrationName, SemVersion version, DateTime createDate)
        {
            this.MigrationName = migrationName;
            this.Version = version;
            this.CreateDate = createDate;
            this.UpdateDate = UpdateDate;
        }

        /// <inheritdoc/>
        [DataMember]
        public string MigrationName
        {
            get
            {
                return this._migrationName;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref this._migrationName, _ps.Value.NameSelector); 
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public SemVersion Version
        {
            get
            {
                return this._version;
            }

            set
            {
                this.SetPropertyValueAndDetectChanges(value, ref this._version, _ps.Value.VersionSelector);
            }
        }

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