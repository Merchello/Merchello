namespace Merchello.Core.Events
{
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

        //// REFACTOR - can this be removed

        /// <summary>
        /// Returns null for the default.
        /// </summary>
        /// <returns>
        /// The <see cref="EventMessages"/>.
        /// </returns>
        public EventMessages GetOrDefault()
        {
            return null;
        }
    }
}