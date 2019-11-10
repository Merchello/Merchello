using System;
using Umbraco.Core;

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
        /// <param name="applicationContext"></param>
        public static void Init(BootManagerBase bootManager, ApplicationContext applicationContext)
        {
            bootManager
                .Initialize(applicationContext)
                .Startup(merchContext => bootManager.OnMerchelloStarting(bootManager, new EventArgs()))
                .Complete(merchContext => bootManager.OnMerchelloStarted(bootManager, new EventArgs()));
        }
    }
}
