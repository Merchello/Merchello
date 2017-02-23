namespace Merchello.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Semver;

    /// <summary>
    /// Extension methods for versioning.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Gets <see cref="Version"/> from <see cref="SemVersion"/>.
        /// </summary>
        /// <param name="semVersion">
        /// The semantic version.
        /// </param>
        /// <param name="maxParts">
        /// The max parts.
        /// </param>
        /// <returns>
        /// The <see cref="Version"/>.
        /// </returns>
        internal static Version GetVersion(this SemVersion semVersion, int maxParts = 4)
        {
            var build = 0;
            int.TryParse(semVersion.Build, out build);

            if (maxParts >= 4)
            {
                return new Version(semVersion.Major, semVersion.Minor, semVersion.Patch, build);
            }

            if (maxParts == 3)
            {
                return new Version(semVersion.Major, semVersion.Minor, semVersion.Patch);
            }

            return new Version(semVersion.Major, semVersion.Minor);
        }
    }
}
