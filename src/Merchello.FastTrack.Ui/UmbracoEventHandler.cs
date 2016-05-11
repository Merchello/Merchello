namespace Merchello.FastTrack.Ui
{
    using Merchello.Web.Models.VirtualContent;

    using Umbraco.Core;

    /// <summary>
    /// Registers Umbraco event handlers.
    /// </summary>
    public class UmbracoEventHandler : IApplicationEventHandler
    {
        /// <summary>
        /// Handles Umbraco Initialized Event.
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
            // We handle the Initializing event so that we can set the parent node of the virtual content to the store
            // so that published content queries in views will work correctly
            ProductContentFactory.Initializing += ProductContentFactoryOnInitializing;
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

        //// Event handler methods

        /// <summary>
        /// Handles the <see cref="ProductContentFactory"/> on initializing event.
        /// </summary>
        /// <param name="sender">
        /// The <see cref="ProductContentFactory"/>.
        /// </param>
        /// <param name="e">
        /// The <see cref="VirtualContentEventArgs"/>.
        /// </param>
        /// <remarks>
        /// This is required to set the parent id of the virtual content
        /// </remarks>
        private static void ProductContentFactoryOnInitializing(ProductContentFactory sender, VirtualContentEventArgs e)
        {
            var store = StoreUiHelper.Content.GetStoreRoot();
            e.Parent = store;
        }
    }
}