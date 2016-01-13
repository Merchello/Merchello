namespace Merchello.Core.Checkout
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// The CheckoutExtendedManager.
    /// </summary>
    public interface ICheckoutExtendedManager : ICheckoutContextManagerBase
    {
        /// <summary>
        /// Adds a <see cref="ILineItem"/> to the collection of items
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <remarks>
        /// Intended for custom line item types
        /// http://issues.merchello.com/youtrack/issue/M-381
        /// </remarks>
        void AddItem(ILineItem lineItem);

        /// <summary>
        /// Removes a line item from the collection of items
        /// </summary>
        /// <param name="lineItem">
        /// The line item to be removed
        /// </param>
        void RemoveItem(ILineItem lineItem);

        /// <summary>
        /// Clears all notes.
        /// </summary>
        void ClearNotes();

        /// <summary>
        /// Saves a list of messages as notes.
        /// </summary>
        /// <param name="messages">
        /// The messages.
        /// </param>
        void SaveNotes(IEnumerable<string> messages);

        /// <summary>
        /// Adds to get associated with the invoice as a note on invoice creation.
        /// </summary>
        /// <param name="message">
        /// The message or note body text.
        /// </param>
        void AddNote(string message);

        /// <summary>
        /// Gets any previously added notes.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        IEnumerable<string> GetNotes();
    }
}