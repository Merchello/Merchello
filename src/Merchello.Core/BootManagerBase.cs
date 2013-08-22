using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core
{
    public abstract class BootManagerBase
    {

        public abstract IBootManager Initialize();
        public abstract IBootManager Startup(Action<MerchelloContext> afterStartup);
        public abstract IBootManager Complete(Action<MerchelloContext> afterComplete);

        /// <summary>
        /// Called to raise the MerchelloInit event
        /// </summary>        
        protected void OnMerchelloInit()
        {
            if (MerchelloInit != null)
                MerchelloInit(this, new EventArgs());
        }

        /// <summary>
        /// Developers can override this method to modify objects on startup
        /// </summary>        
        public void OnMerchelloStarting(object sender, EventArgs e)
        {
            if (MerchelloStarting != null)
                MerchelloStarting(sender, e);
        }

        /// <summary>
        /// Developers can override this method to modify objects once the application has been started
        /// </summary>
        public void OnMerchelloStarted(object sender, EventArgs e)
        {
            if (MerchelloStarted != null)
                MerchelloStarted(sender, e);
        }

        #region Events

        public static event EventHandler MerchelloInit;
        public static event EventHandler MerchelloStarting;
        public static event EventHandler MerchelloStarted;

        #endregion
       
    }
}
