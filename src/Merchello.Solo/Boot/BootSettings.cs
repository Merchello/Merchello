namespace Merchello.Solo.Boot
{
    using Merchello.Web.Boot;

    /// <inheritdoc/>
    internal class BootSettings : WebBootSettings, IBootSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootSettings"/> class.
        /// </summary>
        /// <param name="isForTesting">
        /// A value indicating this is startup is for testing.
        /// </param>
        public BootSettings(bool isForTesting = false)
        {
            this.IsForTesting = isForTesting;
        }
    }
}