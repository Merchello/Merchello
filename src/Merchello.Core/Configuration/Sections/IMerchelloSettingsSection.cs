namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    using Merchello.Core.Configuration.Elements;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a MerchelloSettings configuration section
    /// </summary>
    /// <remarks>
    /// Responsible for the merchelloSettings.config
    /// </remarks>
    public interface IMerchelloSettingsSection : IMerchelloConfigurationSection
    {
        /// <summary>
        /// Gets the default connection string name for Merchello database connectivity.
        /// </summary>
        string DefaultConnectionStringName { get; }

        /// <summary>
        /// Gets a value indicating whether or not installs and upgrades can be tracked.
        /// </summary>
        bool EnableInstallTracking { get; }

        /// <inheritdoc/>
        IProductsSection Products { get; }

        /// <inheritdoc/>
        ICheckoutSection Checkout { get; }

        /// <inheritdoc/>
        ISalesSection Sales { get; }

        /// <inheritdoc/>
        ICustomersSection Customers { get; }

        /////// <inheritdoc/>
        ////ICollectionsSection Collections { get; }

        /// <inheritdoc/>
        IFiltersSection Filters { get; }


        /// <inheritdoc/>
        IMigrationsSection Migrations { get; }

        /// <inheritdoc/>
        IMvcSection Mvc { get; }

        /// <summary>
        /// Gets the custom currency formats.
        /// </summary>
        IEnumerable<ICurrencyFormat> CurrencyFormats { get; }
    }
}