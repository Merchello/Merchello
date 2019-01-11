using System;
using System.Collections.Generic;
using Umbraco.Core.Persistence;

namespace Merchello.Web.Models.Reports
{
    /// <summary>
    /// Model for the sales search report
    /// </summary>
    public class SalesSearchSnapshot
    {
        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the Search
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Invoice statuses
        /// </summary>
        public IEnumerable<InvStatus> InvoiceStatuses { get; set; }

        /// <summary>
        /// Gets or sets the variants
        /// </summary>
        public IEnumerable<ProductLineItem> Products { get; set; }

    }

    /// <summary>
    /// Model for the product line item
    /// </summary>
    public class ProductLineItem
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        ///  Gets or sets the total
        /// </summary>
        public double Total { get; set; }

        /// <summary>
        /// Gets or sets the variants
        /// </summary>
        public List<ProductLineItem> Variants { get; set; }       
    }

    /// <summary>
    /// Small model for the invoice status
    /// </summary>
    public class InvStatus
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the checked property for the status
        /// </summary>
        [Ignore]
        public bool Checked { get; set; }
    }
}
