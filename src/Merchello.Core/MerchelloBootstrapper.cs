using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core
{
    internal class MerchelloBootstrapper
    {
        public static void Init(BootManagerBase bootManager)
        {
            bootManager
                .Initialize()
                .Startup(merchContext => bootManager.OnMerchelloStarting(bootManager, new EventArgs()))
                .Complete(merchContext => bootManager.OnMerchelloStarted(bootManager, new EventArgs()));
        }
    }
}
