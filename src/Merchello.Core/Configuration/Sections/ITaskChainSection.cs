namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    /// <summary>
    ///  Represents a configuration section for configurations related to Merchello "task chains". 
    /// </summary>
    public interface ITaskChainSection : IMerchelloConfigurationSection
    {
        /// <summary>
        /// Gets a dictionary of configured <see cref="ITaskChain"/>.
        /// </summary>
        IDictionary<string, ITypeReference> Chains { get; }
    }
}