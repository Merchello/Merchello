namespace Merchello.Core
{
    using System;

    public abstract class BootManagerBase
    {
        #region Events

        public static event EventHandler MerchelloInit;

        public static event EventHandler MerchelloStarting;

        public static event EventHandler MerchelloStarted;

        #endregion 

        public abstract IBootManager Initialize();

        public abstract IBootManager Startup(Action<MerchelloContext> afterStartup);
        
        public abstract IBootManager Complete(Action<MerchelloContext> afterComplete);


        /// <summary>
        /// Developers can override this method to modify objects on startup
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments
        /// </param>
        public void OnMerchelloStarting(object sender, EventArgs e)
        {
            if (MerchelloStarting != null)
                MerchelloStarting(sender, e);
        }

        /// <summary>
        /// Developers can override this method to modify objects once the application has been started
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments
        /// </param>
        public void OnMerchelloStarted(object sender, EventArgs e)
        {
            if (MerchelloStarted != null)
                MerchelloStarted(sender, e);
        }

        /// <summary>
        /// Called to raise the MerchelloInit event
        /// </summary>        
        protected void OnMerchelloInit()
        {
            if (MerchelloInit != null)
                MerchelloInit(this, new EventArgs());
        }      
    }
}
