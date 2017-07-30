namespace Merchello.Core.Models.Rdbms
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchStore2GatewayProviderSettings" table.
    /// </summary>
    internal class Store2GatewayProviderSettingsDto : IDto
    {
        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid StoreKey { get; set; }

        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        public Guid ProviderKey { get; set; }

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