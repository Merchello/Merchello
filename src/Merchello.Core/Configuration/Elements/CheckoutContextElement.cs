namespace Merchello.Core.Configuration.Elements
{
    using System.Configuration;

    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    internal class CheckoutContextElement : ConfigurationElement, ICheckoutContextSection
    {
        /// <inheritdoc/>
        string ICheckoutContextSection.InvoiceNumberPrefix
        {
            get
            {
                return this.InvoiceNumberPrefix;
            }
        }

        /// <inheritdoc/>
        bool ICheckoutContextSection.ApplyTaxesToInvoice
        {
            get
            {
                return this.ApplyTaxesToInvoice;
            }
        }

        /// <inheritdoc/>
        bool ICheckoutContextSection.RaiseCustomerEvents
        {
            get
            {
                return this.RaiseCustomerEvents;
            }
        }

        /// <inheritdoc/>
        bool ICheckoutContextSection.ResetCustomerManagerDataOnVersionChange
        {
            get
            {
                return this.ResetCustomerManagerDataOnVersionChange;
            }
        }

        /// <inheritdoc/>
        bool ICheckoutContextSection.ResetPaymentManagerDataOnVersionChange
        {
            get
            {
                return this.ResetPaymentManagerDataOnVersionChange;
            }
        }

        /// <inheritdoc/>
        bool ICheckoutContextSection.ResetExtendedManagerDataOnVersionChange
        {
            get
            {
                return this.ResetExtendedManagerDataOnVersionChange;
            }
        }

        /// <inheritdoc/>
        bool ICheckoutContextSection.ResetShippingManagerDataOnVersionChange
        {
            get
            {
                return this.ResetShippingManagerDataOnVersionChange;
            }
        }

        /// <inheritdoc/>
        bool ICheckoutContextSection.ResetOfferManagerDataOnVersionChange
        {
            get
            {
                return this.ResetOfferManagerDataOnVersionChange;
            }
        }

        /// <inheritdoc/>
        bool ICheckoutContextSection.EmptyBasketOnPaymentSuccess
        {
            get
            {
                return this.EmptyBasketOnPaymentSuccess;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("invoiceNumberPrefix")]
        internal InnerTextConfigurationElement<string> InvoiceNumberPrefix
        {
            get
            {
                return (InnerTextConfigurationElement<string>)this["invoiceNumberPrefix"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("applyTaxesToInvoice")]
        internal InnerTextConfigurationElement<bool> ApplyTaxesToInvoice
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["applyTaxesToInvoice"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("raiseCustomerEvents")]
        internal InnerTextConfigurationElement<bool> RaiseCustomerEvents
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["raiseCustomerEvents"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("resetCustomerManagerDataOnVersionChange")]
        internal InnerTextConfigurationElement<bool> ResetCustomerManagerDataOnVersionChange
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["resetCustomerManagerDataOnVersionChange"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("resetPaymentManagerDataOnVersionChange")]
        internal InnerTextConfigurationElement<bool> ResetPaymentManagerDataOnVersionChange
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["resetPaymentManagerDataOnVersionChange"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("resetExtendedManagerDataOnVersionChange")]
        internal InnerTextConfigurationElement<bool> ResetExtendedManagerDataOnVersionChange
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["resetExtendedManagerDataOnVersionChange"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("resetShippingManagerDataOnVersionChange")]
        internal InnerTextConfigurationElement<bool> ResetShippingManagerDataOnVersionChange
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["resetShippingManagerDataOnVersionChange"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("resetOfferManagerDataOnVersionChange")]
        internal InnerTextConfigurationElement<bool> ResetOfferManagerDataOnVersionChange
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["resetOfferManagerDataOnVersionChange"];
            }
        }
        
        /// <inheritdoc/>
        [ConfigurationProperty("emptyBasketOnPaymentSuccess")]
        internal InnerTextConfigurationElement<bool> EmptyBasketOnPaymentSuccess
        {
            get
            {
                return (InnerTextConfigurationElement<bool>)this["emptyBasketOnPaymentSuccess"];
            }
        }
    }
}