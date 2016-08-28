namespace Merchello.Core.Events
{
    /// <summary>
    /// Event messages factory
    /// </summary>
    public interface IEventMessagesFactory
    {
        /// <summary>
        /// Gets the Event Messages.
        /// </summary>
        /// <returns>
        /// The <see cref="EventMessages"/>.
        /// </returns>
        EventMessages Get();

        /// <summary>
        /// Gets the EventMessages or a default instance.
        /// </summary>
        /// <returns>
        /// The <see cref="EventMessages"/>.
        /// </returns>
        EventMessages GetOrDefault();
    }
}