namespace Merchello.Umbraco.Boot
{
    using Merchello.Core.Logging;
    using Merchello.Web.Boot;

    /// <summary>
    /// Starts the Merchello Umbraco CMS Package.
    /// </summary>
    internal class UmbracoBootManager : WebBootManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoBootManager"/> class.
        /// </summary>
        public UmbracoBootManager()
            : this(new BootSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoBootManager"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IBootSettings"/>.
        /// </param>
        public UmbracoBootManager(IBootSettings settings)
            : base(settings)
        {
        }
    }
}