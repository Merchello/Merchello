namespace Merchello.Core.Models
{
    using System;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents an offer redeemed record.
    /// </summary>
    public interface IOfferRedeemed : IEntity
    {
        /// <summary>
        /// Gets or sets the offer settings key.
        /// </summary>
        Guid? OfferSettingsKey { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the redeemed date.
        /// </summary>
        DateTime RedeemedDate { get; set; }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets the extended data.
        /// </summary>
        //ExtendedDataCollection ExtendedData { get; }
    }
}