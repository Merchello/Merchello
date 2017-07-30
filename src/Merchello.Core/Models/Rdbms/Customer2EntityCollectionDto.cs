namespace Merchello.Core.Models.Rdbms
{
    using System; 

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchCustomer2EntityCollection" table.
    /// </summary>
    internal class Customer2EntityCollectionDto : IDto
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        public Guid CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the product collection key.
        /// </summary>
        public Guid EntityCollectionKey { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}