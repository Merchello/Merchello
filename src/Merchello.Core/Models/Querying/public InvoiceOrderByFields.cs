namespace Merchello.Core.Models.Querying
{
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The invoice order by fields.
    /// </summary>
    public class InvoiceOrderByFields : IOrderByField
    {
        /// <summary>
        /// Gets the invoice number.
        /// </summary>
        public static string InvoiceNumber
        {
            get
            {
                return "invoiceNumber";
            }
        }

        /// <summary>
        /// Gets the invoice date.
        /// </summary>
        public static string InvoiceDate
        {
            get
            {
                return "invoiceDate";
            }
        }

        /// <summary>
        /// Gets the bill to name.
        /// </summary>
        public static string BillToName
        {
            get
            {
                return "billToName";
            }
        }
    }
}