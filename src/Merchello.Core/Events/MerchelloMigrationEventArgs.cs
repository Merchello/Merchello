namespace Merchello.Core.Events
{
    using System;

    using Merchello.Core.Models.Migrations;

    /// <summary>
    /// The merchello migration event args.
    /// </summary>
    internal class MerchelloMigrationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloMigrationEventArgs"/> class.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        public MerchelloMigrationEventArgs(MigrationRecord record)
        {
            MigrationRecord = record;
        }

        /// <summary>
        /// Gets or sets the migration record.
        /// </summary>
        public MigrationRecord MigrationRecord { get; set; }
    }
}