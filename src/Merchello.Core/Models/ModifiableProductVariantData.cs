namespace Merchello.Core.Models
{
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    using Newtonsoft.Json;

    /// <summary>
    /// The modifiable product variant data.
    /// </summary>
    public abstract class ProductVariantDataModifierData : IProductVariantDataModifierData
    {
        /// <summary>
        /// The _modified data logs.
        /// </summary>
        private readonly List<IDataModifierLog> _modifiedDataLogs = new List<IDataModifierLog>();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the sale price.
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether on sale.
        /// </summary>
        public bool OnSale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether available.
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether track inventory.
        /// </summary>
        public bool TrackInventory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shippable.
        /// </summary>
        public bool Shippable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether taxable.
        /// </summary>
        public bool Taxable { get; set; }

        /// <summary>
        /// Gets or sets the modified data logs.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<IDataModifierLog> ModifiedDataLogs { get;  set; }

    }
}