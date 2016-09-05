namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a MerchelloSettings configuration section
    /// </summary>
    /// <remarks>
    /// Responsible for the merchelloSettings.config
    /// </remarks>
    public interface IMerchelloSettingsSection : IMerchelloConfigurationSection
    {
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
        IBackOfficeSection BackOffice { get; }

        /// <inheritdoc/>
        IMigrationsSection Migrations { get; }

        /// <inheritdoc/>
        IViewsSection Views { get; }

        /// <inheritdoc/>
        IRoutingSection Routing { get; }

        /// <summary>
        /// Gets the custom currency formats.
        /// </summary>
        IEnumerable<ICurrencyFormat> CurrencyFormats { get; }
    }
}