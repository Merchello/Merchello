namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    /// <inheritdoc/>
    internal class MerchelloExtensibilitySection : MerchelloConfigurationSection, IMerchelloExtensibilitySection
    {
        /// <inheritdoc/>
        IBackOfficeSection IMerchelloExtensibilitySection.BackOffice { get; }

        /// <inheritdoc/>
        IDictionary<string, ITypeReference> IMerchelloExtensibilitySection.PluggableObjects { get; }

        /// <inheritdoc/>
        IDictionary<string, ITypeReference> IMerchelloExtensibilitySection.Strategies { get; }

        /// <inheritdoc/>
        ITaskChainSection IMerchelloExtensibilitySection.TaskChains { get; }

        /// <inheritdoc/>
        ITypeFieldsSection IMerchelloExtensibilitySection.TypeFields { get; }
    }
}