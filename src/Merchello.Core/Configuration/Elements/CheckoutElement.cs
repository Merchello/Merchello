namespace Merchello.Core.Configuration.Elements
{
    using System.Configuration;

    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    internal class CheckoutElement : ConfigurationSection, ICheckoutSection
    {
        /// <inheritdoc/>
        ICheckoutContextSection ICheckoutSection.CheckoutContext
        {
            get
            {
                return this.CheckoutContext;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("checkoutContext", IsRequired = true)]
        internal CheckoutContextElement CheckoutContext
        {
            get
            {
                return (CheckoutContextElement)this["checkoutContext"];
            }
        }

    }
}