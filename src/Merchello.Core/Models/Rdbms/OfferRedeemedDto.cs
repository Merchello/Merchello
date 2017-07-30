namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchOfferRedeemed" table.
    /// </summary>
    internal class OfferRedeemedDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the offer settings key.
        /// </summary>
        /// <remarks>
        /// This accepts a null so that the offer can be deleted without having to 
        /// delete this reference
        /// </remarks>
        public Guid? OfferSettingsKey { get; set; }
        
        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        /// <remarks>
        /// This does not need to allow nulls since the key is not associated
        /// with a database constraint.
        /// </remarks>
        public Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        /// <remarks>
        /// Not all offers will be associated with known customers (could be anonymous)
        /// </remarks>
        [CanBeNull]
        public Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        /// <remarks>
        /// If the invoice is deleted - so will this record
        /// </remarks>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the redeemed date.
        /// </summary>
        public DateTime RedeemedDate { get; set; }

        /// <summary>
        /// Gets or sets the extended data serialization.
        /// </summary>
        [CanBeNull]
        public string ExtendedData { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}