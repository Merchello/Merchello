namespace Merchello.Core.Events
{
    using System;

    using Merchello.Core.Models;

    /// <summary>
    /// The add item event args.
    /// </summary>
    public class AddItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddItemEventArgs"/> class.
        /// </summary>
        /// <param name="lineItem">
        /// The line Item.
        /// </param>
        public AddItemEventArgs(ILineItem lineItem)
        {
            this.LineItem = lineItem;
        }

        /// <summary>
        /// Gets or sets the <see cref="ILineItem"/>.
        /// </summary>
        public ILineItem LineItem { get; set; }
    }
}