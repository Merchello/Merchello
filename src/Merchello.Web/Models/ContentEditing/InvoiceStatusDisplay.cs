namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// The invoice status display.
    /// </summary>
    public class InvoiceStatusDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reportable.
        /// </summary>
        public bool Reportable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the status is an active status.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        public int SortOrder { get; set; }
    }
}