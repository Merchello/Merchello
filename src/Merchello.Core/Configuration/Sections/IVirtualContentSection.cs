namespace Merchello.Core.Configuration.Sections
{
    /// <summary>
    /// Represents a configuration section for Merchello virtual (or detached) content configurations.
    /// </summary>
    public interface IVirtualContentSection : IMerchelloConfigurationSection
    {
        /// <summary>
        /// Gets the custom routing configurations for virtual content.
        /// </summary>
        IVirtualContentRoutingSection Routing { get; }
    }
}