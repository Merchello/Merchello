using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Core
{
    /// <summary>
    /// The abstract class for the Merchello Plugin Application
    /// </summary>
    internal abstract class MerchelloPluginBase : IPluginApplication
    {
        public static event EventHandler PluginStarting;
        public static event EventHandler PluginStarted;

        /// <summary>
        /// Called when the Merchello plugin is initialized once the UmbracoApplication.Started event has fired
        /// </summary>
        public static event EventHandler PluginInit;

        /// <summary>
        /// Starts the Merchello Plugin Application
        /// </summary>        
        internal void StartPlugin(object sender, EventArgs e)
        {
            GetBootManager()
                .Initialize();
        }

        /// <summary>
        /// Initializes the Merchello Plugin
        /// </summary>        
        public void Plugin_Start(object sender, EventArgs e)
        {
            StartPlugin(sender, e);
        }

        public void Init()
        {
            OnPluginInit(this, new EventArgs());
        }

        /// <summary>
        /// Called to raise the PluginInit event
        /// </summary>        
        protected virtual void OnPluginInit(object sender, EventArgs e)
        {
            if (PluginInit != null)
                PluginInit(sender, e);
        }

        /// <summary>
        /// Developers can override this method to modify objects on startup
        /// </summary>        
        protected virtual void OnPluginStarting(object sender, EventArgs e)
        {
            if (PluginStarting != null)
                PluginStarting(sender, e);
        }

        /// <summary>
        /// Developers can override this method to modify objects once the application has been started
        /// </summary>
        protected virtual void OnPluginStarted(object sender, EventArgs e)
        {
            if (PluginStarted != null)
                PluginStarted(sender, e);
        }

        public abstract IBootManager GetBootManager();
    }
}
