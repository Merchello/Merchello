namespace Merchello.Core.Chains.InvoiceCreation
{
    using System.IO;
    using Models;
    using Sales;
    using Umbraco.Core;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Core.Services;
    using System.Linq;

    /// <summary>
    /// Represents a task responsible for adding a note collected during a checkout process to the
    /// invoice.
    /// </summary>
    internal class AddNotesToInvoiceTask : InvoiceCreationAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddBillingInfoToInvoiceTask"/> class.
        /// </summary>
        /// <param name="salePreparation">
        /// The sale preparation.
        /// </param>
        public AddNotesToInvoiceTask(SalePreparationBase salePreparation)
            : base(salePreparation)
        {            
        }

        /// <summary>
        /// Adds billing information to the invoice
        /// </summary>
        /// <param name="value">
        /// The <see cref="IInvoice"/>
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IInvoice> PerformTask(IInvoice value)
        {
                var noteDisplay = SalePreparation.Customer.ExtendedData.GetNote();
                if (noteDisplay != null)
                {
                    var note = new Note();
                    note.EntityKey = value.Key;
                    note.EntityTfKey = EnumTypeFieldConverter.EntityType.GetTypeField(EntityType.Invoice).TypeKey;
                    note.Message = noteDisplay.Message;
                    if (value.Notes != null)
                    {
                        if (!value.Notes.Where(x => x.Message == note.Message).Any())
                        {
                            value.Notes.Add(note);
                        }
                    }
                    else
                    {
                        value.Notes = new System.Collections.Generic.List<Note>();
                        value.Notes.Add(note);
                    }

                }

            return Attempt<IInvoice>.Succeed(value);            
        }
    }
}