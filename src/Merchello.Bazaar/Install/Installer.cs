namespace Merchello.Bazaar.Install
{
    using System;

    using global::Examine;

    using Umbraco.Core.Logging;
    using Umbraco.Web.UI.Controls;

    /// <summary>
    /// The installer.
    /// </summary>
    public class Installer : UmbracoUserControl
    {
        /// <summary>
        /// Gets the store url.
        /// </summary>
        public string StoreUrl { get; private set; }

        /// <summary>
        /// Handles the initialize event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="EventArgs"/>.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var dataInstaller = new BazaarDataInstaller();
            var root = dataInstaller.Execute();

            StoreUrl = Umbraco.TypedContent(root.Id).Url;
        }
    }
}