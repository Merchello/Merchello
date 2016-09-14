namespace Merchello.Solo.Boot
{
    using Merchello.Web.Boot;

    /// <summary>
    /// Boots Merchello.
    /// </summary>
    internal partial class SoloBootManager : WebBootManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoloBootManager"/> class.
        /// </summary>
        public SoloBootManager()
            : this(new BootSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoloBootManager"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IBootSettings"/>.
        /// </param>
        public SoloBootManager(IBootSettings settings)
            : base(settings)
        {
        }
    }
}