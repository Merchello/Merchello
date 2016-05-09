namespace Merchello.Implementation
{
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