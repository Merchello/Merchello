using System;

namespace Merchello.Core
{
    /// <summary>
    /// The merchello bootstrapper.
    /// </summary>
    internal class MerchelloBootstrapper
    {
        /// <summary>
        /// The init.
        /// </summary>
        /// <param name="bootManager">
        /// The boot manager.
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
