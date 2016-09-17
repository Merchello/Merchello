namespace Merchello.Umbraco.DependencyInjection
{
    using LightInject;

    using Merchello.Umbraco.Mapping;

    /// <summary>
    /// Adds Umbraco native class mappings to the container
    /// </summary>
    public class UmbracoNativeMappingCompositionRoot : ICompositionRoot
    {
        /// <inheritdoc/>
        public void Compose(IServiceRegistry container)
        {
            container.Register<UmbracoNativeMappings>();
        }
    }
}