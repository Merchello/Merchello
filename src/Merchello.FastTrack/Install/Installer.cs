namespace Merchello.FastTrack.Install
{
    using System;

    using Umbraco.Web.UI.Controls;

    /// <summary>
    /// An installer for FastTrack demo content.
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

            var dataInstaller = new FastTrackDataInstaller();
            var root = dataInstaller.Execute();

            StoreUrl = Umbraco.TypedContent(root.Id).Url;
        }
    }
}