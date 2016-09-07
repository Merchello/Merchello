namespace Merchello.Core.Configuration.Elements
{
    using System.Collections.Generic;
    using System.Configuration;

    using Merchello.Core.Acquired.Configuration;
    using Merchello.Core.Configuration.Sections;

    /// <inheritdoc/>
    public class CustomersElement : ConfigurationElement, ICustomersSection
    {
        /// <inheritdoc/>
        int ICustomersSection.AnonymousCustomersMaxDays
        {
            get
            {
                return this.AnonymousCustomersMaxDays;
            }
        }

        /// <inheritdoc/>
        IEnumerable<string> ICustomersSection.MemberTypes
        {
            get
            {
                return this.MemberTypes;
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("anonymousCustomersMaxDays")]
        internal InnerTextConfigurationElement<int> AnonymousCustomersMaxDays
        {
            get
            {
                return (InnerTextConfigurationElement<int>)this["anonymousCustomersMaxDays"];
            }
        }

        /// <inheritdoc/>
        [ConfigurationProperty("memberTypes")]
        internal CommaDelimitedConfigurationElement MemberTypes
        {
            get
            {
                return (CommaDelimitedConfigurationElement)this["memberTypes"];
            }
        }
    }
}