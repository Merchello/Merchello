namespace Merchello.Core.Events
{
    using System.Collections.Generic;

    /// <summary>
    /// Event messages collection
    /// </summary>
    /// <see cref="https://github.com/umbraco/Umbraco-CMS/blob/dev-v7/src/Umbraco.Core/Events/EventMessages.cs"/>
    public sealed class EventMessages : DisposableObject
    {
        /// <summary>
        /// A list of error messages.
        /// </summary>
        private readonly List<EventMessage> _msgs = new List<EventMessage>();

        /// <summary>
        /// Gets the current count of error messages.
        /// </summary>
        public int Count
        {
            get { return _msgs.Count; }
        }

        /// <summary>
        /// Adds a message to the collection.
        /// </summary>
        /// <param name="msg">
        /// The message to be added.
        /// </param>
        public void Add(EventMessage msg)
        {
            _msgs.Add(msg);
        }

        /// <summary>
        /// Gets all of the error messages.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{EventMessage}"/>.
        /// </returns>
        public IEnumerable<EventMessage> GetAll()
        {
            return _msgs;
        }

        /// <summary>
        /// Disposes the resources.
        /// </summary>
        protected override void DisposeResources()
        {
            _msgs.Clear();
        }
    }
}