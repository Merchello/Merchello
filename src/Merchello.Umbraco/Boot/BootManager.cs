namespace Merchello.Umbraco.Boot
{
    using Merchello.Core.Logging;
    using Merchello.Web.Boot;

    /// <summary>
    /// Starts the Merchello Umbraco CMS Package.
    /// </summary>
    internal class BootManager : WebBootManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootManager"/> class.
        /// </summary>
        public BootManager()
            : this(new BootSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootManager"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IBootSettings"/>.
        /// </param>
        public BootManager(IBootSettings settings)
            : base(settings)
        {
        }
    }
}