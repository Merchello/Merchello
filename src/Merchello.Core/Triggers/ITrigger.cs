using System;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Defines a 
    /// </summary>
    internal interface ITrigger
    {
        /// <summary>
        /// Invokes the trigger action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Invoke(Object sender, EventArgs e);
        
    }
}