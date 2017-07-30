namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchAnonymousCustomer" table.
    /// </summary>
    internal class AnonymousCustomerDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the last activity date of the anonymous customer.
        /// </summary>
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public string ExtendedData { get; set; }


        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}
