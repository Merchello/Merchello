namespace Merchello.Core.Boot
{
    using System;
    using System.Collections.Generic;

    using LightInject;

    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Defines settings required for the <see cref="CoreBootManager"/>.
    /// </summary>
    internal interface ICoreBootSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the boot manager is being used in tests.
        /// </summary>
        bool IsForTesting { get; set; }
    }
}