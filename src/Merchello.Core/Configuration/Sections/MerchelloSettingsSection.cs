namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents the MerchelloSettings configuration section.
    /// </summary>
    /// <remarks>
    /// Responsible for the merchelloSettings.config
    /// </remarks>
    public class MerchelloSettingsSection : MerchelloConfigurationSection, IMerchelloSettingsSection
    {
        /// <inheritdoc/>
        public IProductsSection Products { get; }

        /// <inheritdoc/>
        public ICheckoutSection Checkout { get; }

        /// <inheritdoc/>
        public ISalesSection Sales { get; }

        /// <inheritdoc/>
        public ICustomersSection Customers { get; }

        /// <inheritdoc/>
        public IFiltersSection Filters { get; }

        /// <inheritdoc/>
        public IBackOfficeSection BackOffice { get; }

        /// <inheritdoc/>
        public IMigrationsSection Migrations { get; }

        /// <inheritdoc/>
        public IViewsSection Views { get; }

        /// <inheritdoc/>
        public IRoutingSection Routing { get; }

        /// <inheritdoc/>
        public IEnumerable<ICurrencyFormat> CurrencyFormats { get; }
    }
}