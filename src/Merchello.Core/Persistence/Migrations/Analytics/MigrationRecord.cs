namespace Merchello.Core.Persistence.Migrations.Analytics
{
    using System;

    /// <summary>
    /// Anonymous analytic data used to give the Merchello team an idea of how many installs there are.
    /// We will use this information to help assess usage growth or decline. 
    /// </summary>
    /// <remarks>
    /// You can completely disable this tracking by adding the attribute enableInstallTracking="false" to the merchello element (root)
    /// in the merchello.config file
    /// '<merchello  enableInstallTracking="False" />' 
    /// </remarks>
    internal class MigrationRecord
    {
        /// <summary>
        /// Gets or sets the migration key.
        /// </summary>
        public string MigrationKey { get; set; }

        /// <summary>
        /// Gets or sets the DB provider.
        /// </summary>
        public string DbProvider { get; set; }

        /// <summary>
        /// Gets or sets the install date.
        /// </summary>
        public DateTime InstallDate { get; set; }

        /// <summary>
        /// Gets or sets the current version.
        /// </summary>
        public string CurrentVersion { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string TargetVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is upgrade.
        /// </summary>
        public bool IsUpgrade { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        public string DomainName { get; set; }
    }
}