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
        /// <param name="appContext">
        /// Umbraco's <see cref="ApplicationContext"/>.
        /// </param>
        /// <param name="isForTesting">
        /// A value indicating this is startup is going to be used for testing.
        /// </param>
        /// <returns>
        /// The <see cref="IBootSettings"/>.
        /// </returns>
        private static IBootSettings GetBootSettings(ApplicationContext appContext, bool isForTesting = false)
        {
            Ensure.ParameterNotNull(appContext, nameof(appContext));

            return new BootSettings(isForTesting)
            {
                // Adapt Umbraco's logger
                Logger = new LoggerAdapter(appContext.ProfilingLogger.Logger)
            };
        }
    }
}
