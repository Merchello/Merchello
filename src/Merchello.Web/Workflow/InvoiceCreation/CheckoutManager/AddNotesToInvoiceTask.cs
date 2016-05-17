namespace Merchello.Web.Workflow.InvoiceCreation.CheckoutManager
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Chains.InvoiceCreation.CheckoutManager;
    using Merchello.Core.Checkout;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Adds any notes to the invoice.
    /// </summary>
    internal class AddNotesToInvoiceTask : CheckoutManagerInvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNotesToInvoiceTask"/> class.
        /// </summary>
        /// <param name="checkoutManager">
        /// The <see cref="ICheckoutManagerBase"/>.
        /// </param>
        public AddNotesToInvoiceTask(ICheckoutManagerBase checkoutManager)
            : base(checkoutManager)
        {
        }

        /// <summary>
        /// Adds notes to an invoice
        /// </summary>
        /// <param name="value">
        /// The <see cref="IInvoice"/>
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
            var notes = CheckoutManager.Extended.GetNotes().ToArray();

            if (!notes.Any()) return Attempt<IInvoice>.Succeed(value);

            var notesList = notes.Select(msg => this.CheckoutManager.Context.Services.NoteService.CreateNote(value.Key, EntityType.Invoice, msg)).ToList();

            foreach (var note in notesList)
            {
                note.Author = value.BillToEmail;
                note.InternalOnly = false;
            }

            value.Notes = notesList;

            return Attempt<IInvoice>.Succeed(value);
        }
    }
}