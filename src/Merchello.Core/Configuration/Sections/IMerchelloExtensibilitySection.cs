namespace Merchello.Core.Configuration.Sections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a Merchello settings configuration section.
    /// </summary>
    /// <remarks>
    /// Responsible for the merchelloExtensibility.config
    /// </remarks>
    public interface IMerchelloExtensibilitySection : IMerchelloSection
    {
        /// <inheritdoc/>
        IBackOfficeSection BackOffice { get; }

        /// <summary>
        /// Gets the pluggable objects.
        /// </summary>
        IDictionary<string, Type> Pluggable { get; }

        /// <summary>
        /// Gets the strategies.
        /// </summary>
        IDictionary<string, Type> Strategies { get; }

        /// <inheritdoc/>
        IDictionary<string, IEnumerable<Type>> TaskChains { get; }

        /// <inheritdoc/>
        ITypeFieldsSection TypeFields { get; }
    }
}