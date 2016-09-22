namespace Merchello.Umbraco.Boot
{
    using Merchello.Core;
    using Merchello.Umbraco.Adapters;

    using global::Umbraco.Core;

    /// <summary>
    /// Methods for getting boot manager settings and adapting Umbraco's instantiated objects.
    /// </summary>
    public partial class UmbracoBoot
    {
        /// <summary>
        /// Gets <see cref="IBootSettings"/> for Merchello startup.
        /// </summary>
        /// <param name="isForTesting">
        /// A value indicating this is startup is going to be used for testing.
        /// </param>
        /// <returns>
        /// The <see cref="IBootSettings"/>.
        /// </returns>
        private static IBootSettings GetBootSettings(bool isForTesting = false)
        {
            return new BootSettings(isForTesting);
        }
    }
}
