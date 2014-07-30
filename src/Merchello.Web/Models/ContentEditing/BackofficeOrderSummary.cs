using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Web.Models.ContentEditing
{
    /// <summary>
    /// The backoffice order summary
    /// </summary>
    public class BackofficeOrderSummary
    {
        /// <summary>
        /// Gets or sets the item total.
        /// </summary>
        public decimal ItemTotal { get; set; }

        /// <summary>
        /// Gets or sets the shipping total.
        /// </summary>
        public decimal ShippingTotal { get; set; }

        /// <summary>
        /// Gets or sets the tax total.
        /// </summary>
        public decimal TaxTotal { get; set; }

        /// <summary>
        /// Gets or sets the invoice total.
        /// </summary>
        public decimal InvoiceTotal { get; set; }
    }
}
