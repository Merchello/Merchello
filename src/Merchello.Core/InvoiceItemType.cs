namespace Merchello.Core
{
    /// <summary>
    /// The type of a invoice line item
    /// </summary>
    public enum InvoiceItemType
    {         
        /// <summary>
        /// Catalog product sales
        /// </summary>
        Product = 0,

        /// <summary>
        /// A standard charge or fee
        /// </summary>
        Charge = 1,

        /// <summary>
        /// A shipping specific charge
        /// </summary>
        Shipping = 2,

        /// <summary>
        /// A tax related charge
        /// </summary>
        Tax = 3,

        /// <summary>
        /// A credit
        /// </summary>
        Credit = 4
    }
}
