namespace Merchello.Core.Boot
{
    using System;

    /// <summary>
    /// Bootstraps Merchello.
    /// </summary>
    internal class MerchelloBootstrapper
    {
        /// <summary>
        /// Initializes the Bootstrap process.
        /// </summary>
        /// <param name="bootManager">
        /// The <see cref="BootManagerBase"/>.
        /// </param>
        public static void Init(BootManagerBase bootManager)
        {
            bootManager
                .Initialize()
                .Startup(merchContext => bootManager.OnMerchelloStarting(bootManager, new EventArgs()))
                .Complete(merchContext => bootManager.OnMerchelloStarted(bootManager, new EventArgs()));
        }
    }
}
