namespace Merchello.Core.Models.Rdbms
{
    using System;
    
    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchCustomer" table.
    /// </summary>
    internal class CustomerDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the login name.
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tax exempt.
        /// </summary>
        public bool TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the last activity date.
        /// </summary>
        public DateTime LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [CanBeNull]
        public string ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        [CanBeNull]
        public string Notes { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}