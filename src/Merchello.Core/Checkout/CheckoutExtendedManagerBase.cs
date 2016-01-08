namespace Merchello.Core.Checkout
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// A base class for CheckoutExtendedManagers.
    /// </summary>
    /// <remarks>
    /// Contains methods for custom invoices
    /// </remarks>
    public abstract class CheckoutExtendedManagerBase : CheckoutCustomerDataManagerBase, ICheckoutExtendedManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutExtendedManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutExtendedManagerBase(ICheckoutContext context)
            : base(context)
        {
        }

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
        public abstract void AddItem(ILineItem lineItem);

        /// <summary>
        /// Removes a line item for the collection of items
        /// </summary>
        /// <param name="lineItem">
        /// The line item to be removed
        /// </param>
        public abstract void RemoveItem(ILineItem lineItem);

        /// <summary>
        /// Adds to get associated with the invoice as a note on invoice creation.
        /// </summary>
        /// <param name="message">
        /// The message or note body text.
        /// </param>
        public abstract void AddNote(string message);

        /// <summary>
        /// Gets any previously added notes.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public abstract IEnumerable<string> GetNotes();
    }
}