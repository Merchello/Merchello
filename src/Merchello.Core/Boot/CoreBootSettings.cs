namespace Merchello.Core.Boot
{
    using System;
    using System.Collections.Generic;

    using LightInject;

    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.SqlSyntax;

    /// <inheritdoc/>
    internal class CoreBootSettings : ICoreBootSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreBootSettings"/> class.
        /// </summary>
        public CoreBootSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreBootSettings"/> class with a logger.
        /// </summary>
        /// <param name="isForTesting">
        /// A value indicating that this startup is being used by tests
        /// </param>
        public CoreBootSettings(bool isForTesting = false)
        {
            this.IsForTesting = isForTesting;
        }

        /// <inheritdoc/>
        public ILogger Logger { get; set; }

        /// <inheritdoc/>
        public ISqlSyntaxProvider SqlSyntaxProvider { get; set; }

        /// <inheritdoc/>
        public IServiceContainer Container { get; set; }

        /// <summary>
        /// Gets or sets the cache helper.
        /// </summary>
        public CacheHelper CacheHelper { get; set; }

        /// <inheritdoc/>
        public bool IsForTesting { get; set; }
    }
}