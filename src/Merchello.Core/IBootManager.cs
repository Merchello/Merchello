using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core
{
    internal interface IBootManager
    {
        /// <summary>
        /// Fires first in the Merchello application startup process before any customizations can occur
        /// </summary>
        /// <returns></returns>
        IBootManager Initialize();

        /// <summary>
        /// Fires after initialization and calls the callback to allow for customizations to occur
        /// </summary>
        /// <param name="afterStartup"></param>
        /// <returns></returns>
        IBootManager Startup(Action<MerchelloAppContext> afterStartup);

        /// <summary>
        /// Fires after startup and calls the callback once customizations are locked
        /// </summary>
        /// <param name="afterComplete"></param>
        /// <returns></returns>
        IBootManager Complete(Action<MerchelloAppContext> afterComplete);
    }
}
