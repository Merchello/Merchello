namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Defines a basket item UI component.
    /// </summary>
    public interface ILineItemModel : IUiModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the SKU.
        /// </summary>
        string Sku { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        /// <remarks>
        /// This could either by the price or the sale price of a product
        /// </remarks>
        decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LineItemType"/>.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        LineItemType LineItemType { get; set; }
    }
}