namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;
    using System.Configuration;

    using Merchello.Core.Configuration.Elements;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents the MerchelloSettings configuration section.
    /// </summary>
    /// <remarks>
    /// Responsible for the merchelloSettings.config
    /// </remarks>
    internal class MerchelloSettingsSection : MerchelloConfigurationSection, IMerchelloSettingsSection
    {
        /// <inheritdoc/>
        [ConfigurationProperty("defaultConnectionStringName", DefaultValue = "umbracoDbDSN", IsRequired = false)]
        public string DefaultConnectionStringName
        {
            get { return (string)this["defaultConnectionStringName"]; }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("enableInstallTracking", DefaultValue = true, IsRequired = false)]
        public bool EnableInstallTracking
        {
            get { return (bool)this["enableInstallTracking"]; }
        }

        /// <inheritdoc/>
        IVirtualContentRouting IMerchelloSettingsSection.VirtualContentRouting { get; }

        /// <inheritdoc/>
        IEnumerable<ICurrencyFormat> IMerchelloSettingsSection.CurrencyFormats { get; }

        /// <summary>
        /// Gets the <see cref="IProductsSection"/>.
        /// </summary>
        IProductsSection IMerchelloSettingsSection.Products
        {
            get
            {
                return this.Products;
            }
        }

        /// <inheritdoc/>
        ICheckoutSection IMerchelloSettingsSection.Checkout
        {
            get
            {
                return this.Checkout;
            }
        }

        /// <inheritdoc/>
        ISalesSection IMerchelloSettingsSection.Sales { get; }

        /// <inheritdoc/>
        ICustomersSection IMerchelloSettingsSection.Customers { get; }

        /// <inheritdoc/>
        IFiltersSection IMerchelloSettingsSection.Filters { get; }

        /// <inheritdoc/>
        IBackOfficeSection IMerchelloSettingsSection.BackOffice { get; }

        /// <inheritdoc/>
        IMigrationsSection IMerchelloSettingsSection.Migrations { get; }

        /// <inheritdoc/>
        IViewsSection IMerchelloSettingsSection.Views { get; }

        /// <inheritdoc/>
        [ConfigurationProperty("products", IsRequired = true)]
        internal ProductsElement Products
        {
            get
            {
                return (ProductsElement)this["products"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("checkout", IsRequired = true)]
        internal CheckoutElement Checkout
        {
            get
            {
                return (CheckoutElement)this["checkout"];
            }
        }
    }
}