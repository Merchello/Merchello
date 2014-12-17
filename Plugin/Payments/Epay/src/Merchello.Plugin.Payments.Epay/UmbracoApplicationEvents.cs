namespace Merchello.Plugin.Payments.Epay
{
    using Umbraco.Core;

    /// <summary>
    /// Handles UmbracoApplicationEvents.
    /// </summary>
    public class UmbracoApplicationEvents : ApplicationEventHandler
    {
        /// <summary>
        /// Handles the Umbraco application started event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The Umbraco <see cref="UmbracoApplicationBase"/>
        /// </param>
        /// <param name="applicationContext">
        /// The Umbraco <see cref="ApplicationContext"/>
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }
    }
}