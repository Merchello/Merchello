namespace Merchello.Core.Boot
{
    using System;

    /// <summary>
    /// A base class for Merchello BootManagers.
    /// </summary>
    public abstract class BootManagerBase
    {
        #region Events

        /// <summary>
        /// Occurs when Merchello is first initialized.
        /// </summary>
        public static event EventHandler MerchelloInit;

        /// <summary>
        /// Occurs before Merchello's setup starts.
        /// </summary>
        public static event EventHandler MerchelloStarting;

        /// <summary>
        /// The merchello started.
        /// </summary>
        public static event EventHandler MerchelloStarted;

        #endregion

        /// <summary>
        /// Initializes the startup processes.
        /// </summary>
        /// <returns>
        /// The <see cref="IBootManager"/>.
        /// </returns>
        public abstract IBootManager Initialize();

        /// <summary>
        /// The startup operation
        /// </summary>
        /// <param name="afterStartup">
        /// Action to be performed after startups completion.
        /// </param>
        /// <returns>
        /// The <see cref="IBootManager"/>.
        /// </returns>
        public abstract IBootManager Startup(Action<IMerchelloContext> afterStartup);

        /// <summary>
        /// A operation that occurs after startup has been completed.
        /// </summary>
        /// <param name="afterComplete">
        /// The after complete.
        /// </param>
        /// <returns>
        /// The <see cref="IBootManager"/>.
        /// </returns>
        public abstract IBootManager Complete(Action<IMerchelloContext> afterComplete);


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
            MerchelloStarting?.Invoke(sender, e);
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
            MerchelloStarted?.Invoke(sender, e);
        }

        /// <summary>
        /// Called to raise the MerchelloInit event
        /// </summary>        
        protected void OnMerchelloInit()
        {
            MerchelloInit?.Invoke(this, new EventArgs());
        }
    }
}
