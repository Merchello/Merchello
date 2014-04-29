using System.Runtime.Remoting.Messaging;

namespace Merchello.Core.Notifications
{
    /// <summary>
    /// Defines a message
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Sends the message
        /// </summary>
        /// <returns></returns>
        bool Send();

        //
    }
}