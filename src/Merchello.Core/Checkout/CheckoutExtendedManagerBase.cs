namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Sales;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// A base class for CheckoutExtendedManagers.
    /// </summary>
    /// <remarks>
    /// Contains methods for custom invoices
    /// </remarks>
    public abstract class CheckoutExtendedManagerBase : CheckoutCustomerDataManagerBase, ICheckoutExtendedManager
    {
        /// <summary>
        /// The list of messages to be converted into invoice notes.
        /// </summary>
        private Lazy<List<string>> _messages;  

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutExtendedManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutExtendedManagerBase(ICheckoutContext context)
            : base(context)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        private List<string> Messages
        {
            get
            {
                return this._messages.Value;
            }
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
        public virtual void AddItem(ILineItem lineItem)
        {
            Mandate.ParameterNotNullOrEmpty(lineItem.Sku, "The line item must have a sku");
            Mandate.ParameterNotNullOrEmpty(lineItem.Name, "The line item must have a name");

            if (lineItem.Quantity <= 0) lineItem.Quantity = 1;
            if (lineItem.Price < 0) lineItem.Price = 0;

            if (lineItem.LineItemType == LineItemType.Custom)
            {
                if (!new LineItemTypeField().CustomTypeFields.Select(x => x.TypeKey).Contains(lineItem.LineItemTfKey))
                {
                    var argError = new ArgumentException("The LineItemTfKey was not found in merchello.config custom type fields");
                    MultiLogHelper.Error<CheckoutContextManagerBase>("The LineItemTfKey was not found in merchello.config custom type fields", argError);

                    throw argError;
                }
            }

            Context.ItemCache.AddItem(lineItem);
        }

        /// <summary>
        /// Removes a line item for the collection of items
        /// </summary>
        /// <param name="lineItem">
        /// The line item to be removed
        /// </param>
        public virtual void RemoveItem(ILineItem lineItem)
        {
            Context.ItemCache.Items.Remove(lineItem.Sku);
        }

        /// <summary>
        /// Clears all notes.
        /// </summary>
        [Obsolete("User Reset()")]
        public void ClearNotes()
        {
            Reset();
        }

        /// <summary>
        /// Saves a list of messages as notes.
        /// </summary>
        /// <param name="messages">
        /// The messages.
        /// </param>
        public void SaveNotes(IEnumerable<string> messages)
        {
            // TODO - why are we clearing the notes here first?
            this._messages.Value.Clear();
            this._messages.Value.AddRange(messages);
            SaveCustomerTempData(Core.Constants.ExtendedDataKeys.Note, this._messages.Value);
        }

        /// <summary>
        /// Adds a message to be associated with the invoice as a note on invoice creation.
        /// </summary>
        /// <param name="message">
        /// The message or note body text.
        /// </param>
        public virtual void AddNote(string message)
        {
            if (!Messages.Contains(message))
            {
                this._messages.Value.Add(message);
                SaveCustomerTempData(Core.Constants.ExtendedDataKeys.Note, this._messages.Value);
            }
        }

        /// <summary>
        /// Gets any previously added notes.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public virtual IEnumerable<string> GetNotes()
        {
            return Messages;
        }

        /// <summary>
        /// Clears the notes.
        /// </summary>
        public override void Reset()
        {
            this._messages.Value.Clear();
            SaveCustomerTempData(Core.Constants.ExtendedDataKeys.Note, this._messages.Value);
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {            
            this._messages = new Lazy<List<string>>(() => BuildVersionedCustomerTempData(Core.Constants.ExtendedDataKeys.Note));
            if (Context.IsNewVersion && Context.Settings.ResetExtendedManagerDataOnVersionChange)
            {
                this.Reset();
            }
        } 
    }
}