namespace Merchello.Web.Models.ContentEditing
{
    /// <summary>
    /// The setting display.
    /// </summary>
    public class SettingDisplay
	{
        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the next order number.
        /// </summary>
        public int NextOrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the next invoice number.
        /// </summary>
        public int NextInvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Gets or sets the time format.
        /// </summary>
        public string TimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the unit format.
        /// </summary>
        public string UnitSystem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether global shippable.
        /// </summary>
        public bool GlobalShippable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether global taxable.
        /// </summary>
        public bool GlobalTaxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether global track inventory.
        /// </summary>
        public bool GlobalTrackInventory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether global shipping is taxable.
        /// </summary>
        public bool GlobalShippingIsTaxable { get; set; }
	}
}
