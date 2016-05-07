namespace Merchello.Implementation
{
    using Merchello.Implementation.Attributes;
    using Merchello.Implementation.Controllers;
    using Merchello.Implementation.Resolvers;

    using Umbraco.Core;

    /// <summary>
    /// Registers Umbraco event handlers.
    /// </summary>
    public class UmbracoApplicationEventHandler : IApplicationEventHandler
    {
        /// <summary>
        /// Handles Umbraco Initializing.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The <see cref="UmbracoApplicationBase"/>.
        /// </param>
        /// <param name="applicationContext">
        /// Umbraco <see cref="ApplicationContext"/>.
        /// </param>
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        /// <summary>
        /// Handles Umbraco Starting.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The <see cref="UmbracoApplicationBase"/>.
        /// </param>
        /// <param name="applicationContext">
        /// Umbraco <see cref="ApplicationContext"/>.
        /// </param>
        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ViewRendererResolver.Current =
                new ViewRendererResolver(
                    PluginManager.Current.ResolveTypesWithAttribute<IViewRenderer, ComponentSetAliasAttribute>(),
                    ApplicationContext.Current.ApplicationCache);
        }

        /// <summary>
        /// Handles Umbraco Started.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The <see cref="UmbracoApplicationBase"/>.
        /// </param>
        /// <param name="applicationContext">
        /// Umbraco <see cref="ApplicationContext"/>.
        /// </param>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    }
}