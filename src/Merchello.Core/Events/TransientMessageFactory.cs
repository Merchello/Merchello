namespace Merchello.Core.Events
{
    using Umbraco.Core.Events;

    /// <summary>
    /// A simple/default transient messages factory
    /// </summary>
    internal class TransientMessageFactory : IEventMessagesFactory
    {
        /// <summary>
        /// Gets the Event Messages.
        /// </summary>
        /// <returns>
        /// The <see cref="EventMessages"/>.
        /// </returns>
        public EventMessages Get()
        {
            return new EventMessages();
        }
    }
}