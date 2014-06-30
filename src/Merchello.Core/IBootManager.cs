namespace Merchello.Core
{
    using System;

    /// <summary>
    /// The BootManager interface.
    /// </summary>
    public interface IBootManager
    {
        /// <summary>
        /// Fires first in the Merchello application startup process before any customizations can occur
        /// </summary>
        /// <returns>
        /// The <see cref="IBootManager"/>
        /// </returns>
        IBootManager Initialize();

        /// <summary>
        /// Fires after initialization and calls the callback to allow for customizations to occur
        /// </summary>
        /// <param name="afterStartup">
        /// The after startup action
        /// </param>
        /// <returns>
        /// The <see cref="IBootManager"/>
        /// </returns>
        IBootManager Startup(Action<MerchelloContext> afterStartup);

        /// <summary>
        /// Fires after startup and calls the callback once customizations are locked
        /// </summary>
        /// <param name="afterComplete">
        /// The after complete action
        /// </param>
        /// <returns>
        /// The <see cref="IBootManager"/>
        /// </returns>
        IBootManager Complete(Action<MerchelloContext> afterComplete);
    }
}
