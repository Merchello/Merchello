namespace Merchello.Core.Configuration.Sections
{
    /// <summary>
    /// Represents a configuration section for configurations related to the Merchello MVC integrations.
    /// </summary>
    public interface IMvcSection : IMerchelloSection
    {
        /// <inheritdoc/>
        IViewsSection Views { get; }

        /// <summary>
        /// Gets the configuration section for configurations related to the custom Merchello MVC routes.
        /// </summary>
        IVirtualContentSection VirtualContent { get; }
    }
}