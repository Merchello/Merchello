namespace Merchello.Core.Models.Migrations
{
    using System;
    using System.Runtime.Serialization;

    using Semver;

    /// <summary>
    /// Anonymous analytic data used to give the Merchello team an idea of how many installs there are.
    /// We will use this information to help assess usage growth or decline. 
    /// </summary>
    /// <remarks>
    /// You can completely disable this tracking by adding the attribute enableInstallTracking="false" to the merchello element (root)
    /// in the merchello.config file
    /// '<merchello  enableInstallTracking="False" />' 
    /// </remarks>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class MigrationRecord
    {
        /// <summary>
        /// Gets or sets the migration key.
        /// </summary>
        [DataMember]
        public string MigrationKey { get; set; }

        /// <summary>
        /// Gets or sets the DB provider.
        /// </summary>
        [DataMember]
        public string DbProvider { get; set; }

        /// <summary>
        /// Gets or sets the install date.
        /// </summary>
        [DataMember]
        public DateTime InstallDate { get; set; }

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
        [DataMember]
        public SemVersion CurrentVersion { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        [DataMember]
        public SemVersion TargetVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is upgrade.
        /// </summary>
        [DataMember]
        public bool IsUpgrade { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        [DataMember]
        public string DomainName { get; set; }

        //// TODO - add success and error messages
    }
}