namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchStore2StoreSetting" table.
    /// </summary>
    internal class Store2StoreSettingDto : IDto
    {
        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid StoreKey { get; set; }

        /// <summary>
        /// Gets or sets the option key.
        /// </summary>
        public Guid SettingKey { get; set; }

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