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
        // advanced use attributes on the merchelloSettings.config root

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
        ISalesSection IMerchelloSettingsSection.Sales
        {
            get
            {
                return this.Sales;
            }
        }

        /// <inheritdoc/>
        ICustomersSection IMerchelloSettingsSection.Customers
        {
            get
            {
                return this.Customers;
            }
        }

        /// <inheritdoc/>
        IFiltersSection IMerchelloSettingsSection.Filters { get; }

        /// <inheritdoc/>
        IBackOfficeSection IMerchelloSettingsSection.BackOffice { get; }

        /// <inheritdoc/>
        IMigrationsSection IMerchelloSettingsSection.Migrations { get; }

        /// <inheritdoc/>
        IViewsSection IMerchelloSettingsSection.Views { get; }

        /// <inheritdoc/>
        IVirtualContentRouting IMerchelloSettingsSection.VirtualContentRouting { get; }

        /// <inheritdoc/>
        IEnumerable<ICurrencyFormat> IMerchelloSettingsSection.CurrencyFormats { get; }

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

        /// <inheritdoc/>
        [ConfigurationProperty("sales", IsRequired = true)]
        internal SalesElement Sales
        {
            get
            {
                return (SalesElement)this["sales"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("customers", IsRequired = true)]
        internal CustomersElement Customers
        {
            get
            {
                return (CustomersElement)this["customers"];
            }
        }
    }
}