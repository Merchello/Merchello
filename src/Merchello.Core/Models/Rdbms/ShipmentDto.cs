namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchShipment" table.
    /// </summary>
    internal class ShipmentDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the shipment number prefix.
        /// </summary>
        [CanBeNull]
        public string ShipmentNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        public int ShipmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the shipment status key.
        /// </summary>
        public Guid ShipmentStatusKey { get; set; }

        /// <summary>
        /// Gets or sets the shipped date.
        /// </summary>
        public DateTime ShippedDate { get; set; }

        /// <summary>
        /// Gets or sets the from organization.
        /// </summary>
        [CanBeNull]
        public string FromOrganization { get; set; }

        /// <summary>
        /// Gets or sets the from name.
        /// </summary>
        [CanBeNull]
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets the from address 1.
        /// </summary>
        [CanBeNull]
        public string FromAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the from address 2.
        /// </summary>
        [CanBeNull]
        public string FromAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the from locality.
        /// </summary>
        [CanBeNull]
        public string FromLocality { get; set; }

        /// <summary>
        /// Gets or sets the from region.
        /// </summary>
        [CanBeNull]
        public string FromRegion { get; set; }

        /// <summary>
        /// Gets or sets the from postal code.
        /// </summary>
        [CanBeNull]
        public string FromPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the from country code.
        /// </summary>
        [CanBeNull]
        public string FromCountryCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether from is commercial.
        /// </summary>
        public bool FromIsCommercial { get; set; }

        /// <summary>
        /// Gets or sets the to organization.
        /// </summary>
        [CanBeNull]
        public string ToOrganization { get; set; }

        /// <summary>
        /// Gets or sets the to name.
        /// </summary>
        [CanBeNull]
        public string ToName { get; set; }

        /// <summary>
        /// Gets or sets the to address 1.
        /// </summary>
        [CanBeNull]
        public string ToAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the to address 2.
        /// </summary>
        [CanBeNull]
        public string ToAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the to locality.
        /// </summary>
        [CanBeNull]
        public string ToLocality { get; set; }

        /// <summary>
        /// Gets or sets the to region.
        /// </summary>
        [CanBeNull]
        public string ToRegion { get; set; }

        /// <summary>
        /// Gets or sets the to postal code.
        /// </summary>
        [CanBeNull]
        public string ToPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the to country code.
        /// </summary>
        [CanBeNull]
        public string ToCountryCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to is commercial.
        /// </summary>
        public bool ToIsCommercial { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        [CanBeNull]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [CanBeNull]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the ship method key.
        /// </summary>
        [CanBeNull]
        public Guid? ShipMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the carrier.
        /// </summary>
        [CanBeNull]
        public string Carrier { get; set; }

        /// <summary>
        /// Gets or sets the tracking code.
        /// </summary>
        [CanBeNull]
        public string TrackingCode { get; set; }

        /// <summary>
        /// Gets or sets the tracking url.
        /// </summary>
        [CanBeNull]
        public string TrackingUrl { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}