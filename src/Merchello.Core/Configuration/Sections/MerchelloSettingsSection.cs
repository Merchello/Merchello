namespace Merchello.Core.Configuration.Sections
{
    using System;
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
    internal class MerchelloSettingsSection : MerchelloSection, IMerchelloSettingsSection
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
        IFiltersSection IMerchelloSettingsSection.Filters
        {
            get
            {
                return this.Filters;
            }
        }

        /// <inheritdoc/>
        IMigrationsSection IMerchelloSettingsSection.Migrations
        {
            get
            {
                return Migrations;
            }
        }

        /// <inheritdoc/>
        IEnumerable<ICurrencyFormat> IMerchelloSettingsSection.CurrencyFormats
        {
            get
            {
                return this.CurrencyFormats.GetCurrencyFormats();
            }
        }

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

        /// <inheritdoc/>
        [ConfigurationProperty("filters", IsRequired = true)]
        internal FiltersElement Filters
        {
            get
            {
                return (FiltersElement)this["filters"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("migrations", IsRequired = false)]
        internal MigrationsElement Migrations
        {
            get
            {
                return (MigrationsElement)this["migrations"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("currencyFormats", IsRequired = true)]
        internal CurrencyFormatsElement CurrencyFormats
        {
            get
            {
                return (CurrencyFormatsElement)this["currencyFormats"];
            }
        }
    }
}