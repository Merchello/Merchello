namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// Represents a Merchello settings configuration section.
    /// </summary>
    /// <remarks>
    /// Responsible for the merchelloExtensibility.config
    /// </remarks>
    public interface IMerchelloExtensibilitySection : IMerchelloConfigurationSection
    {
        /// <inheritdoc/>
        IBackOfficeSection BackOffice { get; }

        /// <summary>
        /// Gets the pluggable objects.
        /// </summary>
        IDictionary<string, ITypeReference> PluggableObjects { get; }

        /// <summary>
        /// Gets the strategies.
        /// </summary>
        IDictionary<string, ITypeReference> Strategies { get; }

        /// <inheritdoc/>
        ITaskChainSection TaskChains { get; }

        /// <inheritdoc/>
        ITypeFieldsSection TypeFields { get; }
    }
}